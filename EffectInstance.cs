using System;
using System.Threading.Tasks;

namespace ALttPREffectProcessor {
    internal class EffectInstance {
        private readonly Effect effect;
        private readonly EffectData data;

        public EffectInstance(Effect effect, EffectData data) {
            this.effect = effect;
            this.data = data;
            if (this.data.Id.Length == 0) this.data.Id = Guid.NewGuid().ToString();
            data.Status = EffectStatus.Unstarted;
            data.Remaining = TimeSpan.Zero;
            data.EffectTitle = OverlayTitle;
        }

        public static EffectInstance? GetEffect(EffectData data) {
            Effect? effect = Effect.GetEffect(data.EffectCode);
            if (effect is null) {
                return null;
            }
            return new EffectInstance(effect, data);
        }

        public string Code {
            get => effect.Code;
        }

        public string Category {
            get => effect.Category;
        }

        public string PollTitle {
            get => effect.GetPollTitle(data);
        }

        public string OverlayTitle {
            get => effect.GetOverlayTitle(data);
        }

        public EffectStatus Status {
            get => data.Status;
        }

        public EffectData Data {
            get => data;
        }

        public async Task Start(int state) {
            await effect.Start(data, state);
        }

        public async Task Step(int state, TimeSpan elapsed) {
            await effect.Step(data, state, elapsed);
        }

        public async Task Cancel() {
            await effect.Cancel(data);
        }
    }
}
