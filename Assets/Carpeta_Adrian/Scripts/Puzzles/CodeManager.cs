// PROGRAMADOR A. Uno en la escena, para el puzle 1.
using System.Collections.Generic;
using GameJam2.Core;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Puzzles
{
    public class CodeManager : MonoBehaviour
    {
        [Tooltip("Dígitos del código en orden. Cada NoteItem muestra uno.")]
        [SerializeField] private string code = "4721";
        [SerializeField] private Door doorToOpen;

        public UnityEvent<int> OnNoteFound;      // índice de la nota
        public UnityEvent OnCodeCorrect;
        public UnityEvent OnCodeWrong;

        private readonly HashSet<int> foundNotes = new HashSet<int>();

        public int CodeLength => code.Length;
        public int NotesFound => foundNotes.Count;

        public char GetDigit(int index)
        {
            // TODO: Devolver el dígito del código en esa posición.
            return '0';
        }

        public void RegisterNote(int index)
        {
            // TODO: Marcar la nota como encontrada (sin repetir) e invocar OnNoteFound.
        }

        /// Lo llama el teclado al presionar "Aceptar".
        public bool TrySubmit(string input)
        {
            // TODO: Comparar input con el código. Correcto: OnCodeCorrect y door.Open(), devolver true. Incorrecto: OnCodeWrong, devolver false.
            return false;
        }
    }
}
