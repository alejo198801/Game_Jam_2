using GameJam2.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam2.Puzzles
{
    public class PlayerCarry : MonoBehaviour
    {
        [Tooltip("Hijo vacío de la cámara, aproximadamente 1 m al frente.")]
        [SerializeField] private Transform holdPoint;

        public PickupObject Held { get; private set; }

        private int pickedFrame = -1;
        private int droppedFrame = -1;

        public void PickUp(PickupObject obj)
        {
            if (obj == null ||
                Held != null ||
                Time.frameCount == droppedFrame ||
                PlayerControlLock.IsLocked ||
                Time.timeScale == 0f)
            {
                return;
            }

            if (holdPoint == null)
            {
                Debug.LogError(
                    "Asigna Hold Point en PlayerCarry.",
                    this
                );
                return;
            }

            Held = obj;
            pickedFrame = Time.frameCount;

            Held.OnPicked(holdPoint, this);
        }

        private void Update()
        {
            
            if (PlayerControlLock.IsLocked || Time.timeScale == 0f)
                return;

            if (Held == null || Keyboard.current == null)
                return;

            
            if (Keyboard.current.eKey.wasPressedThisFrame &&
                Time.frameCount != pickedFrame)
            {
                PickupObject objectToDrop = Held;

                Held = null;
                droppedFrame = Time.frameCount;

                objectToDrop.OnDropped();
            }
        }
    }
}