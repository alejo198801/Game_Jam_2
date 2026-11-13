// PROGRAMADOR A. Va en la cámara del jugador.
using GameJam2.Core;
using TMPro;
using UnityEngine;

namespace GameJam2.Puzzles
{
    public class PlayerInteract : MonoBehaviour
    {
        [SerializeField] private float interactDistance = 2.5f;
        [SerializeField] private LayerMask interactMask = ~0;
        [SerializeField] private KeyCode interactKey = KeyCode.E;
        [SerializeField] private TMP_Text promptText;   // "E para ..."; opcional

        private IInteractable current;

        private void Update()
        {
            // TODO: Raycast desde la cámara; si toca un IInteractable mostrar el aviso y con E llamar Interact.
        }

        private void SetPrompt(IInteractable target)
        {
            // TODO: Poner el texto "[E] acción" o vaciarlo si no se mira nada.
        }
    }
}
