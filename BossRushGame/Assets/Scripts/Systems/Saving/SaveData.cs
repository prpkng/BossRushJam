using System;

namespace Game.Systems.Saving {

    [Serializable]
    public struct SaveData
    {
        public string LastEnteredBoss;
        public string CurrentModifierType;
    }
}