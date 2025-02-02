using System;

namespace BRJ.Systems.Saving
{

    [Serializable]
    public struct SaveData
    {
        public string LastEnteredBoss;
        public string CurrentModifierType;

        public bool HaveSeenJokerCutscene;
        public bool HaveSeenSnookerCutscene;

        public bool HasBeatJoker;
        public bool HasBeatSnooker;
    }
}