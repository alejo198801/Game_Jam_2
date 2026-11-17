// AMBIENTACIÓN. Cambia la luz GLOBAL (sol, ambiente, niebla, reflejos) al look oscuro/rojo
// cuando entra la oficina corrompida. Conéctalo al evento OnWorldSwapped de ElevatorSequence.
// Va en un objeto vacío siempre activo (por ejemplo "LookManager").
using UnityEngine;
using UnityEngine.Rendering;

namespace GameJam2.Flow
{
    public class CorruptedLook : MonoBehaviour
    {
        [Header("Luz global")]
        [SerializeField] private Light sun;                       // la Directional Light
        [SerializeField] private Color ambientColor = new Color(0.04f, 0.01f, 0.01f);

        [Header("Niebla")]
        [SerializeField] private bool useFog = true;
        [SerializeField] private Color fogColor = new Color(0.12f, 0.02f, 0.02f);
        [SerializeField, Range(0f, 0.2f)] private float fogDensity = 0.04f;

        [Header("Pruebas")]
        [Tooltip("Márcalo para ver el look corrupto al dar Play, sin pasar por el ascensor.")]
        [SerializeField] private bool applyOnStart = false;

        // Valores originales, para poder volver
        private AmbientMode origMode;
        private Color origAmbient;
        private float origReflection;
        private bool origFog;
        private Color origFogColor;
        private float origFogDensity;
        private FogMode origFogMode;

        private void Awake()
        {
            origMode = RenderSettings.ambientMode;
            origAmbient = RenderSettings.ambientLight;
            origReflection = RenderSettings.reflectionIntensity;
            origFog = RenderSettings.fog;
            origFogColor = RenderSettings.fogColor;
            origFogDensity = RenderSettings.fogDensity;
            origFogMode = RenderSettings.fogMode;
        }

        private void Start()
        {
            if (applyOnStart) ApplyCorruptedLook();
        }

        public void ApplyCorruptedLook()
        {
            if (sun != null) sun.enabled = false;

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = ambientColor;
            RenderSettings.reflectionIntensity = 0f;

            RenderSettings.fog = useFog;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
        }

        public void ApplyNormalLook()
        {
            if (sun != null) sun.enabled = true;

            RenderSettings.ambientMode = origMode;
            RenderSettings.ambientLight = origAmbient;
            RenderSettings.reflectionIntensity = origReflection;

            RenderSettings.fog = origFog;
            RenderSettings.fogMode = origFogMode;
            RenderSettings.fogColor = origFogColor;
            RenderSettings.fogDensity = origFogDensity;
        }
    }
}