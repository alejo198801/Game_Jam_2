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
            // TODO: Agregar esta zona a la lista estática all.
        }
        private void OnDisable()
        {
            // TODO: Quitar esta zona de la lista all.
        }
        private void Reset()
        {
            // TODO: Marcar el Collider como trigger.
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Si entra un PickupObject, agregarlo a inside.
        }

        private void OnTriggerExit(Collider other)
        {
            // TODO: Si sale un PickupObject, quitarlo de inside.
        }

        /// Al soltar un objeto, la primera zona libre que lo tenga dentro lo encaja.
        public static void TrySnapNearest(PickupObject obj)
        {
            // TODO: Buscar la primera zona libre que tenga obj dentro y llamar Place(obj).
        }

        private void Place(PickupObject obj)
        {
            // TODO: Guardar el ocupante, encajarlo en snapPoint, ver si su ID es el esperado, invocar el evento y avisar al manager.
        }

        /// Lo llama PickupObject cuando lo vuelven a recoger.
        public void Release(PickupObject obj)
        {
            // TODO: Si obj es el ocupante, vaciar la zona, IsSolved = false y avisar al manager.
        }
    }
}
