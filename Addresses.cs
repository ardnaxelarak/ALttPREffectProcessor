namespace ALttPREffectProcessor {
    internal static class Addresses {
        public static readonly DataAddress GameState = new(0x10, 2);
        public static readonly DataAddress YBBitfield = new(0x3A, 1);
        public static readonly DataAddress CapeFlag = new(0x55, 1);
        public static readonly DataAddress LinkState = new(0x5D, 1);
        public static readonly DataAddress LinkSpeed = new(0x5E, 1);
        public static readonly DataAddress DoorState = new(0x6C, 1);
        public static readonly DataAddress Supertile = new(0xA0, 2);
        public static readonly DataAddress Equipped = new(0x0202, 1);
        public static readonly DataAddress ItemHold = new(0x02E4, 1);
        public static readonly DataAddress YAction = new(0x0303, 1);
        public static readonly DataAddress Tempbunny = new(0x03F5, 2);
        public static readonly DataAddress DungeonId = new(0x040C, 1);
        public static readonly DataAddress World = new(0x0FFF, 1);
        public static readonly DataAddress RoomData = new(0xF000, 0x250);

        public static readonly DataAddress EasternBoss = new(0xF000 + 0xC8 * 2, 2);
        public static readonly DataAddress DesertBoss = new(0xF000 + 0x33 * 2, 2);
        public static readonly DataAddress HeraBoss = new(0xF000 + 0x07 * 2, 2);
        public static readonly DataAddress CastleTowerBoss = new(0xF000 + 0x20 * 2, 2);
        public static readonly DataAddress DarknessBoss = new(0xF000 + 0x5A * 2, 2);
        public static readonly DataAddress SwampBoss = new(0xF000 + 0x06 * 2, 2);
        public static readonly DataAddress SkullBoss = new(0xF000 + 0x29 * 2, 2);
        public static readonly DataAddress ThievesBoss = new(0xF000 + 0xAC * 2, 2);
        public static readonly DataAddress IceBoss = new(0xF000 + 0xDE * 2, 2);
        public static readonly DataAddress MireBoss = new(0xF000 + 0x90 * 2, 2);
        public static readonly DataAddress TurtleBoss = new(0xF000 + 0xA4 * 2, 2);
        public static readonly DataAddress GanonsTowerBoss = new(0xF000 + 0x0D * 2, 2);

        public static readonly DataAddress OverworldData = new(0xF280, 0x82);

        public static readonly DataAddress SramEquipment = new(0xF340, 0x50);
        public static readonly DataAddress Bow = new(0xF340, 1);
        public static readonly DataAddress Boomerang = new(0xF341, 1);
        public static readonly DataAddress Hookshot = new(0xF342, 1);
        public static readonly DataAddress Bombs = new(0xF343, 1);
        public static readonly DataAddress Powder = new(0xF344, 1);
        public static readonly DataAddress FireRod = new(0xF345, 1);
        public static readonly DataAddress IceRod = new(0xF346, 1);
        public static readonly DataAddress Bombos = new(0xF347, 1);
        public static readonly DataAddress Ether = new(0xF348, 1);
        public static readonly DataAddress Quake = new(0xF349, 1);
        public static readonly DataAddress Lamp = new(0xF34A, 1);
        public static readonly DataAddress Hammer = new(0xF34B, 1);
        public static readonly DataAddress Flute = new(0xF34C, 1);
        public static readonly DataAddress BugNet = new(0xF34D, 1);
        public static readonly DataAddress Book = new(0xF34E, 1);
        public static readonly DataAddress BottleIndex = new(0xF34F, 1);
        public static readonly DataAddress Somaria = new(0xF350, 1);
        public static readonly DataAddress Byrna = new(0xF351, 1);
        public static readonly DataAddress Cape = new(0xF352, 1);
        public static readonly DataAddress Mirror = new(0xF353, 1);
        public static readonly DataAddress Gloves = new(0xF354, 1);
        public static readonly DataAddress Boots = new(0xF355, 1);
        public static readonly DataAddress Flippers = new(0xF356, 1);
        public static readonly DataAddress MoonPearl = new(0xF357, 1);
        public static readonly DataAddress Sword = new(0xF359, 1);
        public static readonly DataAddress Shield = new(0xF35A, 1);
        public static readonly DataAddress Armor = new(0xF35B, 1);
        public static readonly DataAddress Bottle1 = new(0xF35C, 1);
        public static readonly DataAddress Bottle2 = new(0xF35D, 1);
        public static readonly DataAddress Bottle3 = new(0xF35E, 1);
        public static readonly DataAddress Bottle4 = new(0xF35F, 1);
        public static readonly DataAddress Rupees = new(0xF360, 2);
        public static readonly DataAddress RupeesTemp = new(0xF362, 2);
        public static readonly DataAddress Compass = new(0xF364, 2);
        public static readonly DataAddress BigKey = new(0xF366, 2);
        public static readonly DataAddress Map = new(0xF368, 2);
        public static readonly DataAddress PiecesOfHeart = new(0xF36B, 1);
        public static readonly DataAddress MaxHealth = new(0xF36C, 1);
        public static readonly DataAddress CurrentHealth = new(0xF36D, 1);
        public static readonly DataAddress Magic = new(0xF36E, 1);
        public static readonly DataAddress SmallKeys = new(0xF36F, 1);
        public static readonly DataAddress BombCapacity = new(0xF370, 1);
        public static readonly DataAddress ArrowCapacity = new(0xF371, 1);
        public static readonly DataAddress HealthFiller = new(0xF372, 1);
        public static readonly DataAddress MagicFiller = new(0xF373, 1);
        public static readonly DataAddress Pendants = new(0xF374, 1);
        public static readonly DataAddress BombFiller = new(0xF375, 1);
        public static readonly DataAddress ArrowFiller = new(0xF376, 1);
        public static readonly DataAddress Arrows = new(0xF377, 1);
        public static readonly DataAddress Abilities = new(0xF379, 1);
        public static readonly DataAddress Crystals = new(0xF37A, 1);
        public static readonly DataAddress MagicUsage = new(0xF37B, 1);
        public static readonly DataAddress DungeonSmallKeys = new(0xF37C, 14);
        public static readonly DataAddress GenericSmallKeys = new(0xF38B, 1);
        public static readonly DataAddress InventoryTracking = new(0xF38C, 2);
        public static readonly DataAddress BowTracking = new(0xF38E, 2);
        public static readonly DataAddress FluteBitfield = new(0xF3C2, 1);
        public static readonly DataAddress SpecialWeaponLevel = new(0xF3C3, 1);
        public static readonly DataAddress ItemOnB = new(0xF3C4, 1);
        public static readonly DataAddress ProgressIndicator = new(0xF3C5, 1);
        public static readonly DataAddress ProgressFlags = new(0xF3C6, 1);
        public static readonly DataAddress MapIcons = new(0xF3C7, 1);
        public static readonly DataAddress ProgressIndicator3 = new(0xF3C9, 1);
        public static readonly DataAddress DR_DungeonCountDisplay = new(0xF403, 2);
        public static readonly DataAddress NpcFlags = new(0xF410, 2);
        public static readonly DataAddress Timer = new(0xF43E, 4);
        public static readonly DataAddress DR_DungeonLocationsChecked = new(0xF4B0, 32);
        public static readonly DataAddress VT_DungeonLocationsChecked = new(0xF4C0, 16);
        public static readonly DataAddress ChestSmallKeys = new(0xF4E0, 16);
        public static readonly DataAddress DR_AllSmallKeys = new(0xF4F1, 13);
        public static readonly DataAddress SwordModifier = new(0x150C0, 1);
        public static readonly DataAddress ArmorModifier = new(0x150C2, 1);
        public static readonly DataAddress MagicModifier = new(0x150C3, 1);
        public static readonly DataAddress CuccoStorm = new(0x150C5, 1);
        public static readonly DataAddress IcePhysics = new(0x150C7, 1);
        public static readonly DataAddress InfiniteArrows = new(0x150C8, 1);
        public static readonly DataAddress InfiniteBombs = new(0x150C9, 1);
        public static readonly DataAddress InfiniteMagic = new(0x150CA, 1);
        public static readonly DataAddress ButtonChaos = new(0x150CB, 1);
        public static readonly DataAddress OneHitKO = new(0x150CC, 1);
        public static readonly DataAddress BootsModifier = new(0x150CE, 1);

        public static readonly DataAddress RomName = new(0x007FC0, 21, DataBank.Rom);
        public static readonly DataAddress SpecialWeapons = new(0x18002F, 1, DataBank.Rom);
        public static readonly DataAddress Pseudoboots = new(0x18008E, 1, DataBank.Rom);
        public static readonly DataAddress DR_DungeonCheckTotals = new(0x187040, 32, DataBank.Rom);
        public static readonly DataAddress DR_ChestKeyTotals = new(0x187010, 16, DataBank.Rom);
    }

    internal enum DataBank {
        WRAM = 0,
        Rom = 1,
    }

    internal readonly struct DataAddress {
        public readonly int address;
        public readonly int size;
        public readonly DataBank bank;

        public DataAddress(int address, int size, DataBank bank = DataBank.WRAM) {
            this.address = address;
            this.size = size;
            this.bank = bank;
        }
    }
}
