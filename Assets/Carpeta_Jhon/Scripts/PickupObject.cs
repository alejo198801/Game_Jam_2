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
            rb = GetComponent<Rigidbody>();
        }

        public void Interact(GameObject interactor)
        {
            if (!IsHeld)
            {
                PlayerCarry playerCarry = interactor.GetComponentInChildren<PlayerCarry>();
                if (playerCarry == null)
                    playerCarry = interactor.GetComponent<PlayerCarry>();

                if (playerCarry != null)
                {
                    playerCarry.PickUp(this);
                }
            }
        }

        public void OnPicked(Transform holdPoint, PlayerCarry by)
        {
            IsHeld = true;
            carrier = by;

            if (CurrentZone != null)
            {
                CurrentZone.Release(this);
            }

            rb.isKinematic = true;
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;

            transform.SetParent(holdPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        public void OnDropped()
        {
            IsHeld = false;
            transform.SetParent(null);

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;

            rb.isKinematic = false;

            DropZone.TrySnapNearest(this);
            carrier = null;
        }

        public void SnapTo(Transform anchor)
        {
            rb.isKinematic = true;
            transform.SetParent(null);
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;

            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }
    }
}