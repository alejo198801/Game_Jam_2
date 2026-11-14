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
            // NUEVO: Si el jugador está bloqueado (ej. en el ascensor o usando el teclado), no puede interactuar con nada
            if (PlayerControlLock.IsLocked)
            {
                current = null;
                SetPrompt(null);
                return;
            }

            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask))
            {
                current = hit.collider.GetComponent<IInteractable>();
                if (current != null)
                {
                    SetPrompt(current);
                    if (Input.GetKeyDown(interactKey))
                    {
                        current.Interact(gameObject);
                    }
                    return;
                }
            }
            current = null;
            SetPrompt(null);
        }

        private void SetPrompt(IInteractable target)
        {
            if (promptText != null)
            {
                if (target != null)
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = $"[E] {target.InteractPrompt}";
                }
                else
                {
                    promptText.gameObject.SetActive(false);
                }
            }
        }
    }
}