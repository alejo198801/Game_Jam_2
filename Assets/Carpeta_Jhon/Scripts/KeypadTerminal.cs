// PROGRAMADOR A. El objeto del teclado en la pared (con Collider); abre el KeypadPanel.
using GameJam2.Core;
using UnityEngine;

namespace GameJam2.Puzzles
{
    public class KeypadTerminal : MonoBehaviour, IInteractable
    {
        [SerializeField] private KeypadPanel panel;
        public string InteractPrompt => "Usar teclado";
        public void Interact(GameObject interactor)
        {
            if (panel != null)
            {
                panel.Show();
            }
        }
    }
}