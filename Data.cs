using System;
using System.Linq;

namespace ALttPREffectProcessor {
    public enum EffectStatus {
        Unknown,
        Unstarted,
        InProgress,
        Finished,
        Failed,
    }

    public class EffectData : ICloneable {
        public string Id { get; set; } = "";
        public string EffectCode { get; set; } = "";
        public string EffectTitle { get; set; } = "";
        public EffectStatus Status { get; set; } = EffectStatus.Unknown;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
        public TimeSpan Remaining { get; set; } = TimeSpan.Zero;
        public int Count { get; set; } = 0;

        object ICloneable.Clone() => Clone();

        public EffectData Clone() {
            EffectData copy = (EffectData) MemberwiseClone();
            return copy;
        }
    }

    internal class MemoryCondition {
        private readonly Func<int, bool> condition;

        public MemoryCondition(Func<int, bool> condition) {
            this.condition = condition;
        }

        public MemoryCondition(params int[] values) {
            condition = v => values.Contains(v);
        }

        public bool IsValid(int value) {
            return condition(value);
        }
    }

    internal class MemoryUpdate {
        private readonly Func<int, Func<DataAddress, int>, int?> update;

        public MemoryUpdate(Func<int, Func<DataAddress, int>, int> update) {
            this.update = (x, y) => update(x, y);
        }

        public MemoryUpdate(Func<int, int> update) {
            this.update = (x, _) => update(x);
        }

        public MemoryUpdate(int value) {
            update = (_, _) => value;
        }

        public MemoryUpdate() {
            update = (_, _) => null;
        }

        public int? NewValue(int value, Func<DataAddress, int> func) {
            return update(value, func);
        }
    }
}
