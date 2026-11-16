using System;
using UnityEngine;

namespace GameJam2.Core
{
    // Ajuste compartido entre Inicio, Pausa y las escenas con PlayerMovement.
    public static class MouseSensitivitySettings
    {
        public const float MinValue = 0.1f;
        public const float MaxValue = 3f;
        private const string PreferenceKey = "Controls.MouseSensitivity";

        private static bool loaded;
        private static float value = 1f;
        public static event Action<float> Changed;

        public static float Value
        {
            get
            {
                if (!loaded)
                {
                    value = Mathf.Clamp(
                        PlayerPrefs.GetFloat(PreferenceKey, 1f), MinValue, MaxValue
                    );
                    loaded = true;
                }
                return value;
            }
            set
            {
                float clamped = Mathf.Clamp(value, MinValue, MaxValue);
                if (Mathf.Approximately(Value, clamped))
                    return;

                MouseSensitivitySettings.value = clamped;
                PlayerPrefs.SetFloat(PreferenceKey, clamped);
                // AudioSettingsUI guarda PlayerPrefs al desactivarse o cerrar el juego.
                Changed?.Invoke(clamped);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            loaded = false;
            value = 1f;
            Changed = null;
        }
    }
}
