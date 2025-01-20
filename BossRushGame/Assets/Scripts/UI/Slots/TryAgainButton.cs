using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI.Slots
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
            GameManager.CurrentActiveModifier = SlotButton.CurrentSelectedSlot.CurrentModifier;
            SceneManager.LoadScene("Lobby");
        }

    }
}