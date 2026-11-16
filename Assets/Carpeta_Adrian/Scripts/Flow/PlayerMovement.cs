using GameJam2.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform cameraPivot;   

        [Header("Movimiento")]
        [SerializeField] private float walkSpeed = 3f;
        [SerializeField] private float sprintSpeed = 5f;     

        [Header("Mouse")]
        [SerializeField] private float mouseSensitivity = 0.1f;
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Gravedad")]
        [SerializeField] private float gravity = -20f;

        public float CurrentSpeed { get; private set; }   

        private CharacterController cc;
        private Animator animator;                       
        private float pitch;
        private float verticalVel;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
            if (animator != null) animator.applyRootMotion = false;
            if (cameraPivot == null && Camera.main != null)
                cameraPivot = Camera.main.transform;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            bool locked = PlayerControlLock.IsLocked;

            if (!locked) Look();

            Vector3 dir = Vector3.zero;
            bool sprinting = false;

            var kb = Keyboard.current;
            if (!locked && kb != null)
            {
                float x = 0f, z = 0f;
                if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
                if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
                if (kb.wKey.isPressed || kb.upArrowKey.isPressed) z += 1f;
                if (kb.sKey.isPressed || kb.downArrowKey.isPressed) z -= 1f;

                sprinting = kb.leftShiftKey.isPressed || kb.rightShiftKey.isPressed;

                
                dir = transform.right * x + transform.forward * z;
                if (dir.sqrMagnitude > 1f) dir.Normalize();
            }

            float speed = sprinting ? sprintSpeed : walkSpeed;

            
            if (cc.isGrounded && verticalVel < 0f) verticalVel = -2f;
            else verticalVel += gravity * Time.deltaTime;

            Vector3 velocity = dir * speed;
            velocity.y = verticalVel;
            cc.Move(velocity * Time.deltaTime);   

            CurrentSpeed = dir.magnitude * speed;

            if (animator != null)
                animator.SetFloat(SpeedHash, dir.magnitude * (sprinting ? 1f : 0.5f), 0.1f, Time.deltaTime);
        }

        private void Look()
        {
            if (Mouse.current == null) return;

            
            Vector2 delta = Mouse.current.delta.ReadValue() * mouseSensitivity
                * MouseSensitivitySettings.Value;

           
            transform.Rotate(Vector3.up * delta.x);

            
            pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
            if (cameraPivot != null)
                cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }
    }
}