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
            // TODO: Sumar un bloqueo; si showCursor, liberar y mostrar el cursor.
        }

        public static void Unlock()
        {
            // TODO: Quitar un bloqueo; si ya no queda ninguno, volver a bloquear el cursor.
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void ResetAll() => lockCount = 0;
    }
}
