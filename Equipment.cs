using System;
using System.Collections.Generic;

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

    internal class ValueOffsetEquipment : Equipment {
        protected readonly DataAddress addr;
        protected readonly int offset;

        internal ValueOffsetEquipment(TrackingCache cache, DataAddress addr, int offset) : base(cache) {
            this.addr = addr;
            this.offset = offset;
        }

        public override int Value {
            get => cache.ReadBytes(addr)[offset];
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

    public interface IEquipmentMap<T> {
        public IEquipment? this[T key] { get; }
    }

    internal class SmallKeysEquipment : IEquipmentMap<Dungeon> {
        private readonly Dictionary<Dungeon, IEquipment> values = new();

        internal SmallKeysEquipment(TrackingCache cache) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                values[dungeon] = new ValueOffsetEquipment(cache, Addresses.ChestSmallKeys, (int) dungeon);
            }
        }

        public IEquipment? this[Dungeon key] => values[key];
    }

    internal class DungeonItemEquipment : IEquipmentMap<Dungeon> {
        private readonly Dictionary<Dungeon, IEquipment> values = new();

        internal DungeonItemEquipment(TrackingCache cache, DataAddress addr) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                values[dungeon] = new BitmaskEquipment(cache, addr, 1 << (int) dungeon);
            }
        }

        public IEquipment? this[Dungeon key] => values[key];
    }
}
