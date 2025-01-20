namespace BRJ.Bosses {
    using Pixelplacement;

    public class BossMusicController : Singleton<BossMusicController> {

        public FMODUnity.StudioEventEmitter eventEmitter;

        // Salvar como uma constante pra evitar typos
        private const string AggressiveParam = "AgressiveAct";

        public void SetAggressive() {
            eventEmitter.SetParameter(AggressiveParam, 1);
        }

        public void SetKidding() {
            eventEmitter.SetParameter(AggressiveParam, 0);
        }
    }
}