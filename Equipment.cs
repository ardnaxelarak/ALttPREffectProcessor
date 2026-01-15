using System;
using System.Collections.Generic;

namespace ALttPREffectProcessor {
    public interface IEquipment {
        public int Value { get; }
    }

    public interface IEquipment<T> : IEquipment {
        public new T Value { get; }
        int IEquipment.Value => Convert.ToInt32(Value);
    }

    internal abstract class Equipment<T> : IEquipment<T>  {
        protected readonly TrackingCache cache;

        protected Equipment(TrackingCache cache) {
            this.cache = cache;
        }

        public abstract T Value { get; }
    }

    internal class BitmaskEquipment : Equipment<int> {
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

    internal class ValueEquipment : Equipment<int> {
        protected readonly DataAddress addr;

        internal ValueEquipment(TrackingCache cache, DataAddress addr) : base(cache) {
            this.addr = addr;
        }

        public override int Value {
            get => cache.ReadInt(addr);
        }
    }

    internal class ValueOffsetEquipment : Equipment<int> {
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

    internal class CustomEquipment : Equipment<int> {
        protected readonly Func<Func<DataAddress, int>, int> func;

        internal CustomEquipment(TrackingCache cache, Func<Func<DataAddress, int>, int> func) : base(cache) {
            this.func = func;
        }

        public override int Value {
            get => func.Invoke(cache.ReadInt);
        }
    }

    public interface IEquipmentMap<T, U> {
        public IEquipment<U>? this[T key] { get; }
    }

    internal class SmallKeysEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal SmallKeysEquipment(TrackingCache cache) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                values[dungeon] = new ValueOffsetEquipment(cache, Addresses.ChestSmallKeys, (int) dungeon);
            }
        }

        public IEquipment<int>? this[Dungeon key] => values[key];
    }

    internal class DungeonItemEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal DungeonItemEquipment(TrackingCache cache, DataAddress addr) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                values[dungeon] = new BitmaskEquipment(cache, addr, 1 << (15 - (int) dungeon));
            }
        }

        public IEquipment<int>? this[Dungeon key] => values[key];
    }

    internal class DungeonBossEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal DungeonBossEquipment(TrackingCache cache) {
            values[Dungeon.Sewers] = new CustomEquipment(cache, (_) => 1);
            values[Dungeon.HyruleCastle] = new CustomEquipment(cache, (_) => 1);
            values[Dungeon.EasternPalace] = new BitmaskEquipment(cache, Addresses.EasternBoss, 0x0800);
            values[Dungeon.DesertPalace] = new BitmaskEquipment(cache, Addresses.DesertBoss, 0x0800);
            values[Dungeon.TowerOfHera] = new BitmaskEquipment(cache, Addresses.HeraBoss, 0x0800);
            values[Dungeon.CastleTower] = new BitmaskEquipment(cache, Addresses.CastleTowerBoss, 0x0800);
            values[Dungeon.PalaceOfDarkness] = new BitmaskEquipment(cache, Addresses.DarknessBoss, 0x0800);
            values[Dungeon.SwampPalace] = new BitmaskEquipment(cache, Addresses.SwampBoss, 0x0800);
            values[Dungeon.SkullWoods] = new BitmaskEquipment(cache, Addresses.SkullBoss, 0x0800);
            values[Dungeon.ThievesTown] = new BitmaskEquipment(cache, Addresses.ThievesBoss, 0x0800);
            values[Dungeon.IcePalace] = new BitmaskEquipment(cache, Addresses.IceBoss, 0x0800);
            values[Dungeon.MiseryMire] = new BitmaskEquipment(cache, Addresses.MireBoss, 0x0800);
            values[Dungeon.TurtleRock] = new BitmaskEquipment(cache, Addresses.TurtleBoss, 0x0800);
            values[Dungeon.GanonsTower] = new BitmaskEquipment(cache, Addresses.GanonsTowerBoss, 0x0800);
        }

        public IEquipment<int>? this[Dungeon key] => values[key];
    }
}
