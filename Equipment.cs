using System;

namespace ALttPREffectProcessor {
    public interface IEquipment {
        public int Value { get; }
    }

    internal abstract class Equipment : IEquipment {
        protected readonly TrackingCache cache;

        protected Equipment(TrackingCache cache) {
            this.cache = cache;
        }

        public abstract int Value { get; }
    }

    internal class BitmaskEquipment : Equipment {
        protected readonly DataAddress addr;
        protected readonly int bitmask;

        internal BitmaskEquipment(TrackingCache cache, DataAddress addr, int bitmask) : base(cache) {
            this.addr = addr;
            this.bitmask = bitmask;
        }

        public override int Value {
            get => ((cache.ReadInt(addr) & bitmask) == bitmask) ? 1 : 0;
        }
    }

    internal class ValueEquipment : Equipment {
        protected readonly DataAddress addr;

        internal ValueEquipment(TrackingCache cache, DataAddress addr) : base(cache) {
            this.addr = addr;
        }

        public override int Value {
            get => cache.ReadInt(addr);
        }
    }

    internal class CustomEquipment : Equipment {
        protected readonly Func<Func<DataAddress, int>, int> func;

        internal CustomEquipment(TrackingCache cache, Func<Func<DataAddress, int>, int> func) : base(cache) {
            this.func = func;
        }

        public override int Value {
            get => func.Invoke(cache.ReadInt);
        }
    }
}
