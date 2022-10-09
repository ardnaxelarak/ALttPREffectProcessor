using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#pragma warning disable IDE0052 // Remove unread private members -- ignored because our attribute processor will pick them up
namespace ALttPREffectProcessor {
    [ALttPREffect("ice_physics", "Ice Physics", category: "movement_chaos")]
    internal sealed class IcePhysics : Effect {
        private static readonly List<int> invalidSupertiles = new() { 0x0091, 0x0092, 0x0093 };

        [CanStart, CanStep]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Supertile] = new(x => !invalidSupertiles.Contains(x)),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.IcePhysics] = new(0x10),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.IcePhysics] = new(0x00),
        };
    }

    [ALttPREffect("no_dashing", "No Dashing", category: "movement_chaos")]
    internal sealed class NoDashing : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Boots] = new(x => x > 0),
            [Addresses.Abilities] = new(x => (x & 0x04) > 0),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.BootsModifier] = new(0x02),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.BootsModifier] = new(0x00),
        };
    }

    [ALttPREffect("super_speed", "Super Speed", category: "movement_chaos"), NoCheckDoorState]
    internal sealed class SuperSpeed : Effect {
        [ProcessStart, ProcessStep]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.LinkSpeed] = new(0x10),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.LinkSpeed] = new(0x00),
        };
    }

    [ALttPREffect("invisibility", "Invisibility")]
    internal sealed class Invisible : Effect {
        [ProcessStart, ProcessStep]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.CapeFlag] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.CapeFlag] = new(0x00),
        };
    }

    [ALttPREffect("ohko", "One-Hit KO")]
    internal sealed class OneHitKO : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.OneHitKO] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.OneHitKO] = new(0x00),
        };
    }

    [ALttPREffect("cucco_storm", "Cucco Storm")]
    internal sealed class CuccoStorm : Effect {
        [CanStart, CanStep]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.GameState] = new(x => (x & 0xFF) == 0x09),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.CuccoStorm] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.CuccoStorm] = new(0x00),
        };
    }

    [ALttPREffect("deactivate_flute", "Deactivate Flute", instantaneous: true), NoCheckDoorState]
    internal sealed class DeactivateFlute : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Swap1] = new(x => (x & 0x01) > 0),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.Swap1] = new(x => (x & ~0x03) | 0x02),
            [Addresses.Flute] = new(0x02),
        };
    }

    [ALttPREffect("equip_mirror", "Equip Mirror", instantaneous: true)]
    internal sealed class EquipMirror : Effect {
        [FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.YAction] = new(0x14),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.YAction] = new(0x14),
        };
    }

    [ALttPREffect("kill_player", "Kill Player", instantaneous: true)]
    internal sealed class KillPlayer : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.GameState] = new(0x0012),
        };
    }

    [ALttPREffect("force_mirror", "Force Mirror", instantaneous: true)]
    internal sealed class ForceMirror : Effect {
        [FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> inDungeonCondition = new() {
            [Addresses.GameState] = new(x => (x & 0xFF) == 0x07),
            [Addresses.DungeonId] = new(x => x <= 0x1A),
        };

        [CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.ItemHold] = new(0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.GameState] = new(0x1907),
        };
    }

    [ALttPREffect("force_flute", "Force Flute", instantaneous: true)]
    internal sealed class ForceFlute : Effect {
        [FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> inDungeonCondition = new() {
            [Addresses.GameState] = new(x => (x & 0xFF) == 0x09),
            [Addresses.World] = new(0x00),
        };

        [CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.ItemHold] = new(0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.GameState] = new(0x0A0E),
        };
    }

    [ALttPREffect("use_bombos", "Use Bombos", instantaneous: true)]
    internal sealed class UseBombos : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.LinkState] = new(0x09),
        };
    }

    [ALttPREffect("use_ether", "Use Ether", instantaneous: true)]
    internal sealed class UseEther : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.LinkState] = new(0x08),
        };
    }

    [ALttPREffect("use_quake", "Use Quake", instantaneous: true)]
    internal sealed class UseQuake : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.LinkState] = new(0x0A),
        };
    }

    [ALttPREffect("bunnify", "Bunnify", instantaneous: true), NoCheckLinkState]
    internal sealed class Bunnify : Effect {
        [CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.LinkState] = new(0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.Tempbunny] = new(0x0500),
        };
    }

    [ALttPREffect("add_containers", "Add Containers", instantaneous: true), NoCheckDoorState]
    internal sealed class AddContainers : Effect {
        protected override string GetTitleBase(EffectData data) {
            if (data.Count > 1) {
                return $"Add {data.Count} Containers";
            } else {
                return $"Add Container";
            }
        }

        private async Task<bool> ValidEndState(EffectData data) {
            if (data.Count < 1) return false;
            return await SnesController.Instance.CheckMemory(new() {
                [Addresses.MaxHealth] = new(x => x + data.Count * 8 <= 20 * 8),
            });
        }

        public override async Task<bool> CanOffer(EffectData data) {
            if (!await base.CanOffer(data)) return false;
            return await ValidEndState(data);
        }

        protected override async Task<bool> FailStartUnless(EffectData data) {
            if (!await base.FailStartUnless(data)) return false;
            return await ValidEndState(data);
        }

        protected override async Task ProcessStart(EffectData data) {
            await base.ProcessStart(data);
            await SnesController.Instance.UpdateMemory(new() {
                [Addresses.MaxHealth] = new(x => Math.Min(x + data.Count * 8, 20 * 8)),
                [Addresses.CurrentHealth] = new(x => Math.Min(x + data.Count * 8, 20 * 8)),
            });
        }
    }

    [ALttPREffect("remove_containers", "Remove Containers", instantaneous: true), NoCheckDoorState]
    internal sealed class RemoveContainers : Effect {
        protected override string GetTitleBase(EffectData data) {
            if (data.Count > 1) {
                return $"Remove {data.Count} Containers";
            } else {
                return $"Remove Container";
            }
        }

        private async Task<bool> ValidEndState(EffectData data) {
            if (data.Count < 1) return false;
            return await SnesController.Instance.CheckMemory(new() {
                [Addresses.MaxHealth] = new(x => x - data.Count * 8 >= 3 * 8),
            });
        }

        public override async Task<bool> CanOffer(EffectData data) {
            if (!await base.CanOffer(data)) return false;
            return await ValidEndState(data);
        }

        protected override async Task<bool> FailStartUnless(EffectData data) {
            if (!await base.FailStartUnless(data)) return false;
            return await ValidEndState(data);
        }

        private int NewMaxHealth(int CurrentMax, EffectData data) {
            return Math.Max(CurrentMax - data.Count * 8, 3 * 8);
        }

        protected override async Task ProcessStart(EffectData data) {
            await base.ProcessStart(data);
            await SnesController.Instance.UpdateMemory(new() {
                [Addresses.MaxHealth] = new(x => NewMaxHealth(x, data)),
                [Addresses.CurrentHealth] = new((x, other) => Math.Min(x, NewMaxHealth(other(Addresses.MaxHealth), data))),
            });
        }
    }

    [ALttPREffect("drain_bombs", "Drain Bombs", instantaneous: true), NoCheckLinkState, NoCheckDoorState]
    internal sealed class DrainBombs : Effect {
        [CanOffer, CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Bombs] = new(x => x > 0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.Bombs] = new(0x00),
        };
    }

    [ALttPREffect("drain_arrows", "Drain Arrows", instantaneous: true), NoCheckLinkState, NoCheckDoorState]
    internal sealed class DrainArrows : Effect {
        [CanOffer, CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Arrows] = new(x => x > 0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.Arrows] = new(0x00),
        };
    }

    [ALttPREffect("drain_magic", "Drain Magic", instantaneous: true), NoCheckLinkState, NoCheckDoorState]
    internal sealed class DrainMagic : Effect {
        [CanOffer, CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Magic] = new(x => x > 0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.Magic] = new(0x00),
        };
    }

    [ALttPREffect("drain_rupees", "Drain Rupees", instantaneous: true), NoCheckLinkState, NoCheckDoorState]
    internal sealed class DrainRupees : Effect {
        [CanOffer, CanStart]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Rupees] = new(x => x > 0x0000),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.Rupees] = new(0x0000),
        };
    }

    [ALttPREffect("buff_sword", "Buff Sword", category: "sword_modifier")]
    internal sealed class BuffSword : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Sword] = new(x => x > 0x00 && x < 0x04),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.SwordModifier] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.SwordModifier] = new(0x00),
        };
    }

    [ALttPREffect("debuff_sword", "Debuff Sword", category: "sword_modifier")]
    internal sealed class DebuffSword : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Sword] = new(x => x > 0x01 && x <= 0x04),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.SwordModifier] = new(0xFD),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.SwordModifier] = new(0x00),
        };
    }

    [ALttPREffect("debuff_armor", "Debuff Armor")]
    internal sealed class DebuffArmor : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.Armor] = new(x => x > 0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.ArmorModifier] = new(0xFE),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.ArmorModifier] = new(0x00),
        };
    }

    [ALttPREffect("debuff_magic", "Debuff Magic")]
    internal sealed class DebuffMagic : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.MagicUsage] = new(x => x > 0x00),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.MagicModifier] = new(0xFE),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.MagicModifier] = new(0x00),
        };
    }

    [ALttPREffect("infinite_bombs", "Infinite Bombs")]
    internal sealed class InfiniteBombs : Effect {
        [CanOffer, FailStartUnless]
        private static readonly Dictionary<DataAddress, MemoryCondition> conditions = new() {
            [Addresses.SpecialWeapons] = new(x => x != 0x01),
        };

        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.InfiniteBombs] = new(0x01),
        };

        [ProcessEnd, ProcessCancel]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.InfiniteBombs] = new(0x00),
        };

        protected override async Task ProcessClear() {
            if (await SnesController.Instance.CheckMemory(conditions)) {
                await SnesController.Instance.UpdateMemory(turnOff);
            }
        }
    }

    [ALttPREffect("infinite_arrows", "Infinite Arrows")]
    internal sealed class InfiniteArrows : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.InfiniteArrows] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.InfiniteArrows] = new(0x00),
        };
    }

    [ALttPREffect("infinite_magic", "Infinite Magic")]
    internal sealed class InfiniteMagic : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.InfiniteMagic] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.InfiniteMagic] = new(0x00),
        };
    }

    [ALttPREffect("full_restore", "Full Restore", instantaneous: true)]
    internal sealed class FullRestore : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> apply = new() {
            [Addresses.HealthFiller] = new(0xA0),
            [Addresses.MagicFiller] = new(0x80),
        };
    }

    [ALttPREffect("invert_dpad", "Invert D-Pad", category: "button_chaos")]
    internal sealed class InvertDPad : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.ButtonChaos] = new(0x01),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.ButtonChaos] = new(0x00),
        };
    }

    [ALttPREffect("invert_buttons", "Invert Buttons", category: "button_chaos")]
    internal sealed class InvertButtons : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.ButtonChaos] = new(0x02),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.ButtonChaos] = new(0x00),
        };
    }

    [ALttPREffect("invert_buttons_and_dpad", "Invert Controller", category: "button_chaos")]
    internal sealed class InvertButtonsAndDPad : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.ButtonChaos] = new(0x03),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.ButtonChaos] = new(0x00),
        };
    }

    [ALttPREffect("swap_buttons_and_dpad", "Swap Buttons/DPad", category: "button_chaos")]
    internal sealed class SwapButtonsAndDPad : Effect {
        [ProcessStart]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOn = new() {
            [Addresses.ButtonChaos] = new(0x04),
        };

        [ProcessEnd, ProcessCancel, ProcessClear]
        private static readonly Dictionary<DataAddress, MemoryUpdate> turnOff = new() {
            [Addresses.ButtonChaos] = new(0x00),
        };
    }
}
