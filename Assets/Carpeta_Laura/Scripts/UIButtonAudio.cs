using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument), typeof(AudioSource))]
public class UIButtonAudio : MonoBehaviour
{
    [Header("Sonido de botones")]
    [SerializeField] private AudioClip clickSound;

    [Header("Filtro opcional")]
    [Tooltip("Vacío: todos los botones. Si tiene un nombre, solo los de ese contenedor.")]
    [SerializeField] private string containerName;

    private VisualElement root;
    private AudioSource audioSettings;

    
    private static AudioSource sharedAudio;

    private void OnEnable()
    {
        audioSettings = GetComponent<AudioSource>();
        root = GetComponent<UIDocument>().rootVisualElement;

       
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
        if (evt.button != 0)
            return;

        VisualElement element = evt.target as VisualElement;

        while (element != null && element != root)
        {
            if (element is Button button)
            {
                if (!button.enabledInHierarchy)
                    return;

               
                if (!string.IsNullOrWhiteSpace(containerName))
                {
                    VisualElement parent = button.parent;
                    bool insideContainer = false;

                    while (parent != null)
                    {
                        if (parent.name == containerName)
                        {
                            insideContainer = true;
                            break;
                        }

                        parent = parent.parent;
                    }

                    if (!insideContainer)
                        return;
                }

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

            
            sharedAudio.ignoreListenerPause = true;
        }

        
        sharedAudio.volume = audioSettings.volume;
        sharedAudio.outputAudioMixerGroup =
            audioSettings.outputAudioMixerGroup;

        sharedAudio.PlayOneShot(clickSound);
    }
}