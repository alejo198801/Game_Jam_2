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
            if (index >= 0 && index < code.Length)
                return code[index];
            return '0';
        }

        public void RegisterNote(int index)
        {
            if (foundNotes.Add(index))
            {
                OnNoteFound?.Invoke(index);
            }
        }

        public bool TrySubmit(string input)
        {
            if (input == code)
            {
                OnCodeCorrect?.Invoke();
                if (doorToOpen != null)
                    doorToOpen.Open();
                return true;
            }
            else
            {
                OnCodeWrong?.Invoke();
                return false;
            }
        }
    }
}