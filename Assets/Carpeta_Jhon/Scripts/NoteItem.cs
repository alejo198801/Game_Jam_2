// PROGRAMADOR A. En cada nota escondida (necesita un Collider).
using GameJam2.Core;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Puzzles
{
    public class NoteItem : MonoBehaviour, IInteractable
    {
        [SerializeField] private CodeManager codeManager;
        [Tooltip("Posición del dígito en el código: 0 = primero.")]
        [SerializeField] private int digitIndex;
        [SerializeField] private bool hideAfterRead = false;

        public UnityEvent<int, char> OnNoteRead;

        public string InteractPrompt => "Leer nota";

        public void Interact(GameObject interactor)
        {
            if (codeManager != null)
            {
                codeManager.RegisterNote(digitIndex);
                char digit = codeManager.GetDigit(digitIndex);
                OnNoteRead?.Invoke(digitIndex, digit);
            }

            if (hideAfterRead)
            {
                gameObject.SetActive(false);
            }
        }
    }
}