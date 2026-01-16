using System;
using System.Collections.Generic;
using System.Linq;

namespace ALttPREffectProcessor {
    internal class TrackingCache {
        private readonly Dictionary<int, byte> memoryCache = new();

        public TrackingCache() { }

        public void Update(Dictionary<DataAddress, byte[]> values) {
            this.memoryCache.Clear();
            foreach (KeyValuePair<DataAddress, byte[]> kvp in values) {
                int realAddress = GetRealAddress(kvp.Key);
                for (int i = 0; i < kvp.Value.Length; i++) {
                    this.memoryCache[realAddress + i] = kvp.Value[i];
                }
            }
        }

        public byte ReadWramByte(int addr) {
            int realAddress = 0xF50000 + addr;
            if (this.memoryCache.TryGetValue(realAddress, out byte result)) {
                return result;
            } else {
                throw new MemoryNotCachedException(new() { realAddress });
            }
        }

        public byte[] ReadBytes(DataAddress addr, int offset = 0) {
            int realAddress = GetRealAddress(addr);
            List<int> uncached = Enumerable.Range(realAddress + offset, addr.size - offset).Where(address => !this.memoryCache.ContainsKey(address)).ToList();
            if (uncached.Count > 0) {
                throw new MemoryNotCachedException(uncached);
            }
            return Enumerable.Range(realAddress + offset, addr.size - offset).Select(address => this.memoryCache[address]).ToArray();
        }

        public int ReadFixedInt(DataAddress addr, int size, int offset = 0) {
            byte[] result = this.ReadBytes(addr, offset);

            int value = 0;
            for (int i = 0; i < Math.Min(result.Length, size); i++) {
                value |= result[i] << (8 * i);
            }
            return value;
        }

        private static int GetRealAddress(DataAddress addr) {
            return addr.bank switch {
                DataBank.Rom => addr.address,
                _ => 0xF50000 + addr.address,
            };
        }
    }
}
