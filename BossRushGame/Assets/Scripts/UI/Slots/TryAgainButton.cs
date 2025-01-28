using System;
using BRJ.Systems.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BRJ.UI.Slots
{
    public class TryAgainButton : MonoBehaviour
    {
        [SerializeField] private Button btn;

        private void FixedUpdate()
        {
            btn.interactable = (bool)SlotButton.CurrentSelectedSlot;
        }

        public void OnClick()
        {
            Game.Instance.World.CurrentActiveModifier = SlotButton.CurrentSelectedSlot.CurrentModifier;
            SaveManager.SetCurrentModifierType(SlotButton.CurrentSelectedSlot.CurrentModifier.GetType());
            Game.Instance.Transition.TransitionToScene("Lobby");
        }

    }
}