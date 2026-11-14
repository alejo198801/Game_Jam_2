// COMPARTIDO (dueño: Programador B). A solo llama Open().
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GameJam2.Core
{
    public class Door : MonoBehaviour
    {
        [Header("Estado")]
        [SerializeField] private bool startsLocked = true;

        [Header("Movimiento de la hoja")]
        [SerializeField] private Transform doorLeaf;      // la Bisagra
        [SerializeField] private float openAngle = 90f;   // usa -90 si abre hacia el otro lado
        [SerializeField] private float openDuration = 1f;

        [Header("Eventos para arte y sonido")]
        public UnityEvent OnDoorOpened;
        public UnityEvent OnDoorClosed;
        public UnityEvent OnDoorLockedTry;

        public bool IsLocked { get; private set; }
        public bool IsOpen { get; private set; }

        private Quaternion closedRot;
        private Quaternion openRot;

        private void Awake()
        {
            IsLocked = startsLocked;
            if (doorLeaf == null) doorLeaf = transform;
            closedRot = doorLeaf.localRotation;
            openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
        }

        // SOLO PARA PROBAR: borra este Update cuando los puzles ya llamen Open()
       

        /// Desbloquea y abre. Esto es lo que llaman los puzles.
        public void Open()
        {
            if (IsOpen) return;
            IsLocked = false;
            IsOpen = true;
            StopAllCoroutines();
            StartCoroutine(MoveLeaf(openRot));
            OnDoorOpened?.Invoke();
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            StopAllCoroutines();
            StartCoroutine(MoveLeaf(closedRot));
            OnDoorClosed?.Invoke();
        }

        public void Lock()
        {
            IsLocked = true;
        }

        private IEnumerator MoveLeaf(Quaternion target)
        {
            Quaternion start = doorLeaf.localRotation;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / openDuration;
                doorLeaf.localRotation = Quaternion.Slerp(start, target, t);
                yield return null;
            }
            doorLeaf.localRotation = target; // asegura que termine exacto
        }
    }
}