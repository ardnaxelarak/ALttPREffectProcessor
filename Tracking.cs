using System;
using System.Collections.Generic;

namespace ALttPREffectProcessor {
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

        public IEquipmentMap<Dungeon, int> SmallKeys { get; private set; }
        public IEquipmentMap<Dungeon, int> BigKey { get; private set; }
        public IEquipmentMap<Dungeon, int> Map { get; private set; }
        public IEquipmentMap<Dungeon, int> Compass { get; private set; }
        public IEquipmentMap<Dungeon, int> Bosses { get; private set; }

        public Tracking() {
            Bow = new BitmaskEquipment(cache, Addresses.BowTracking, 0x80);
            SilverArrows = new BitmaskEquipment(cache, Addresses.BowTracking, 0x40);
            BlueBoomerang = new BitmaskEquipment(cache, Addresses.InventoryTracking, 0x80);
            RedBoomerang = new BitmaskEquipment(cache, Addresses.InventoryTracking, 0x40);
            Hookshot = new ValueEquipment(cache, Addresses.Hookshot);
            Bombs = new CustomEquipment(cache, get => (get(Addresses.Bombs) > 0 || get(Addresses.InfiniteBombs) > 0) ? 1 : 0);
            Mushroom = new BitmaskEquipment(cache, Addresses.InventoryTracking, 0x20);
            Powder = new BitmaskEquipment(cache, Addresses.InventoryTracking, 0x10);
            FireRod = new ValueEquipment(cache, Addresses.FireRod);
            IceRod = new ValueEquipment(cache, Addresses.IceRod);
            Bombos = new ValueEquipment(cache, Addresses.Bombos);
            Ether = new ValueEquipment(cache, Addresses.Ether);
            Quake = new ValueEquipment(cache, Addresses.Quake);
            Lamp = new ValueEquipment(cache, Addresses.Lamp);
            Hammer = new ValueEquipment(cache, Addresses.Hammer);
            Shovel = new BitmaskEquipment(cache, Addresses.InventoryTracking, 0x04);
            Flute = new CustomEquipment(cache, get => (get(Addresses.InventoryTracking) & 0x03) switch { 0 => 0, 2 => 1, _ => 2 });
            BugNet = new ValueEquipment(cache, Addresses.BugNet);
            Book = new ValueEquipment(cache, Addresses.Book);
            BottleCount = new CustomEquipment(cache, get => {
                return (get(Addresses.Bottle1) > 0 ? 1 : 0)
                    + (get(Addresses.Bottle2) > 0 ? 1 : 0)
                    + (get(Addresses.Bottle3) > 0 ? 1 : 0)
                    + (get(Addresses.Bottle4) > 0 ? 1 : 0);
            });
            Somaria = new ValueEquipment(cache, Addresses.Somaria);
            Byrna = new ValueEquipment(cache, Addresses.Byrna);
            Cape = new ValueEquipment(cache, Addresses.Cape);
            Mirror = new ValueEquipment(cache, Addresses.Mirror);
            Gloves = new ValueEquipment(cache, Addresses.Gloves);
            Boots = new ValueEquipment(cache, Addresses.Boots);
            Flippers = new ValueEquipment(cache, Addresses.Flippers);
            MoonPearl = new ValueEquipment(cache, Addresses.MoonPearl);
            Sword = new ValueEquipment(cache, Addresses.Sword);
            Shield = new ValueEquipment(cache, Addresses.Shield);
            Armor = new ValueEquipment(cache, Addresses.Armor);
            MagicUsage = new ValueEquipment(cache, Addresses.MagicUsage);

            SmallKeys = new SmallKeysEquipment(cache);
            BigKey = new DungeonItemEquipment(cache, Addresses.BigKey);
            Map = new DungeonItemEquipment(cache, Addresses.Map);
            Compass = new DungeonItemEquipment(cache, Addresses.Compass);
            Bosses = new DungeonBossEquipment(cache);
        }

        internal List<DataAddress> GetReads() {
            return new() {
                Addresses.RoomData,
                Addresses.OverworldData,
                Addresses.SramEquipment,
                Addresses.ProgressFlags,
                Addresses.ProgressIndicator3,
                Addresses.NpcFlags,
                Addresses.ChestSmallKeys,
                Addresses.InfiniteBombs,
            };
        }

        internal void SetReads(Dictionary<DataAddress, byte[]> reads) {
            cache.Update(reads);
            OnReceiveUpdate?.Invoke();
        }

        public byte GetWramByte(int address) {
            return cache.ReadWramByte(address);
        }
    }
}
