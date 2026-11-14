// COMPARTIDO (dueño: Programador B). A solo llama Open().
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Core
{
    public class Door : MonoBehaviour
    {
        [Header("Estado")]
        [SerializeField] private bool startsLocked = true;

        [Header("Movimiento de la hoja")]
        [SerializeField] private Transform doorLeaf;
        [SerializeField] private Vector3 openOffset = new Vector3(0f, 3f, 0f);
        [SerializeField] private float openDuration = 1f;

        [Header("Eventos para arte y sonido")]
        public UnityEvent OnDoorOpened;
        public UnityEvent OnDoorClosed;
        public UnityEvent OnDoorLockedTry;

        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }

        private Vector3 closedPos;

        private void Awake()
        {
            IsLocked = startsLocked;
            if (doorLeaf != null)
                closedPos = doorLeaf.localPosition;
            else
                closedPos = transform.position;
        }

        /// Desbloquea y abre. Esto es lo que llaman los puzles.
        public void Open()
        {
            if (IsOpen) return;

            IsLocked = false;
            IsOpen = true;

            // ==========================================
            // PRUEBA TEMPORAL PARA TI:
            // Esto hará que el cubo de la puerta desaparezca 
            // al resolverse el puzle para que sepas que funcionó.
            gameObject.SetActive(false);
            // ==========================================

            // (O si prefieres que se mueva en lugar de desaparecer, 
            //  comenta la línea de arriba y descomenta la siguiente):
            // StartCoroutine(MoveLeaf(closedPos + openOffset));

            OnDoorOpened?.Invoke();
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            StartCoroutine(MoveLeaf(closedPos));
            OnDoorClosed?.Invoke();
        }

        public void Lock()
        {
            IsLocked = true;
            OnDoorLockedTry?.Invoke();
        }

        private IEnumerator MoveLeaf(Vector3 target)
        {
            Transform targetTransform = doorLeaf != null ? doorLeaf : transform;
            float elapsed = 0f;
            Vector3 startPos = targetTransform.localPosition;

            while (elapsed < openDuration)
            {
                elapsed += Time.deltaTime;
                targetTransform.localPosition = Vector3.Lerp(startPos, target, elapsed / openDuration);
                yield return null;
            }
            targetTransform.localPosition = target;
        }
    }
}