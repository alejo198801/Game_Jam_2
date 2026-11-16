using UnityEngine;
using UnityEngine.InputSystem;

namespace MimicSpace
{
    /// <summary>
    /// Movimiento básico del Mimic adaptado al nuevo Input System.
    /// Si lo reemplazas, actualiza siempre myMimic.velocity con un Vector3(x, 0, z).
    /// </summary>
    public class Movement : MonoBehaviour
    {
        [Header("Controls")]
        [Tooltip("Body Height from ground")]
        [Range(0.5f, 5f)]
        public float height = 0.8f;
        public float speed = 5f;
        Vector3 velocity = Vector3.zero;
        public float velocityLerpCoef = 4f;
        Mimic myMimic;

        private void Start()
        {
            myMimic = GetComponent<Mimic>();
        }

        void Update()
        {
            // Equivalente a Input.GetAxisRaw("Horizontal") y ("Vertical")
            Vector3 input = ReadInput();

            velocity = Vector3.Lerp(velocity, input.normalized * speed, velocityLerpCoef * Time.deltaTime);

            // Assigning velocity to the mimic to assure great leg placement
            myMimic.velocity = velocity;

            transform.position = transform.position + velocity * Time.deltaTime;
            RaycastHit hit;
            Vector3 destHeight = transform.position;
            if (Physics.Raycast(transform.position + Vector3.up * 5f, -Vector3.up, out hit))
                destHeight = new Vector3(transform.position.x, hit.point.y + height, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, destHeight, velocityLerpCoef * Time.deltaTime);
        }

        private Vector3 ReadInput()
        {
            var kb = Keyboard.current;
            if (kb == null) return Vector3.zero;   // por si no hay teclado conectado

            float x = 0f, z = 0f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  x -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    z += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  z -= 1f;

            return new Vector3(x, 0f, z);
        }
    }
}