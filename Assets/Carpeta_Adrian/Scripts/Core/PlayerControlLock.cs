// COMPARTIDO: forma única de congelar al jugador (ascensor, teclado, muerte).
using UnityEngine;

namespace GameJam2.Core
{
    public static class PlayerControlLock
    {
        private static int lockCount;
        public static bool IsLocked => lockCount > 0;

        /// Cada Lock() necesita su Unlock(). Así el teclado y el ascensor no se pisan.
        public static void Lock(bool showCursor = false)
        {
            lockCount++;
            if (showCursor) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        }

        public static void Unlock()
        {
            lockCount = Mathf.Max(0, lockCount - 1);
            if (lockCount == 0) { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetAll() => lockCount = 0;
    }
}