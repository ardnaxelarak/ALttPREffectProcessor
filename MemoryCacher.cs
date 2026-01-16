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
            this.memoryCache.Clear();
        }

        public async Task Cache(DataAddress addr) {
            int realAddress = GetRealAddress(addr);
            List<byte> result = await this.memoryReader(realAddress, addr.size);
            for (int i = 0; i < result.Count; i++) {
                this.memoryCache[realAddress + i] = result[i];
            }
        }

        public async Task<byte[]> ReadBytes(DataAddress addr, int offset = 0) {
            int realAddress = GetRealAddress(addr);
            if (!Enumerable.Range(realAddress + offset, addr.size - offset).All(address => this.memoryCache.ContainsKey(address))) {
                await Cache(addr);
            }
            return Enumerable.Range(realAddress + offset, addr.size - offset).Select(address => this.memoryCache[address]).ToArray();
        }

        public async Task<int> ReadFixedInt(DataAddress addr, int size, int offset = 0) {
            byte[] result = await this.ReadBytes(addr, offset);

            int value = 0;
            for (int i = 0; i < Math.Min(result.Length, size); i++) {
                value |= result[i] << (8 * i);
            }
            return value;
        }

        public async Task<int> ReadInt(DataAddress addr) => await this.ReadFixedInt(addr, 4);

        private static int GetRealAddress(DataAddress addr) {
            return addr.bank switch {
                DataBank.Rom => addr.address,
                _ => 0xF50000 + addr.address,
            };
        }
    }
}
