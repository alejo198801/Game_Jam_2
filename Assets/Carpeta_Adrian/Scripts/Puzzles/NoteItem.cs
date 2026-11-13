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

        /// El UI de la nota se suscribe aquí: recibe la posición y el dígito.
        public UnityEvent<int, char> OnNoteRead;

        public string InteractPrompt => "Leer nota";

        public void Interact(GameObject interactor)
        {
            // TODO: Registrar la nota en el CodeManager e invocar OnNoteRead con su posición y dígito. Ocultarla si hideAfterRead.
        }
    }
}
