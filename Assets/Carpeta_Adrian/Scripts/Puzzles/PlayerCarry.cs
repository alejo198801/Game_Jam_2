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
            // TODO: Si no carga nada (y no soltó en este frame), guardar el objeto y llamar obj.OnPicked.
        }

        private void Update()
        {
            // TODO: Si carga algo y presiona E en otro frame distinto al de recoger, soltarlo con OnDropped.
        }
    }
}
