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

    internal sealed class ConstantEquipment : IEquipment<int> {
        private readonly int value;

        internal ConstantEquipment(int value) {
            this.value = value;
        }

        public int Value => this.value;
    }

    internal class BitmaskEquipment : Equipment<int> {
        protected readonly DataAddress addr;
        protected readonly int bitmask;
        protected readonly int size;

        internal BitmaskEquipment(TrackingCache cache, DataAddress addr, int size, int bitmask) : base(cache) {
            this.addr = addr;
            this.bitmask = bitmask;
            this.size = size;
        }

        public override int Value {
            get => ((this.cache.ReadFixedInt(addr, size) & bitmask) == bitmask) ? 1 : 0;
        }
    }

    internal class ValueEquipment : Equipment<int> {
        protected readonly DataAddress addr;
        protected readonly int size;

        internal ValueEquipment(TrackingCache cache, DataAddress addr, int size) : base(cache) {
            this.addr = addr;
            this.size = size;
        }

        public override int Value {
            get => this.cache.ReadFixedInt(this.addr, this.size);
        }
    }

    internal class ValueOffsetEquipment : Equipment<int> {
        protected readonly DataAddress addr;
        protected readonly int offset;
        protected readonly int size;

        internal ValueOffsetEquipment(TrackingCache cache, DataAddress addr, int size, int offset) : base(cache) {
            this.addr = addr;
            this.size = size;
            this.offset = offset;
        }

        public override int Value {
            get => this.cache.ReadFixedInt(this.addr, this.size, this.offset);
        }
    }

    internal class CustomEquipment : Equipment<int> {
        protected readonly Func<Func<DataAddress, int, int, int>, int> func;

        internal CustomEquipment(TrackingCache cache, Func<Func<DataAddress, int, int, int>, int> func) : base(cache) {
            this.func = func;
        }

        public override int Value {
            get => func.Invoke(this.cache.ReadFixedInt);
        }
    }

    public interface IEquipmentMap<T, U> {
        public IEquipment<U>? this[T key] { get; }
    }

    internal class DungeonValueEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal DungeonValueEquipment(TrackingCache cache, DataAddress addr, int size) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                this.values[dungeon] = new ValueOffsetEquipment(cache, addr, size, size * (int) dungeon);
            }
        }

        public IEquipment<int>? this[Dungeon key] => this.values[key];
    }

    internal class VersionedDungeonValueEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal VersionedDungeonValueEquipment(TrackingCache cache, DataAddress vt_addr, int vt_size, DataAddress dr_addr, int dr_size) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                this.values[dungeon] = new CustomEquipment(cache, get => {
                    var version = get(Addresses.RomName, 2, 0);
                    if (version == 0x5456 /* VT */ || version == 0x5245 /* ER */) {
                        return get(vt_addr, vt_size, vt_size * (int) dungeon);
                    } else {
                        return get(dr_addr, dr_size, dr_size * (int) dungeon);
                    }
                });
            }
        }

        public IEquipment<int>? this[Dungeon key] => this.values[key];
    }

    internal class DungeonNoSewersEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal DungeonNoSewersEquipment(TrackingCache cache, DataAddress addr, int size) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                var dungeonID = (int) (dungeon switch { Dungeon.Sewers => Dungeon.HyruleCastle, _ => dungeon });
                this.values[dungeon] = new ValueOffsetEquipment(cache, addr, size, size * (dungeonID - 1));
            }
        }

        public IEquipment<int>? this[Dungeon key] => this.values[key];
    }

    internal class DungeonBitmaskEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal DungeonBitmaskEquipment(TrackingCache cache, DataAddress addr) {
            foreach (Dungeon dungeon in Enum.GetValues(typeof(Dungeon))) {
                this.values[dungeon] = new BitmaskEquipment(cache, addr, 2, 1 << (15 - (int) dungeon));
            }
        }

        public IEquipment<int>? this[Dungeon key] => this.values[key];
    }

    internal class DungeonBossEquipment : IEquipmentMap<Dungeon, int> {
        private readonly Dictionary<Dungeon, IEquipment<int>> values = new();

        internal DungeonBossEquipment(TrackingCache cache) {
            this.values[Dungeon.Sewers] = new ConstantEquipment(0);
            this.values[Dungeon.HyruleCastle] = new ConstantEquipment(0);
            this.values[Dungeon.EasternPalace] = new BitmaskEquipment(cache, Addresses.EasternBoss, 2, 0x0800);
            this.values[Dungeon.DesertPalace] = new BitmaskEquipment(cache, Addresses.DesertBoss, 2, 0x0800);
            this.values[Dungeon.TowerOfHera] = new BitmaskEquipment(cache, Addresses.HeraBoss, 2, 0x0800);
            this.values[Dungeon.CastleTower] = new BitmaskEquipment(cache, Addresses.CastleTowerBoss, 2, 0x0800);
            this.values[Dungeon.PalaceOfDarkness] = new BitmaskEquipment(cache, Addresses.DarknessBoss, 2, 0x0800);
            this.values[Dungeon.SwampPalace] = new BitmaskEquipment(cache, Addresses.SwampBoss, 2, 0x0800);
            this.values[Dungeon.SkullWoods] = new BitmaskEquipment(cache, Addresses.SkullBoss, 2, 0x0800);
            this.values[Dungeon.ThievesTown] = new BitmaskEquipment(cache, Addresses.ThievesBoss, 2, 0x0800);
            this.values[Dungeon.IcePalace] = new BitmaskEquipment(cache, Addresses.IceBoss, 2, 0x0800);
            this.values[Dungeon.MiseryMire] = new BitmaskEquipment(cache, Addresses.MireBoss, 2, 0x0800);
            this.values[Dungeon.TurtleRock] = new BitmaskEquipment(cache, Addresses.TurtleBoss, 2, 0x0800);
            this.values[Dungeon.GanonsTower] = new BitmaskEquipment(cache, Addresses.GanonsTowerBoss, 2, 0x0800);
        }

        public IEquipment<int>? this[Dungeon key] => this.values[key];
    }
}
