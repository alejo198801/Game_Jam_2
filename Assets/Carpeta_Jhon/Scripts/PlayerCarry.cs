// PROGRAMADOR A. En la cámara del jugador, junto a PlayerInteract.
using GameJam2.Core;
using UnityEngine;

namespace GameJam2.Puzzles
{
    public class PlayerCarry : MonoBehaviour
    {
        [Tooltip("Hijo vacío de la cámara, ~1 m al frente.")]
        [SerializeField] private Transform holdPoint;
        [SerializeField] private KeyCode dropKey = KeyCode.E;

        public PickupObject Held { get; private set; }
        private int pickedFrame;
        private int droppedFrame = -1;

        public void PickUp(PickupObject obj)
        {
            // Evita recoger si ya tiene un objeto, si acaba de soltar en este frame o si el jugador está bloqueado
            if (Held == null && Time.frameCount != droppedFrame && !PlayerControlLock.IsLocked)
            {
                Held = obj;
                pickedFrame = Time.frameCount;
                Held.OnPicked(holdPoint, this);
            }
        }

        private void Update()
        {
            // Si el control del jugador está bloqueado (ej. usando el teclado o en el ascensor), no puede soltar el objeto
            if (PlayerControlLock.IsLocked) return;

            if (Held != null && Input.GetKeyDown(dropKey) && Time.frameCount != pickedFrame)
            {
                PickupObject tmp = Held;
                Held = null;
                droppedFrame = Time.frameCount;
                tmp.OnDropped();
            }
        }
    }
}
