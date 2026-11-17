using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GameJam2.Core
{
    public class Door : MonoBehaviour
    {
        [Header("Estado")]
        [SerializeField] private bool startsLocked = true;

        [Header("Animación")]
        [SerializeField] private Animator animator;              // el de la Visagra
        [SerializeField] private string openTrigger = "Abrir";   // trigger del controller
        [SerializeField] private string closeTrigger = "";       // vacío: no hay animación de cerrar

        [Header("Bloqueo físico (opcional)")]
        [SerializeField] private Collider blockingCollider;

        [Header("Solo para probar")]
        [SerializeField] private bool debugKeys = true;          // O abre, C cierra

        [Header("Eventos para arte y sonido")]
        public UnityEvent OnDoorOpened;
        public UnityEvent OnDoorClosed;
        public UnityEvent OnDoorLockedTry;

        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }

        private void Awake()
        {
            IsLocked = startsLocked;
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (animator == null) Debug.LogWarning($"{name}: no encuentro Animator", this);
        }

        private void Update()
        {
            if (!debugKeys) return;
            var kb = Keyboard.current;
            if (kb == null) return;
            if (kb.oKey.wasPressedThisFrame) Open();
            if (kb.cKey.wasPressedThisFrame) Close();
        }

        [ContextMenu("Open")]
        public void Open()
        {
            if (IsOpen) return;
            Debug.Log($"{name}: Open()");
            IsLocked = false;
            IsOpen = true;
            if (animator != null) animator.SetTrigger(openTrigger);
            if (blockingCollider != null) blockingCollider.enabled = false;
            OnDoorOpened?.Invoke();
        }

        [ContextMenu("Close")]
        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            if (animator != null && !string.IsNullOrEmpty(closeTrigger)) animator.SetTrigger(closeTrigger);
            if (blockingCollider != null) blockingCollider.enabled = true;
            OnDoorClosed?.Invoke();
        }

        public void Lock() => IsLocked = true;

        public void TryOpenByPlayer()
        {
            if (IsLocked) OnDoorLockedTry?.Invoke();
            else Open();
        }
    }
}