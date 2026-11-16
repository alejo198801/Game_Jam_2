// PROGRAMADOR A. Uno en la escena, para el puzle 2.
using GameJam2.Core;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Puzzles
{
    public class ObjectPuzzleManager : MonoBehaviour
    {
        [SerializeField] private DropZone[] zones;
        [SerializeField] private Door doorToOpen;

        public UnityEvent<int, int> OnProgress;   // (correctas, total)
        public UnityEvent OnPuzzleSolved;

        private bool solved;

        public void Evaluate()
        {
            if (solved) return;

            int correctCount = 0;
            int total = zones != null ? zones.Length : 0;

            if (zones != null)
            {
                foreach (var zone in zones)
                {
                    if (zone != null && zone.IsSolved)
                    {
                        correctCount++;
                    }
                }
            }

            OnProgress?.Invoke(correctCount, total);

            if (total > 0 && correctCount >= total)
            {
                solved = true;
                OnPuzzleSolved?.Invoke();
                if (doorToOpen != null)
                {
                    doorToOpen.Open();
                }
            }
        }
    }
}
