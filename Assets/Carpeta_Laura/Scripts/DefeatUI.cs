using System.Collections;
using GameJam2.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(UIDocument))]
public class DefeatUI : MonoBehaviour
{
    [Header("Referencias de la escena")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PauseMenuController pauseMenu;
    [Tooltip("Audio Source del hijo SpiderAttackAudio de Capsule.")]
    [SerializeField] private AudioSource captureAudio;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private VisualElement defeatRoot;
    private VisualElement keypadRoot;
    private VisualElement noteRoot;
    private VisualElement prompt;
    private Button retryButton;
    private Button menuButton;
    private bool ready;
    private bool subscribed;
    private bool showing;
    private bool ownsLock;
    private bool frozeTime;
    private bool leavingScene;
    private float previousTimeScale;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private void OnEnable()
    {
        BindUI();
        ConnectManager();
    }

    private void Start()
    {
        if (!ready)
            BindUI();

        if (gameManager == null)
            gameManager = GameManager.Instance;
        ConnectManager();

        if (!ready || gameManager == null)
        {
            Debug.LogError("DefeatUI: revisa el documento y asigna Game Manager.", this);
            return;
        }

        if (gameManager.State == GameState.Lost)
            ShowDefeat();
    }

    private void BindUI()
    {
        if (ready)
            return;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        defeatRoot = root.Q("defeat-root");
        retryButton = root.Q<Button>("defeat-retry-button");
        menuButton = root.Q<Button>("defeat-menu-button");
        keypadRoot = root.Q("puzzle-root");
        noteRoot = root.Q("note-root");
        prompt = root.Q("interaction-prompt");

        if (defeatRoot != null)
            defeatRoot.style.display = DisplayStyle.None;

        if (defeatRoot == null || retryButton == null || menuButton == null)
            return;

        root.pickingMode = PickingMode.Ignore;
        defeatRoot.pickingMode = PickingMode.Position;
        retryButton.clicked += Retry;
        menuButton.clicked += ReturnToMenu;
        ready = true;
    }

    private void ConnectManager()
    {
        if (subscribed || gameManager == null)
            return;

        gameManager.OnLose.AddListener(ShowDefeat);
        subscribed = true;
    }

    public void ShowDefeat()
    {
        if (!ready || showing || gameManager == null)
            return;

        // Devuelve únicamente el bloqueo de Pausa, si estaba abierto.
        if (pauseMenu != null)
            pauseMenu.CloseForDefeat();

        previousTimeScale = Time.timeScale;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        leavingScene = false;
        showing = true;

        PlayerControlLock.Lock(true);
        ownsLock = true;

        if (keypadRoot != null) keypadRoot.style.display = DisplayStyle.None;
        if (noteRoot != null) noteRoot.style.display = DisplayStyle.None;
        if (prompt != null) prompt.style.display = DisplayStyle.None;
        defeatRoot.style.display = DisplayStyle.Flex;
        retryButton.SetEnabled(false);
        menuButton.SetEnabled(false);

        StartCoroutine(FinishCapture());
    }

    private IEnumerator FinishCapture()
    {
        // OnLose se emite dentro de Lose(): deja terminar ese método primero.
        yield return null;
        gameManager.CancelInvoke(nameof(GameManager.Restart));

        if (captureAudio != null && captureAudio.isPlaying)
        {
            // Límite de seguridad: un Loop activado por error no bloquea los botones.
            float duration = captureAudio.clip != null
                ? captureAudio.clip.length / Mathf.Max(0.01f, Mathf.Abs(captureAudio.pitch))
                : 5f;
            float deadline = Time.unscaledTime + duration + 0.25f;

            if (captureAudio.loop)
                Debug.LogWarning("Desactiva Loop en SpiderAttackAudio.", captureAudio);

            // El mundo sigue durante el ataque, pero el jugador ya está bloqueado.
            while (captureAudio != null && captureAudio.isPlaying &&
                   Time.unscaledTime < deadline)
                yield return null;
        }

        // Pausar antes silenciaría el ataque mediante EnemyAudioPause.
        Time.timeScale = 0f;
        frozeTime = true;
        retryButton.SetEnabled(true);
        menuButton.SetEnabled(true);
        retryButton.Focus();
    }

    private void Retry()
    {
        if (!showing || leavingScene || gameManager == null)
            return;

        leavingScene = true;
        PrepareSceneChange();
        gameManager.Restart();
    }

    private void ReturnToMenu()
    {
        if (!showing || leavingScene)
            return;

        if (!Application.CanStreamedLevelBeLoaded(mainMenuScene))
        {
            Debug.LogError("Añade " + mainMenuScene + " a la Scene List.", this);
            return;
        }

        leavingScene = true;
        PrepareSceneChange();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void PrepareSceneChange()
    {
        StopAllCoroutines();
        if (gameManager != null)
            gameManager.CancelInvoke(nameof(GameManager.Restart));

        ownsLock = false;
        frozeTime = false;
        Time.timeScale = 1f;
        PlayerControlLock.ResetAll();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (subscribed && gameManager != null)
            gameManager.OnLose.RemoveListener(ShowDefeat);
        subscribed = false;

        if (ready)
        {
            retryButton.clicked -= Retry;
            menuButton.clicked -= ReturnToMenu;
        }

        if (defeatRoot != null)
            defeatRoot.style.display = DisplayStyle.None;

        if (!leavingScene)
        {
            if (ownsLock) PlayerControlLock.Unlock();
            if (frozeTime) Time.timeScale = previousTimeScale;
            if (showing)
            {
                Cursor.lockState = previousCursorLock;
                Cursor.visible = previousCursorVisible;
            }
        }

        ownsLock = false;
        frozeTime = false;
        showing = false;
        ready = false;
    }
}
