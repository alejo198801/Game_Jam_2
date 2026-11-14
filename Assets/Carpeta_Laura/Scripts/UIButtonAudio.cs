using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument), typeof(AudioSource))]
public class UIButtonAudio : MonoBehaviour
{
    [Header("Sonido de botones")]
    [SerializeField] private AudioClip clickSound;

    private VisualElement root;
    private AudioSource audioSettings;

    // Un reproductor compartido que continúa al cambiar de escena.
    private static AudioSource sharedAudio;

    private void OnEnable()
    {
        audioSettings = GetComponent<AudioSource>();
        root = GetComponent<UIDocument>().rootVisualElement;

        // Escucha la pulsación antes de que el botón cambie de pantalla.
        root.RegisterCallback<PointerDownEvent>(
            OnPointerDown,
            TrickleDown.TrickleDown
        );
    }

    private void OnDisable()
    {
        if (root != null)
        {
            root.UnregisterCallback<PointerDownEvent>(
                OnPointerDown,
                TrickleDown.TrickleDown
            );
        }
    }

    private void OnPointerDown(PointerDownEvent evt)
    {
        // Solo responde al botón izquierdo del mouse.
        if (evt.button != 0)
            return;

        // Busca si lo pulsado es un botón o parte de uno.
        VisualElement element = evt.target as VisualElement;

        while (element != null && element != root)
        {
            if (element is Button button)
            {
                if (button.enabledInHierarchy)
                    PlayClick();

                return;
            }

            element = element.parent;
        }
    }

    private void PlayClick()
    {
        if (clickSound == null)
            return;

        if (sharedAudio == null)
        {
            GameObject audioObject = new GameObject("SharedUIButtonAudio");
            DontDestroyOnLoad(audioObject);

            sharedAudio = audioObject.AddComponent<AudioSource>();
            sharedAudio.playOnAwake = false;
            sharedAudio.loop = false;
            sharedAudio.spatialBlend = 0f;

            // Permite sonidos de interfaz si luego pausamos el audio del juego.
            sharedAudio.ignoreListenerPause = true;
        }

        // Usa el volumen y la salida configurados en este menú.
        sharedAudio.volume = audioSettings.volume;
        sharedAudio.outputAudioMixerGroup =
            audioSettings.outputAudioMixerGroup;

        sharedAudio.PlayOneShot(clickSound);
    }
}