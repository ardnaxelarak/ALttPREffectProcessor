using System;
using System.Collections.Generic;
using System.Linq;

namespace ALttPREffectProcessor {
    public class MemoryNotCachedException : Exception {
        public MemoryNotCachedException(List<int> addresses) : base($"{string.Join(", ", addresses.Select(address => $"{address:X}").ToList())} not cached") { }
    }
}
