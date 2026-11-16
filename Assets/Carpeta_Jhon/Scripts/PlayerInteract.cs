using GameJam2.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace GameJam2.Puzzles
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("Interacción")]
        [SerializeField] private float interactDistance = 2.5f;
        [SerializeField] private LayerMask interactMask = ~0;

        [Header("Interfaz")]
        [SerializeField] private UIDocument puzzleDocument;

        private Label promptLabel;
        private VisualElement noteRoot;
        private IInteractable current;

        private void OnEnable()
        {
            BindPrompt();
            SetPrompt(null);
        }

        private void Start()
        {
            
            if (promptLabel == null)
                BindPrompt();

            if (promptLabel == null)
            {
                Debug.LogError(
                    "PlayerInteract: asigna Puzzle Document y revisa " +
                    "el Name interaction-prompt en PuzzleUI.uxml.",
                    this
                );
            }
        }

        private void BindPrompt()
        {
            if (puzzleDocument == null)
                return;

            VisualElement root = puzzleDocument.rootVisualElement;

            promptLabel = root.Q<Label>("interaction-prompt");
            noteRoot = root.Q("note-root");

            
            root.pickingMode = PickingMode.Ignore;

            if (promptLabel != null)
            {
                promptLabel.pickingMode = PickingMode.Ignore;
                promptLabel.style.display = DisplayStyle.None;
            }
        }

        private void Update()
        {
            
            if (PlayerControlLock.IsLocked || Time.timeScale == 0f)
            {
                current = null;
                SetPrompt(null);
                return;
            }

            Ray ray = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                interactDistance,
                interactMask))
            {
                current = hit.collider.GetComponent<IInteractable>();

                if (current != null)
                {
                    SetPrompt(current);

                    if (Keyboard.current != null &&
                        Keyboard.current.eKey.wasPressedThisFrame)
                    {
                        
                        current.Interact(gameObject);

                        
                        current = null;
                        SetPrompt(null);
                    }

                    return;
                }
            }

            current = null;
            SetPrompt(null);
        }

        private void SetPrompt(IInteractable target)
        {
            if (promptLabel == null)
                return;

            
            bool noteVisible = noteRoot != null &&
                noteRoot.resolvedStyle.display == DisplayStyle.Flex;

            if (target == null || noteVisible)
            {
                promptLabel.style.display = DisplayStyle.None;
                return;
            }

            promptLabel.text = $"[E] {target.InteractPrompt}";
            promptLabel.style.display = DisplayStyle.Flex;
        }

        private void OnDisable()
        {
            current = null;
            SetPrompt(null);
            promptLabel = null;
            noteRoot = null;
        }
    }
}