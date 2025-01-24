namespace BRJ
{
    using System;
    using BRJ.Bosses;
    using BRJ.Systems;
    using FMOD.Studio;
    using UnityEngine;



    public class SoundManager : MonoBehaviour
    {
        public Maybe<BossMusicController> BossMusic;

        public EventInstance? CurrentBGM = null;

        public const string DamageAttenuationParam = "DamageAttenuation";
        public const string PauseAttenuationParam = "PauseAttenuation";

        public void SetGlobalParameter(string param, int value)
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName(param, value);
        }
    }
}