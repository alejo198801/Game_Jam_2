// PROGRAMADOR A. En cada zona del puzle 2: Collider con "Is Trigger" + un hijo vacío "SnapPoint".
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Puzzles
{
    [RequireComponent(typeof(Collider))]
    public class DropZone : MonoBehaviour
    {
        [Tooltip("ID del PickupObject correcto para esta zona.")]
        [SerializeField] private string expectedObjectId = "Objeto";
        [SerializeField] private Transform snapPoint;
        [SerializeField] private ObjectPuzzleManager manager;

        public UnityEvent OnCorrectPlaced;
        public UnityEvent OnWrongPlaced;

        public bool IsSolved { get; private set; }
        private PickupObject occupant;

        private static readonly List<DropZone> all = new List<DropZone>();
        private readonly HashSet<PickupObject> inside = new HashSet<PickupObject>();

        private void OnEnable()
        {
            if (!all.Contains(this))
                all.Add(this);
        }
        private void OnDisable()
        {
            all.Remove(this);
        }
        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null)
                col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            PickupObject pickup = other.GetComponent<PickupObject>();
            if (pickup != null)
            {
                inside.Add(pickup);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PickupObject pickup = other.GetComponent<PickupObject>();
            if (pickup != null)
            {
                inside.Remove(pickup);
            }
        }

        public static void TrySnapNearest(PickupObject obj)
        {
            foreach (var zone in all)
            {
                if (!zone.IsSolved && zone.inside.Contains(obj))
                {
                    zone.Place(obj);
                    break;
                }
            }
        }

        private void Place(PickupObject obj)
        {
            occupant = obj;
            obj.CurrentZone = this;
            Transform anchor = snapPoint != null ? snapPoint : transform;
            obj.SnapTo(anchor);

            if (obj.ObjectId == expectedObjectId)
            {
                IsSolved = true;
                OnCorrectPlaced?.Invoke();
            }
            else
            {
                IsSolved = false;
                OnWrongPlaced?.Invoke();
            }

            if (manager != null)
            {
                manager.Evaluate();
            }
        }

        public void Release(PickupObject obj)
        {
            if (occupant == obj)
            {
                occupant = null;
                obj.CurrentZone = null;
                IsSolved = false;
                if (manager != null)
                {
                    manager.Evaluate();
                }
            }
        }
    }
}