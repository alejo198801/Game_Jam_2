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
            // TODO: Contar zonas resueltas, invocar OnProgress; si están todas, OnPuzzleSolved y door.Open() una sola vez.
        }
    }
}
