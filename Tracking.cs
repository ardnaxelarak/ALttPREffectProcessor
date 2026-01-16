namespace ALttPREffectProcessor {
    using System;
    using System.Collections.Generic;

    public class Tracking {
        private readonly TrackingCache cache = new();

        public event Action? OnReceiveUpdate;

        public IEquipment Bow { get; private set; }
        public IEquipment SilverArrows { get; private set; }
        public IEquipment BlueBoomerang { get; private set; }
        public IEquipment RedBoomerang { get; private set; }
        public IEquipment Hookshot { get; private set; }
        public IEquipment Bombs { get; private set; }
        public IEquipment Mushroom { get; private set; }
        public IEquipment Powder { get; private set; }
        public IEquipment FireRod { get; private set; }
        public IEquipment IceRod { get; private set; }
        public IEquipment Bombos { get; private set; }
        public IEquipment Ether { get; private set; }
        public IEquipment Quake { get; private set; }
        public IEquipment Lamp { get; private set; }
        public IEquipment Hammer { get; private set; }
        public IEquipment Shovel { get; private set; }
        public IEquipment Flute { get; private set; }
        public IEquipment BugNet { get; private set; }
        public IEquipment Book { get; private set; }
        public IEquipment BottleCount { get; private set; }
        public IEquipment Somaria { get; private set; }
        public IEquipment Byrna { get; private set; }
        public IEquipment Cape { get; private set; }
        public IEquipment Mirror { get; private set; }
        public IEquipment Gloves { get; private set; }
        public IEquipment Boots { get; private set; }
        public IEquipment Flippers { get; private set; }
        public IEquipment MoonPearl { get; private set; }
        public IEquipment Sword { get; private set; }
        public IEquipment Shield { get; private set; }
        public IEquipment Armor { get; private set; }
        public IEquipment MagicUsage { get; private set; }

        public IEquipmentMap<Dungeon, int> ChestSmallKeys { get; private set; }
        public IEquipmentMap<Dungeon, int> AllSmallKeys { get; private set; }
        public IEquipmentMap<Dungeon, int> LocationsChecked { get; private set; }
        public IEquipmentMap<Dungeon, int> BigKey { get; private set; }
        public IEquipmentMap<Dungeon, int> Map { get; private set; }
        public IEquipmentMap<Dungeon, int> Compass { get; private set; }
        public IEquipmentMap<Dungeon, int> Bosses { get; private set; }

        public IEquipmentMap<Dungeon, int> CheckTotals { get; private set; }
        public IEquipmentMap<Dungeon, int> ChestKeyTotals { get; private set; }

        public Tracking() {
            Bow = new BitmaskEquipment(cache, Addresses.BowTracking, 1, 0x80);
            SilverArrows = new BitmaskEquipment(cache, Addresses.BowTracking, 1, 0x40);
            BlueBoomerang = new BitmaskEquipment(cache, Addresses.InventoryTracking, 1, 0x80);
            RedBoomerang = new BitmaskEquipment(cache, Addresses.InventoryTracking, 1, 0x40);
            Hookshot = new ValueEquipment(cache, Addresses.Hookshot, 1);
            Bombs = new CustomEquipment(cache, get => (get(Addresses.Bombs, 1, 0) > 0 || get(Addresses.InfiniteBombs, 1, 0) > 0) ? 1 : 0);
            Mushroom = new BitmaskEquipment(cache, Addresses.InventoryTracking, 1, 0x20);
            Powder = new BitmaskEquipment(cache, Addresses.InventoryTracking, 1, 0x10);
            FireRod = new ValueEquipment(cache, Addresses.FireRod, 1);
            IceRod = new ValueEquipment(cache, Addresses.IceRod, 1);
            Bombos = new ValueEquipment(cache, Addresses.Bombos, 1);
            Ether = new ValueEquipment(cache, Addresses.Ether, 1);
            Quake = new ValueEquipment(cache, Addresses.Quake, 1);
            Lamp = new ValueEquipment(cache, Addresses.Lamp, 1);
            Hammer = new ValueEquipment(cache, Addresses.Hammer, 1);
            Shovel = new BitmaskEquipment(cache, Addresses.InventoryTracking, 1, 0x04);
            Flute = new CustomEquipment(cache, get => {
                return (get(Addresses.InventoryTracking, 1, 0) & 0x03) switch {
                    0 => 0,
                    2 => 2,
                    _ => get(Addresses.FluteBitfield, 1, 0) == 0 ? 3 : 1,
                };
            });
            BugNet = new ValueEquipment(cache, Addresses.BugNet, 1);
            Book = new ValueEquipment(cache, Addresses.Book, 1);
            BottleCount = new CustomEquipment(cache, get => {
                return (get(Addresses.Bottle1, 1, 0) > 0 ? 1 : 0)
                    + (get(Addresses.Bottle2, 1, 0) > 0 ? 1 : 0)
                    + (get(Addresses.Bottle3, 1, 0) > 0 ? 1 : 0)
                    + (get(Addresses.Bottle4, 1, 0) > 0 ? 1 : 0);
            });
            Somaria = new ValueEquipment(cache, Addresses.Somaria, 1);
            Byrna = new ValueEquipment(cache, Addresses.Byrna, 1);
            Cape = new ValueEquipment(cache, Addresses.Cape, 1);
            Mirror = new ValueEquipment(cache, Addresses.Mirror, 1);
            Gloves = new ValueEquipment(cache, Addresses.Gloves, 1);
            Boots = new CustomEquipment(cache, get => {
                return get(Addresses.Boots, 1, 0) switch {
                    0 => get(Addresses.Pseudoboots, 1, 0) > 0 ? 1 : 0,
                    _ => 2,
                };
            });
            Flippers = new ValueEquipment(cache, Addresses.Flippers, 1);
            MoonPearl = new ValueEquipment(cache, Addresses.MoonPearl, 1);
            Sword = new ValueEquipment(cache, Addresses.Sword, 1);
            Shield = new ValueEquipment(cache, Addresses.Shield, 1);
            Armor = new ValueEquipment(cache, Addresses.Armor, 1);
            MagicUsage = new ValueEquipment(cache, Addresses.MagicUsage, 1);

            ChestSmallKeys = new DungeonValueEquipment(cache, Addresses.ChestSmallKeys, 1);
            AllSmallKeys = new DungeonNoSewersEquipment(cache, Addresses.AllSmallKeys, 1);
            LocationsChecked = new DungeonValueEquipment(cache, Addresses.DungeonLocationsChecked, 2);

            BigKey = new DungeonBitmaskEquipment(cache, Addresses.BigKey);
            Map = new DungeonBitmaskEquipment(cache, Addresses.Map);
            Compass = new DungeonBitmaskEquipment(cache, Addresses.Compass);
            Bosses = new DungeonBossEquipment(cache);

            CheckTotals = new DungeonValueEquipment(cache, Addresses.DungeonCheckTotals, 2);
            ChestKeyTotals = new DungeonValueEquipment(cache, Addresses.ChestKeyTotals, 1);
        }

        internal List<DataAddress> GetReads() {
            return new() {
                Addresses.RoomData,
                Addresses.OverworldData,
                Addresses.SramEquipment,
                Addresses.FluteBitfield,
                Addresses.ProgressFlags,
                Addresses.ProgressIndicator3,
                Addresses.NpcFlags,
                Addresses.ChestSmallKeys,
                Addresses.DungeonLocationsChecked,
                Addresses.InfiniteBombs,
                Addresses.AllSmallKeys,
                Addresses.Pseudoboots,
                Addresses.DungeonCheckTotals,
                Addresses.ChestKeyTotals,
            };
        }

        internal void SetReads(Dictionary<DataAddress, byte[]> reads) {
            this.cache.Update(reads);
            this.OnReceiveUpdate?.Invoke();
        }

        public byte GetWramByte(int address) {
            return this.cache.ReadWramByte(address);
        }
    }
}
