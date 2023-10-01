using System;
using System.Collections.Generic;
using System.Linq;

namespace ALttPREffectProcessor {
    internal class MemoryNotCachedException : Exception {
        public MemoryNotCachedException(List<int> addresses) : base($"{addresses.Select(address => $"{address:X}").ToList()} not cached") { }
    }
}
