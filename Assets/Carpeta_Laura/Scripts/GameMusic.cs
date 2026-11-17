using GameJam2.Core;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class GameMusic : MonoBehaviour
{
    [Header("Músicas")]
    [SerializeField] private AudioClip explorationMusic;
    [SerializeField] private AudioClip chaseMusic;

    [Header("Volumen de cada pista")]
    [SerializeField, Range(0f, 1f)] private float explorationVolume = 0.2f;
    [SerializeField, Range(0f, 1f)] private float chaseVolume = 0.3f;

    [Header("Referencia opcional: se busca el singleton si está vacía")]
    [SerializeField] private GameManager gameManager;

    private AudioSource musicAudio;
    private bool subscribed;
    private bool pausedByThis;
    private bool finished;
    private bool initialized;

    private void Awake()
    {
        musicAudio = GetComponent<AudioSource>();
        musicAudio.playOnAwake = false;
        musicAudio.loop = true;
        musicAudio.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        ConnectManager();
        if (initialized)
            PlayExploration();
    }

    private void Start()
    {
        ConnectManager();
        initialized = true;

       
        if (musicAudio.clip == null)
            PlayExploration();
    }

    private void ConnectManager()
    {
        if (subscribed)
            return;

        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager == null)
            return;

        gameManager.OnWin.AddListener(StopMusic);
        gameManager.OnLose.AddListener(StopMusic);
        subscribed = true;
    }

    public void PlayExploration()
    {
        PlayTrack(explorationMusic, explorationVolume);
    }

    
    public void PlayChase()
    {
        PlayTrack(chaseMusic, chaseVolume);
    }

    private void PlayTrack(AudioClip clip, float volume)
    {
        if (!isActiveAndEnabled || finished ||
            (gameManager != null && gameManager.State != GameState.Playing))
            return;

        if (clip == null)
        {
            Debug.LogWarning("GameMusic: falta asignar una música.", this);
            return;
        }

        
        musicAudio.Stop();
        pausedByThis = false;
        musicAudio.clip = clip;
        musicAudio.volume = volume;
        musicAudio.Play();
        UpdatePause();
    }

    private void LateUpdate()
    {
        
        if (gameManager != null && gameManager.State != GameState.Playing)
        {
            if (!finished) StopMusic();
            return;
        }

        UpdatePause();
    }

    private void UpdatePause()
    {
        if (finished)
            return;

        if (Time.timeScale == 0f && !pausedByThis && musicAudio.isPlaying)
        {
            musicAudio.Pause();
            pausedByThis = true;
        }
        else if (Time.timeScale > 0f && pausedByThis)
        {
            musicAudio.UnPause();
            pausedByThis = false;
        }
    }

    public void StopMusic()
    {
        finished = true;
        pausedByThis = false;
        musicAudio.Stop();
    }

    private void OnDisable()
    {
        if (subscribed && gameManager != null)
        {
            gameManager.OnWin.RemoveListener(StopMusic);
            gameManager.OnLose.RemoveListener(StopMusic);
        }

        subscribed = false;
        if (musicAudio != null) musicAudio.Stop();
        pausedByThis = false;
    }
}
