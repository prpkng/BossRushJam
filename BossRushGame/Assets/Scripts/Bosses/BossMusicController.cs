namespace BRJ.Bosses
{
    using BRJ.Systems;
    using Pixelplacement;
    using UnityEngine;

    public class BossMusicController : MonoBehaviour
    {

        public FMODUnity.StudioEventEmitter eventEmitter;

        private void Awake()
        {
            Game.Instance.Sound.BossMusic = new Maybe<BossMusicController>(this);
        }

        private void Start()
        {
            Game.Instance.Sound.CurrentBGM = eventEmitter.EventInstance;
        }

        // Salvar como uma constante pra evitar typos
        private const string AggressiveParam = "AgressiveAct";

        public void SetAggressive()
        {
            eventEmitter.SetParameter(AggressiveParam, 1);
        }

        public void SetKidding()
        {
            eventEmitter.SetParameter(AggressiveParam, 0);
        }
    }
}