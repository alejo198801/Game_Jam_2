// PROGRAMADOR A. Zona que acepta VARIOS objetos (ej: 3 huevos en el mismo nido).
// Collider con "Is Trigger" + un hijo vacío por cada lugar (SnapPoint_1, _2, _3).
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Puzzles
{
    [RequireComponent(typeof(Collider))]
    public class DropZone : MonoBehaviour
    {
        [Tooltip("ID que deben tener los objetos de esta zona (ej: Huevo).")]
        [SerializeField] private string expectedObjectId = "Huevo";
        [Tooltip("Un punto por cada objeto que cabe. 3 huevos = 3 puntos.")]
        [SerializeField] private Transform[] snapPoints;
        [SerializeField] private ObjectPuzzleManager manager;

        public UnityEvent OnCorrectPlaced;
        public UnityEvent OnWrongPlaced;
        public UnityEvent OnZoneFull;          // cuando están todos

        public bool IsSolved { get; private set; }
        public int CorrectCount { get; private set; }
        public int Capacity => snapPoints.Length;

        private PickupObject[] occupants;

        private static readonly List<DropZone> all = new List<DropZone>();
        private readonly HashSet<PickupObject> inside = new HashSet<PickupObject>();

        private void Awake() => occupants = new PickupObject[snapPoints.Length];
        private void OnEnable() => all.Add(this);
        private void OnDisable() => all.Remove(this);
        private void Reset() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            var p = other.GetComponentInParent<PickupObject>();
            if (p != null) inside.Add(p);
        }

        private void OnTriggerExit(Collider other)
        {
            var p = other.GetComponentInParent<PickupObject>();
            if (p != null) inside.Remove(p);
        }

        /// Al soltar un objeto, la primera zona con lugar libre que lo tenga dentro lo encaja.
        public static void TrySnapNearest(PickupObject obj)
        {
            foreach (var zone in all)
                if (zone.inside.Contains(obj) && zone.TryPlace(obj)) return;
        }

        private bool TryPlace(PickupObject obj)
        {
            int slot = System.Array.IndexOf(occupants, null);
            if (slot < 0) return false;                       // zona llena

            occupants[slot] = obj;
            obj.CurrentZone = this;
            obj.SnapTo(snapPoints[slot]);

            if (obj.ObjectId == expectedObjectId) OnCorrectPlaced?.Invoke();
            else OnWrongPlaced?.Invoke();

            Recount();
            return true;
        }

        /// Lo llama PickupObject cuando lo vuelven a recoger.
        public void Release(PickupObject obj)
        {
            int slot = System.Array.IndexOf(occupants, obj);
            if (slot < 0) return;
            occupants[slot] = null;
            obj.CurrentZone = null;
            Recount();
        }

        private void Recount()
        {
            CorrectCount = 0;
            foreach (var o in occupants)
                if (o != null && o.ObjectId == expectedObjectId) CorrectCount++;

            bool wasSolved = IsSolved;
            IsSolved = CorrectCount == Capacity;
            if (IsSolved && !wasSolved) OnZoneFull?.Invoke();
            manager.Evaluate();
        }
    }
}
