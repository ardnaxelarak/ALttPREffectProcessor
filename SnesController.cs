using cs2snes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace ALttPREffectProcessor {
    public class SnesController {
        private static readonly List<int> PORTS = new() { 64213, 23074, 8080 };
        private static readonly List<int> UNSTARTED = new() { 0x00, 0x01, 0x02, 0x03, 0x04, 0x14 };
        private static readonly List<int> FINISHED = new() { 0x19, 0x1A };

        private static readonly SnesController instance = new();

        private readonly Snes snes;
        private int? port;
        private string? device;
        private int lastState = 0;
        private readonly List<EffectInstance> effectQueue = new();
        private readonly List<DeviceWrite> writes = new();
        private readonly MemoryCacher memory;
        private bool connected = false;

        public event Action<EffectData>? OnEffectStatusChange;
        public event Action? OnGameStart;
        public event Action? OnGameFinish;
        public event Action? OnConnected;
        public event Action? OnDisconnected;

        public static SnesController Instance {
            get => instance;
        }

        private SnesController() {
            memory = new(ReadInternal);
            snes = new();
            snes.SetTimeout(TimeSpan.FromSeconds(1));
        }

        public async Task Connect() {
            try {
                await ConnectInternal();

                List<string> devices = await snes.ListDevices();
                if (await PickDevice()) {
                    await snes.Attach(device);
                    Connected = true;
                }
            } catch (SnesException) {
                Connected = false;
            }
        }

        public async Task<bool> PickDevice() {
            try {
                List<string> devices = await snes.ListDevices();
                if (devices.Count > 0) {
                    device = devices[0];
                    return true;
                }
            } catch (SnesException) {
                Connected = false;
            }
            return false;
        }

        public async Task Main(CancellationToken token) {
            var stopwatch = new Stopwatch();
            while (!token.IsCancellationRequested) {
                try {
                    if (snes.State == WebSocketState.Open) {
                        TimeSpan elapsed = stopwatch.Elapsed;
                        stopwatch.Restart();
                        await Loop(elapsed);
                        await FlushWrites();
                        memory.Clear();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(0.05), token);
                } catch (TaskCanceledException) { }
            }
        }

        public void AddEffect(EffectData data) {
            EffectInstance effect = EffectInstance.GetEffect(data);
            effectQueue.Add(effect);
            OnEffectStatusChange?.Invoke(effect.Data.Clone());
        }

        public async Task CancelAll() {
            foreach (var effect in effectQueue) {
                await effect.Cancel();
                OnEffectStatusChange?.Invoke(effect.Data);
            }
            await FlushWrites();
        }

        public async Task ClearAll() {
            foreach (var effect in effectQueue) {
                await effect.Cancel();
                OnEffectStatusChange?.Invoke(effect.Data);
            }
            foreach (var effect in Effect.GetAllEffects()) {
                await effect.Clear();
            }
            await FlushWrites();
        }

        internal async Task<bool> CheckMemory(Dictionary<DataAddress, MemoryCondition> dict) {
            if (dict.Count == 0) return true;
            Dictionary<DataAddress, Task<int>> values = dict.Keys.ToDictionary(key => key, key => memory.ReadInt(key));
            await Task.WhenAll(values.Values);
            foreach (var key in dict.Keys) {
                if (!dict[key].IsValid(values[key].Result)) {
                    return false;
                }
            }
            return true;
        }

        internal async Task UpdateMemory(Dictionary<DataAddress, MemoryUpdate> dict) {
            if (dict.Count == 0) return;
            Dictionary<DataAddress, Task<int>> values = dict.Keys.ToDictionary(key => key, key => memory.ReadInt(key));
            await Task.WhenAll(values.Values);
            foreach (var key in dict.Keys) {
                Console.WriteLine($"{key.address:X6}.{key.size:X}");
                int? newValue = dict[key].NewValue(values[key].Result, addr => values[addr].Result);
                if (newValue.HasValue) {
                    QueueWrite(key, newValue.Value);
                }
            }
        }

        private bool Connected {
            get => connected;
            set {
                if (connected != value) {
                    if (value) {
                        try {
                            OnConnected?.Invoke();
                        } catch (Exception) { }
                    } else {
                        try {
                            OnDisconnected?.Invoke();
                        } catch (Exception) { }
                    }
                }
                connected = value;
            }
        }

        private async Task Loop(TimeSpan elapsed) {
            int state;
            try {
                state = await memory.ReadInt(Addresses.GameState);
                Connected = true;
            } catch (SnesException) {
                Connected = false;
                return;
            }

            if (UNSTARTED.Contains(lastState & 0xFF) && !UNSTARTED.Contains(state & 0xFF)) {
                OnGameStart?.Invoke();
            } else if (!FINISHED.Contains(lastState & 0xFF) && FINISHED.Contains(state & 0xFF)) {
                OnGameFinish?.Invoke();
            }

            lastState = state;

            foreach (var effect in effectQueue.Where(x => x.Status == EffectStatus.InProgress).ToList()) {
                try {
                    await effect.Step(state, elapsed);
                    OnEffectStatusChange?.Invoke(effect.Data.Clone());
                    Connected = true;
                } catch (SnesException) {
                    Connected = false;
                }
            }

            HashSet<string> typesInProgress = new(effectQueue.Where(x => x.Status == EffectStatus.InProgress).Select(x => x.Category));

            foreach (var effect in effectQueue.Where(x => x.Status == EffectStatus.Unstarted).ToList()) {
                if (typesInProgress.Contains(effect.Category)) continue;
                try {
                    await effect.Start(state);
                    OnEffectStatusChange?.Invoke(effect.Data.Clone());
                    if (effect.Status == EffectStatus.InProgress) {
                        typesInProgress.Add(effect.Category);
                    }
                    Connected = true;
                } catch (SnesException) {
                    Connected = false;
                }
            }

            effectQueue.RemoveAll(x => x.Status == EffectStatus.Finished || x.Status == EffectStatus.Failed);
        }

        private async Task<List<byte>> ReadInternal(int address, int length) {
            try {
                return await snes.ReadMemory(address, length);
            } catch (SnesException) {
                await TryReattach();
                return await snes.ReadMemory(address, length);
            }
        }

        private void QueueWrite(DataAddress addr, int value) {
            Console.WriteLine($"Writing {value:X} to {addr.address:X6}");
            int realAddress = addr.bank switch {
                DataBank.Rom => addr.address,
                _ => 0xF50000 + addr.address,
            };
            List<byte> data = new();
            for (int i = 0; i < addr.size; i++) {
                data.Add((byte) ((value >> (8 * i)) & 0xFF));
            }
            writes.Add(new() { Address = realAddress, Bytes = data });
        }

        private async Task FlushWrites() {
            if (writes.Count == 0) {
                return;
            }

            try {
                try {
                    await snes.WriteMemory(writes);
                } catch (SnesException) {
                    await TryReattach();
                    await snes.WriteMemory(writes);
                }
                writes.Clear();
            } catch (SnesException) {
                Connected = false;
            }
        }

        private async Task ConnectInternal() {
            if (port.HasValue) {
                await ConnectToPort(port.Value);
            } else {
                foreach (int testPort in PORTS) {
                    try {
                        await ConnectToPort(testPort);
                        port = testPort;
                        return;
                    } catch (SnesException) { }
                }
                throw new SnesException("Could not find a valid port to connect to.");
            }
        }

        private async Task ConnectToPort(int port) {
            await snes.Connect($"ws://localhost:{port}");
        }

        private async Task TryReattach() {
            if (snes.State != System.Net.WebSockets.WebSocketState.Open) {
                await ConnectInternal();
            } 
            if (device is not null || await PickDevice()) {
                try {
                    await snes.Attach(device);
                } catch (SnesException) {
                    Connected = false;
                }
            }
        }
    }
}
