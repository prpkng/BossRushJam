namespace BRJ.UI {
    using Pixelplacement;
    using UnityEngine;
    
    public class HUDManager : MonoBehaviour {
        public DisplayObject bossBar;

        private void Awake() {
            bossBar.SetActive(true);
        }
    }
}