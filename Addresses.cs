using System;

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
        public static readonly DataAddress Bombs = new(0xF343, 1);
        public static readonly DataAddress Flute = new(0xF34C, 1);
        public static readonly DataAddress Boots = new(0xF355, 1);
        public static readonly DataAddress Sword = new(0xF359, 1);
        public static readonly DataAddress Armor = new(0xF35B, 1);
        public static readonly DataAddress Rupees = new(0xF360, 2);
        public static readonly DataAddress MaxHealth = new(0xF36C, 1);
        public static readonly DataAddress CurrentHealth = new(0xF36D, 1);
        public static readonly DataAddress Magic = new(0xF36E, 1);
        public static readonly DataAddress HealthFiller = new(0xF372, 1);
        public static readonly DataAddress MagicFiller = new(0xF373, 1);
        public static readonly DataAddress Arrows = new(0xF377, 1);
        public static readonly DataAddress Abilities = new(0xF379, 1);
        public static readonly DataAddress MagicUsage = new(0xF37B, 1);
        public static readonly DataAddress Swap1 = new(0xF38C, 1);
        public static readonly DataAddress Timer = new(0xF43E, 4);
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

        public static readonly DataAddress SpecialWeapons = new(0x30802F, 1, DataBank.Rom);
    }

    internal enum DataBank {
        WRAM = 0,
        Rom = 1,
    }

    internal struct DataAddress {
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
