// PROGRAMADOR A. En cada zona del puzle 2: Collider con "Is Trigger" + un hijo vacío "SnapPoint".
// CAMBIO: el encaje se decide por distancia al soltar (no por OnTriggerEnter), porque
// PickupObject apaga el collider mientras se carga y la zona nunca lo registraba.
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
        [Tooltip("Qué tan cerca del collider debe soltarse el objeto para encajar (metros).")]
        [SerializeField] private float snapDistance = 0.5f;

        public UnityEvent OnCorrectPlaced;
        public UnityEvent OnWrongPlaced;

        public bool IsSolved { get; private set; }

        private PickupObject occupant;
        private Collider zoneCol;

        private static readonly List<DropZone> all = new List<DropZone>();

        private void Awake()
        {
            zoneCol = GetComponent<Collider>();
        }

        private void OnEnable()
        {
            if (!all.Contains(this)) all.Add(this);
        }

        private void OnDisable()
        {
            all.Remove(this);
        }

        private void Reset()
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        /// Se llama al soltar un objeto: busca la zona libre más cercana que lo tenga a su alcance.
        public static void TrySnapNearest(PickupObject obj)
        {
            DropZone best = null;
            float bestDist = float.MaxValue;
            Vector3 p = obj.transform.position;

            foreach (var zone in all)
            {
                if (zone.IsSolved || zone.occupant != null) continue;

                // ClosestPoint devuelve el mismo punto si ya está dentro del collider
                Vector3 closest = zone.zoneCol.ClosestPoint(p);
                if ((closest - p).sqrMagnitude > zone.snapDistance * zone.snapDistance) continue;

                float d = (zone.transform.position - p).sqrMagnitude;
                if (d < bestDist)
                {
                    best = zone;
                    bestDist = d;
                }
            }

            if (best != null) best.Place(obj);
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

            if (manager != null) manager.Evaluate();
        }

        public void Release(PickupObject obj)
        {
            if (occupant == obj)
            {
                occupant = null;
                obj.CurrentZone = null;
                IsSolved = false;
                if (manager != null) manager.Evaluate();
            }
        }
    }
}