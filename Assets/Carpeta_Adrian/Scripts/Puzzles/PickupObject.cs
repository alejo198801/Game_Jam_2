// PROGRAMADOR A. En cada objeto movible del puzle 2 (Rigidbody + Collider, tag "Pickup").
using GameJam2.Core;
using UnityEngine;

namespace GameJam2.Puzzles
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickupObject : MonoBehaviour, IInteractable
    {
        [Tooltip("ID que la DropZone espera, por ejemplo \"Lampara\". Debe coincidir exactamente.")]
        [SerializeField] private string objectId = "Objeto";

        public string ObjectId => objectId;
        public bool IsHeld { get; private set; }
        public DropZone CurrentZone { get; set; }   // la llena DropZone

        public string InteractPrompt => IsHeld ? "Soltar" : "Recoger";

        private Rigidbody rb;
        private PlayerCarry carrier;

        private void Awake()
        {
            // TODO: Obtener el Rigidbody.
        }

        public void Interact(GameObject interactor)
        {
            // TODO: Si no está cargado, buscar PlayerCarry en el interactor y llamar PickUp(this).
        }

        /// Lo llama PlayerCarry.
        public void OnPicked(Transform holdPoint, PlayerCarry by)
        {
            // TODO: Liberarse de su zona, volverse kinematic y pegarse al holdPoint.
        }

        /// Lo llama PlayerCarry. Si está sobre una zona, la zona lo encaja.
        public void OnDropped()
        {
            // TODO: Soltarse del holdPoint, volver a la física y pedir a DropZone.TrySnapNearest que lo encaje.
        }

        /// Lo llama DropZone al encajarlo.
        public void SnapTo(Transform anchor)
        {
            // TODO: Volverse kinematic y copiar posición y rotación del anchor.
        }
    }
}
