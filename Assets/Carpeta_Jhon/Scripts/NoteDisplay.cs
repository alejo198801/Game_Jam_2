using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameJam2.Puzzles
{
    
    public class NoteDisplay : MonoBehaviour
    {
        [SerializeField] private UIDocument puzzleDocument;
        [SerializeField, Min(0.1f)] private float showSeconds = 3f;

        [Header("Interfaz antigua (solo para ocultarla)")]
        [SerializeField] private GameObject root;

        private VisualElement noteRoot;
        private VisualElement keypadRoot;
        private Label noteContent;
        private Coroutine routine;
        private bool isVisible;

        private void OnEnable()
        {
            if (root != null)
                root.SetActive(false);

            TryBind();
        }

        private void Start()
        {
            if (!TryBind())
                Debug.LogError(
                    "NoteDisplay: asigna Puzzle Document y revisa note-root y note-content en el UXML.",
                    this
                );
        }

        private bool TryBind()
        {
            if (puzzleDocument == null)
                return false;

            VisualElement documentRoot = puzzleDocument.rootVisualElement;
            VisualElement nextRoot = documentRoot.Q("note-root");
            Label nextContent = documentRoot.Q<Label>("note-content");
            if (nextRoot == null || nextContent == null)
                return false;

            if (nextRoot != noteRoot)
            {
                noteRoot = nextRoot;
                noteRoot.style.display = DisplayStyle.None;
              
                noteRoot.pickingMode = PickingMode.Ignore;
                noteRoot.Query<VisualElement>().ForEach(
                    element => element.pickingMode = PickingMode.Ignore
                );
            }

            documentRoot.pickingMode = PickingMode.Ignore;
            noteContent = nextContent;
            noteContent.style.whiteSpace = WhiteSpace.Normal;
            keypadRoot = documentRoot.Q("puzzle-root");
            return true;
        }

        
        public void Show(int index, char digit)
        {
            if (!isActiveAndEnabled)
                return;

            if (!TryBind())
            {
                Debug.LogError("NoteDisplay: no se puede encontrar la interfaz de la nota.", this);
                return;
            }

            if (routine != null)
                StopCoroutine(routine);

            noteContent.text = $"Dígito {index + 1}: {digit}";
            noteRoot.style.display = DisplayStyle.Flex;
            isVisible = true;
            routine = StartCoroutine(HideLater());
        }

        private IEnumerator HideLater()
        {
            
            yield return new WaitForSeconds(showSeconds);
            if (noteRoot != null)
                noteRoot.style.display = DisplayStyle.None;
            isVisible = false;
            routine = null;
        }

        private void Update()
        {
            
            if (isVisible && keypadRoot != null &&
                keypadRoot.resolvedStyle.display == DisplayStyle.Flex)
                Hide();
        }

        public void Hide()
        {
            if (routine != null)
            {
                StopCoroutine(routine);
                routine = null;
            }

            if (noteRoot != null)
                noteRoot.style.display = DisplayStyle.None;
            isVisible = false;
        }

        private void OnDisable()
        {
            Hide();
            noteRoot = null;
            noteContent = null;
            keypadRoot = null;
        }
    }
}
