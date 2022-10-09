using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALttPREffectProcessor {
    internal class MemoryCacher {
        private readonly Func<int, int, Task<List<byte>>> memoryReader;
        private readonly Dictionary<int, byte> memoryCache = new();

        public MemoryCacher(Func<int, int, Task<List<byte>>> memoryReader) {
            this.memoryReader = memoryReader;
        }

        public void Clear() {
            memoryCache.Clear();
        }

        public async Task Cache(DataAddress addr) {
            int realAddress = GetRealAddress(addr);
            List<byte> result = await memoryReader(realAddress, addr.size);
            for (int i = 0; i < result.Count; i++) {
                memoryCache[realAddress + i] = result[i];
            }
        }

        public async Task<byte[]> ReadBytes(DataAddress addr) {
            int realAddress = GetRealAddress(addr);
            if (!Enumerable.Range(realAddress, addr.size).All(address => memoryCache.ContainsKey(address))) {
                await Cache(addr);
            }
            return Enumerable.Range(realAddress, addr.size).Select(address => memoryCache[address]).ToArray();
        }

        public async Task<int> ReadInt(DataAddress addr) {
            byte[] result = await ReadBytes(addr);

            int value = 0;
            for (int i = 0; i < Math.Min(result.Length, 4); i++) {
                value |= result[i] << (8 * i);
            }
            return value;
        }

        public async Task<long> ReadLong(DataAddress addr) {
            byte[] result = await ReadBytes(addr);

            int value = 0;
            for (int i = 0; i < Math.Min(result.Length, 8); i++) {
                value |= result[i] << (8 * i);
            }
            return value;
        }

        private int GetRealAddress(DataAddress addr) {
            return addr.bank switch {
                DataBank.Rom => addr.address,
                _ => 0xF50000 + addr.address,
            };
        }
    }
}
