using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using GameJam2.Core;

[RequireComponent(typeof(UIDocument))]
public class PauseMenuController : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("Pestañas")]
    [SerializeField] private Texture2D normalTabImage;
    [SerializeField] private Texture2D activeTabImage;

    [Header("Animación")]
    [SerializeField, Min(0.1f)] private float slideDuration = 0.4f;
    [SerializeField] private float openTop = 48f;
    [SerializeField] private float hiddenTop = 600f;

    [Header("Sonido de papel")]
    [SerializeField] private AudioSource paperAudio;
    [SerializeField] private AudioClip paperSlide;
    [SerializeField, Range(0f, 1f)] private float paperVolume = 0.3f;

    private VisualElement root;
    private VisualElement window;
    private VisualElement tabs;
    private VisualElement pausePanel;
    private VisualElement optionsPanel;
    private VisualElement currentPage;
    private VisualElement controlsPanel;

    private Button resumeButton;
    private Button controlsTab;
    private Button controlsBackButton;
    private Button backButton;
    private Button mainMenuButton;
    private Button pauseTab;
    private Button optionsTab;

    private bool isReady;
    private bool isPaused;
    private bool isAnimating;

    private float previousTimeScale;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private void OnEnable()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        window = root.Q("dossier-window");
        tabs = root.Q("dossier-tabs");
        pausePanel = root.Q("pause-panel");
        optionsPanel = root.Q("options-panel");
        controlsPanel = root.Q("controls-panel");

        resumeButton = root.Q<Button>("resume-button");
        controlsTab = root.Q<Button>("controls-tab-button");
        controlsBackButton = root.Q<Button>("controls-back-button");
        backButton = root.Q<Button>("options-back-button");
        mainMenuButton = root.Q<Button>("main-menu-button");
        pauseTab = root.Q<Button>("pause-tab-button");
        optionsTab = root.Q<Button>("options-tab-button");


        root.style.display = DisplayStyle.None;

        isReady = window != null && tabs != null &&
                  pausePanel != null && optionsPanel != null && controlsPanel != null &&
                  resumeButton != null && controlsTab != null && controlsBackButton != null &&
                  backButton != null && mainMenuButton != null &&
                  pauseTab != null && optionsTab != null;

        if (!isReady)
        {
            Debug.LogError(
                "Faltan elementos del dossier. Revisa sus Name.",
                this
            );
            return;
        }

        resumeButton.clicked += Resume;
        controlsTab.clicked += ShowControls;
        controlsBackButton.clicked += ShowPausePanel;
        backButton.clicked += ShowPausePanel;
        mainMenuButton.clicked += ReturnToMainMenu;
        pauseTab.clicked += ShowPausePanel;
        optionsTab.clicked += ShowOptions;

        window.SetEnabled(true);
        tabs.SetEnabled(true);
    }

    private void Update()
    {
        
        if (GameManager.Instance != null &&
            GameManager.Instance.State != GameState.Playing)
        {
            CloseForResult();
            return;
        }

        if (GameJam2.Puzzles.KeypadPanel.BlocksPauseThisFrame)
            return;

        if (!isReady || isAnimating || Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                Pause();
            else if (currentPage != pausePanel)
                ShowPausePanel();
            else
                Resume();
        }
    }

    private void Pause()
    {
        previousTimeScale = Time.timeScale;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        isPaused = true;
        PlayerControlLock.Lock();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        currentPage = null;
        pausePanel.style.display = DisplayStyle.None;
        optionsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;

        root.style.display = DisplayStyle.Flex;
        ShowPausePanel();
    }

    private void ShowPausePanel()
    {
        ChangePage(pausePanel);
    }

    private void ShowOptions()
    {
        ChangePage(optionsPanel);
    }

    private void ShowControls()
    {
        ChangePage(controlsPanel);
    }

    private void ChangePage(VisualElement nextPage)
    {
        if (!isPaused || isAnimating)
            return;

        if (nextPage == currentPage)
            return;

        StartCoroutine(SwitchPage(nextPage));
    }

    private IEnumerator SwitchPage(VisualElement nextPage)
    {
        isAnimating = true;

        
        window.SetEnabled(false);
        tabs.SetEnabled(false);


        if (currentPage != null)
        {
            yield return Slide(currentPage, openTop, hiddenTop);
            currentPage.style.display = DisplayStyle.None;
        }

        currentPage = nextPage;
        UpdateTabImages();


        nextPage.style.top = hiddenTop;
        nextPage.style.display = DisplayStyle.Flex;

        yield return Slide(nextPage, hiddenTop, openTop);

        currentPage = nextPage;
        window.SetEnabled(true);
        tabs.SetEnabled(true);
        isAnimating = false;

        if (currentPage == optionsPanel)
            backButton.Focus();
        else if (currentPage == controlsPanel)
            controlsBackButton.Focus();
        else
            resumeButton.Focus();
    }

    private IEnumerator Slide(
        VisualElement sheet,
        float from,
        float to)

    {
        if (paperAudio != null && paperSlide != null)
            paperAudio.PlayOneShot(paperSlide, paperVolume);

        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / slideDuration);
            float smooth = Mathf.SmoothStep(0f, 1f, progress);

            sheet.style.top = Mathf.Lerp(from, to, smooth);
            yield return null;
        }

        sheet.style.top = to;
    }

    private void UpdateTabImages()
    {
        SetTabImage(pauseTab, currentPage == pausePanel);
        SetTabImage(optionsTab, currentPage == optionsPanel);
        SetTabImage(controlsTab, currentPage == controlsPanel);
    }

    private void SetTabImage(Button tab, bool isActive)
    {
        if (normalTabImage != null && activeTabImage != null)
            tab.style.backgroundImage = new StyleBackground(
                isActive ? activeTabImage : normalTabImage
            );

        Color textColor = isActive
            ? new Color32(229, 225, 223, 255)
            : new Color32(37, 33, 36, 255);
        tab.style.color = textColor;
    }

    private void Resume()
    {
        if (!isPaused || isAnimating)
            return;

        root.style.display = DisplayStyle.None;
        RestoreGameState();
    }

    private void RestoreGameState()
    {
        PlayerControlLock.Unlock();

        Time.timeScale = previousTimeScale;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;

        isPaused = false;
    }

    private void ReturnToMainMenu()
    {
        if (isAnimating)
            return;

        if (!Application.CanStreamedLevelBeLoaded(mainMenuScene))
        {
            Debug.LogError(
                "Añade " + mainMenuScene + " a la Scene List.",
                this
            );
            return;
        }

        Resume();

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(mainMenuScene);
    }

   
    public void CloseForResult()
    {
        CloseForDefeat();
    }

    public void CloseForDefeat()
    {
        if (!isPaused)
            return;

        StopAllCoroutines();
        isAnimating = false;

        root.style.display = DisplayStyle.None;
        RestoreGameState();

        window.SetEnabled(true);
        tabs.SetEnabled(true);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isAnimating = false;

        if (resumeButton != null)
            resumeButton.clicked -= Resume;

        if (controlsTab != null)
            controlsTab.clicked -= ShowControls;

        if (controlsBackButton != null)
            controlsBackButton.clicked -= ShowPausePanel;

        if (backButton != null)
            backButton.clicked -= ShowPausePanel;

        if (mainMenuButton != null)
            mainMenuButton.clicked -= ReturnToMainMenu;

        if (pauseTab != null)
            pauseTab.clicked -= ShowPausePanel;

        if (optionsTab != null)
            optionsTab.clicked -= ShowOptions;

        if (isPaused)
            RestoreGameState();

        isReady = false;
    }


}