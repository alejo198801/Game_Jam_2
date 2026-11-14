using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer mixer;

    private Slider masterSlider;
    private Slider musicSlider;
    private Slider sfxSlider;

    private bool applyOnNextUpdate;

    private const string MasterKey = "Audio.Master";
    private const string MusicKey = "Audio.Music";
    private const string SFXKey = "Audio.SFX";

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        masterSlider = root.Q<Slider>("master-volume-slider");
        musicSlider = root.Q<Slider>("music-volume-slider");
        sfxSlider = root.Q<Slider>("sfx-volume-slider");

        if (mixer == null || masterSlider == null ||
            musicSlider == null || sfxSlider == null)
        {
            Debug.LogError(
                "Asigna el mixer y revisa los Name de los sliders.",
                this
            );
            return;
        }

        // Los tres controles usan una escala de 0 a 100.
        PrepareSlider(masterSlider, MasterKey);
        PrepareSlider(musicSlider, MusicKey);
        PrepareSlider(sfxSlider, SFXKey);

        masterSlider.RegisterValueChangedCallback(OnMasterChanged);
        musicSlider.RegisterValueChangedCallback(OnMusicChanged);
        sfxSlider.RegisterValueChangedCallback(OnSFXChanged);

        // Aplica el mixer después de la inicialización de Unity.
        applyOnNextUpdate = true;
    }

    private void Update()
    {
        if (!applyOnNextUpdate)
            return;

        applyOnNextUpdate = false;

        ApplyVolume("MasterVolume", masterSlider.value);
        ApplyVolume("MusicVolume", musicSlider.value);
        ApplyEffects(sfxSlider.value);
    }

    private void PrepareSlider(Slider slider, string key)
    {
        slider.lowValue = 0f;
        slider.highValue = 100f;

        float savedValue = PlayerPrefs.GetFloat(key, 80f);
        slider.SetValueWithoutNotify(Mathf.Clamp(savedValue, 0f, 100f));
    }

    private void OnMasterChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(MasterKey, evt.newValue);
        ApplyVolume("MasterVolume", evt.newValue);
    }

    private void OnMusicChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(MusicKey, evt.newValue);
        ApplyVolume("MusicVolume", evt.newValue);
    }

    private void OnSFXChanged(ChangeEvent<float> evt)
    {
        PlayerPrefs.SetFloat(SFXKey, evt.newValue);
        ApplyEffects(evt.newValue);
    }

    private void ApplyEffects(float value)
    {
        // El slider Efectos controla también el ambiente.
        ApplyVolume("SFXVolume", value);
        ApplyVolume("AmbienceVolume", value);
    }

    private void ApplyVolume(string parameter, float value)
    {
        // Convierte el porcentaje a decibelios.
        // Evita calcular Log10(0); el mínimo del mixer es -80 dB.
        float decibels = value <= 0f
            ? -80f
            : Mathf.Max(-80f, 20f * Mathf.Log10(value / 100f));
         
        if (!mixer.SetFloat(parameter, decibels))
        {
            Debug.LogWarning(
                "No se encontró el parámetro del mixer: " + parameter,
                this
            );
        }
    }

    private void OnDisable()
    {
        applyOnNextUpdate = false;

        if (masterSlider != null)
            masterSlider.UnregisterValueChangedCallback(OnMasterChanged);

        if (musicSlider != null)
            musicSlider.UnregisterValueChangedCallback(OnMusicChanged);

        if (sfxSlider != null)
            sfxSlider.UnregisterValueChangedCallback(OnSFXChanged);

        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}