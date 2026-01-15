using System;
using System.Collections.Generic;
using System.Linq;

namespace ALttPREffectProcessor {
    internal class TrackingCache {
        private readonly Dictionary<int, byte> memoryCache = new();

        public TrackingCache() { }

        public void Update(Dictionary<DataAddress, byte[]> values) {
            memoryCache.Clear();
            foreach (KeyValuePair<DataAddress, byte[]> kvp in values) {
                int realAddress = GetRealAddress(kvp.Key);
                for (int i = 0; i < kvp.Value.Length; i++) {
                    memoryCache[realAddress + i] = kvp.Value[i];
                }
            }
        }

        public byte ReadWramByte(int addr) {
            int realAddress = 0xF50000 + addr;
            if (memoryCache.TryGetValue(realAddress, out byte result)) {
                return result;
            } else {
                throw new MemoryNotCachedException(new() { realAddress });
            }
        }

        public byte[] ReadBytes(DataAddress addr) {
            int realAddress = GetRealAddress(addr);
            List<int> uncached = Enumerable.Range(realAddress, addr.size).Where(address => !memoryCache.ContainsKey(address)).ToList();
            if (uncached.Count > 0) {
                throw new MemoryNotCachedException(uncached);
            }
            return Enumerable.Range(realAddress, addr.size).Select(address => memoryCache[address]).ToArray();
        }

        public int ReadInt(DataAddress addr) {
            byte[] result = ReadBytes(addr);

            int value = 0;
            for (int i = 0; i < Math.Min(result.Length, 4); i++) {
                value |= result[i] << (8 * i);
            }
            return value;
        }

        public long ReadLong(DataAddress addr) {
            byte[] result = ReadBytes(addr);

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
