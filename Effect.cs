using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ALttPREffectProcessor {
    public abstract class Effect {
        internal static readonly Dictionary<DataAddress, MemoryCondition> checkLinkState = new() {
            [Addresses.LinkState] = new(0x00, 0x17, 0x1C),
        };
        internal static readonly Dictionary<DataAddress, MemoryCondition> checkDoorState = new() {
            [Addresses.DoorState] = new(0x00),
        };

        private static Dictionary<string, Effect> Effects = LoadEffectList();

        private readonly Dictionary<DataAddress, MemoryCondition> canOffer = new();
        private readonly Dictionary<DataAddress, MemoryCondition> failStartUnless = new();
        private readonly Dictionary<DataAddress, MemoryCondition> canStart = new();
        private readonly Dictionary<DataAddress, MemoryUpdate> processStart = new();
        private readonly Dictionary<DataAddress, MemoryCondition> canStep = new();
        private readonly Dictionary<DataAddress, MemoryUpdate> processStep = new();
        private readonly Dictionary<DataAddress, MemoryUpdate> processEnd = new();
        private readonly Dictionary<DataAddress, MemoryUpdate> processCancel = new();
        private readonly Dictionary<DataAddress, MemoryUpdate> processClear = new();
        private readonly string defaultTitle;
        private readonly string effectCode;
        private readonly string category;
        private readonly bool instantaneous = false;

        public Effect() {
            if (Attribute.GetCustomAttribute(GetType(), typeof(NoCheckLinkState)) is null) {
                checkLinkState.ToList().ForEach(condition => {
                    canStart.Add(condition.Key, condition.Value);
                    canStep.Add(condition.Key, condition.Value);
                });
            }

            if (Attribute.GetCustomAttribute(GetType(), typeof(NoCheckDoorState)) is null) {
                checkDoorState.ToList().ForEach(condition => {
                    canStart.Add(condition.Key, condition.Value);
                    canStep.Add(condition.Key, condition.Value);
                });
            }

            FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(CanOffer), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryCondition> dict) {
                    dict.ToList().ForEach(condition => { canOffer.Add(condition.Key, condition.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryCondition>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(FailStartUnless), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryCondition> dict) {
                    dict.ToList().ForEach(condition => { failStartUnless.Add(condition.Key, condition.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryCondition>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(CanStart), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryCondition> dict) {
                    dict.ToList().ForEach(condition => { canStart.Add(condition.Key, condition.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryCondition>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(ProcessStart), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryUpdate> dict) {
                    dict.ToList().ForEach(update => { processStart.Add(update.Key, update.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryUpdate>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(CanStep), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryCondition> dict) {
                    dict.ToList().ForEach(condition => { canStep.Add(condition.Key, condition.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryCondition>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(ProcessStep), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryUpdate> dict) {
                    dict.ToList().ForEach(update => { processStep.Add(update.Key, update.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryUpdate>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(ProcessEnd), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryUpdate> dict) {
                    dict.ToList().ForEach(update => { processEnd.Add(update.Key, update.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryUpdate>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(ProcessCancel), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryUpdate> dict) {
                    dict.ToList().ForEach(update => { processCancel.Add(update.Key, update.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryUpdate>");
                }
            }
            foreach (FieldInfo field in fields.Where(field => field.IsDefined(typeof(ProcessClear), true))) {
                object? fieldValue = field.GetValue(this);
                if (fieldValue is Dictionary<DataAddress, MemoryUpdate> dict) {
                    dict.ToList().ForEach(update => { processClear.Add(update.Key, update.Value); });
                } else if (fieldValue is not null) {
                    throw new InvalidCastException($"{fieldValue.GetType()} cannot be cast to Dictionary<DataAddress, MemoryUpdate>");
                }
            }

            Attribute? att = Attribute.GetCustomAttribute(GetType(), typeof(ALttPREffect));
            if (att is ALttPREffect effectAtt) {
                defaultTitle = effectAtt.TitleBase;
                effectCode = effectAtt.Name;
                category = effectAtt.Category ?? effectCode;
                instantaneous = effectAtt.Instantaneous;
            } else {
                defaultTitle = GetType().Name;
                effectCode = "";
                category = "";
            }
        }

        public string Code {
            get => effectCode;
        }

        public string Category {
            get => category;
        }

        public bool Instantaneous {
            get => instantaneous;
        }

        protected virtual string GetTitleBase(EffectData data) {
            return defaultTitle;
        }

        public virtual string GetPollTitle(EffectData data) {
            if (data.Duration > TimeSpan.Zero) {
                return $"{GetTitleBase(data)} ({(int) data.Duration.TotalSeconds}s)";
            } else {
                return GetTitleBase(data);
            }
        }

        public virtual string GetOverlayTitle(EffectData data) {
            return GetTitleBase(data);
        }

        public virtual async Task<bool> CanOffer(EffectData data) {
            return await SnesController.Instance.CheckMemory(canOffer);
        }

        protected virtual async Task<bool> CanStart(EffectData data) {
            return await SnesController.Instance.CheckMemory(canStart);
        }

        protected virtual async Task<bool> FailStartUnless(EffectData data) {
            return await SnesController.Instance.CheckMemory(failStartUnless);
        }

        protected virtual async Task ProcessStart(EffectData data) {
            await SnesController.Instance.UpdateMemory(processStart);
        }

        protected virtual async Task<bool> CanStep(EffectData data) {
            return await SnesController.Instance.CheckMemory(canStep);
        }

        protected virtual async Task ProcessStep(EffectData data) {
            await SnesController.Instance.UpdateMemory(processStep);
        }

        protected virtual async Task ProcessEnd(EffectData data) {
            await SnesController.Instance.UpdateMemory(processEnd);
        }

        protected virtual async Task ProcessCancel(EffectData data) {
            await SnesController.Instance.UpdateMemory(processCancel);
        }

        protected virtual async Task ProcessClear() {
            await SnesController.Instance.UpdateMemory(processClear);
        }

        public async Task Start(EffectData data, int state) {
            if (GameAlreadyWon(state)) {
                data.Status = EffectStatus.Failed;
                return;
            }

            if (!CheckGameState(state)) {
                return;
            }

            if (!await FailStartUnless(data)) {
                data.Status = EffectStatus.Failed;
                return;
            }

            if (!await CanStart(data)) {
                return;
            }

            await ProcessStart(data);

            data.Remaining = data.Duration;
            if (data.Remaining <= TimeSpan.Zero) {
                await ProcessEnd(data);
                data.Status = EffectStatus.Finished;
                return;
            }

            data.Status = EffectStatus.InProgress;
        }

        public async Task Step(EffectData data, int state, TimeSpan elapsed) {
            if (GameAlreadyWon(state)) {
                data.Status = EffectStatus.Failed;
                return;
            }

            if (!CheckGameState(state)) {
                return;
            }

            if (!await CanStep(data)) {
                return;
            }

            await ProcessStep(data);

            data.Remaining -= elapsed;
            if (data.Remaining <= TimeSpan.Zero) {
                await ProcessEnd(data);
                data.Status = EffectStatus.Finished;
                return;
            }
        }

        public async Task Cancel(EffectData data) {
            if (data.Status == EffectStatus.InProgress) {
                await ProcessCancel(data);
                data.Status = EffectStatus.Failed;
            } else if (data.Status == EffectStatus.Unstarted) {
                data.Status = EffectStatus.Failed;
            }
        }

        public async Task Clear() {
            await ProcessClear();
        }

        private static Dictionary<string, Effect> LoadEffectList() {
            Dictionary<string, Effect> result = new();

            foreach (Type effectType in typeof(Effect).Assembly.GetTypes()) {
                ALttPREffect? att = effectType.GetCustomAttribute<ALttPREffect>(false);
                if (att is null || att.Name.Length == 0) continue;
                ConstructorInfo? ctor = effectType.GetConstructor(new Type[] { });
                if (ctor is null) {
                    Console.Error.WriteLine($"Error finding constructor for {effectType.Name}.");
                    continue;
                }
                if (ctor.Invoke(new object[] { }) is Effect effect) {
                    result.Add(att.Name, effect);
                } else {
                    Console.Error.WriteLine($"Error constructing {effectType.Name}.");
                    continue;
                }
            }

            return result;
        }

        public static List<Effect> GetAllEffects() {
            return Effects.Values.ToList();
        }

        public static Effect GetEffect(string code) {
            return Effects[code];
        }

        public static bool HasEffect(string code) {
            return Effects.ContainsKey(code);
        }

        private static bool GameAlreadyWon(int state) {
            int mainState = state & 0xFF;
            return mainState == 0x19 || mainState == 0x1a;
        }

        private static bool CheckGameState(int state) {
            int mainState = state & 0xFF;
            return mainState == 0x07 || mainState == 0x09 || mainState == 0x0b;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class ALttPREffect : Attribute {
        public string Name { get; private set; }
        public string TitleBase { get; private set; }
        public string? Category { get; private set; }
        public bool Instantaneous { get; private set; }

        public ALttPREffect(string name, string titleBase, string? category = null, bool instantaneous = false) {
            Name = name;
            TitleBase = titleBase;
            Category = category;
            Instantaneous = instantaneous;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal class NoCheckLinkState : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    internal class NoCheckDoorState : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class CanOffer : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class FailStartUnless : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class CanStart : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class ProcessStart : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class CanStep : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class ProcessStep : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class ProcessEnd : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class ProcessCancel : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal class ProcessClear : Attribute { }
}
