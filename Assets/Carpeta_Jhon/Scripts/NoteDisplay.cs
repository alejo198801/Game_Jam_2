// OPCIONAL, para probar el puzle 1: muestra en pantalla el dígito de la nota leída.
// Va en un objeto que SIEMPRE esté activo (por ejemplo el Canvas), no en el panel que se oculta.
using System.Collections;
using TMPro;
using UnityEngine;

namespace GameJam2.Puzzles
{
    public class NoteDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject root;     // panel o texto que se muestra y se oculta
        [SerializeField] private TMP_Text text;
        [SerializeField] private float showSeconds = 3f;

        private Coroutine routine;

        // Se conecta al evento OnNoteRead (int, char) de cada NoteItem
        public void Show(int index, char digit)
        {
            if (text != null) text.text = $"Dígito {index + 1}: {digit}";
            if (root != null) root.SetActive(true);

            if (routine != null) StopCoroutine(routine);
            routine = StartCoroutine(HideLater());
        }

        private IEnumerator HideLater()
        {
            yield return new WaitForSeconds(showSeconds);
            if (root != null) root.SetActive(false);
        }
    }
}