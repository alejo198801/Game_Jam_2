using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movimiento")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float runSpeed = 5.5f;   // araña ~4.5; ajusta jugando

        [Header("Mouse")]
        [SerializeField] private Transform cameraTransform;   // la cámara (hija del Player)
        [SerializeField] private float mouseSensitivity = 0.1f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Gravedad")]
        [SerializeField] private float gravity = -20f;

        /// Velocidad horizontal actual (m/s).
        public float CurrentSpeed { get; private set; }

        /// Lo usan el ascensor y el teclado (luego conéctalo a PlayerControlLock).
        public bool InputLocked { get; set; }

        private CharacterController controller;
        private Animator animator;
        private float verticalVelocity;
        private float pitch;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            Look();
            Move();
        }

        private void Look()
        {
            if (InputLocked || Mouse.current == null) return;

            Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity;

            // Yaw: gira el cuerpo entero (izquierda/derecha)
            transform.Rotate(Vector3.up * delta.x);

            // Pitch: solo la cámara (arriba/abajo), limitado
            pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
            if (cameraTransform != null)
                cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        private void Move()
        {
            var kb = Keyboard.current;
            float x = 0f, z = 0f;
            bool isRunning = false;

            if (kb != null && !InputLocked)
            {
                isRunning = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  x -= 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    z += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  z -= 1f;
            }

            // Relativo a hacia dónde mira el jugador: W = adelante, S = atrás
            Vector3 dir = (transform.right * x + transform.forward * z);
            if (dir.sqrMagnitude > 1f) dir.Normalize();

            float speed = isRunning ? runSpeed : walkSpeed;

            // Gravedad acumulada
            if (controller.isGrounded && verticalVelocity < 0f)
                verticalVelocity = -2f;
            else
                verticalVelocity += gravity * Time.deltaTime;

            Vector3 velocity = dir * speed;
            velocity.y = verticalVelocity;
            controller.Move(velocity * Time.deltaTime);

            CurrentSpeed = dir.magnitude * speed;

            if (animator != null)
            {
                float anim = dir.magnitude * (isRunning ? 1f : 0.5f);
                animator.SetFloat(SpeedHash, anim, 0.1f, Time.deltaTime);
            }
        }
    }
}