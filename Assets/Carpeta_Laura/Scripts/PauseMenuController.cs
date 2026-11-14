using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

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

    private VisualElement root;
    private VisualElement window;
    private VisualElement tabs;
    private VisualElement pausePanel;
    private VisualElement optionsPanel;
    private VisualElement currentPage;

    private Button resumeButton;
    private Button optionsButton;
    private Button backButton;
    private Button mainMenuButton;
    private Button pauseTab;
    private Button optionsTab;

    private bool isReady;
    private bool isPaused;
    private bool showingOptions;
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

        resumeButton = root.Q<Button>("resume-button");
        optionsButton = root.Q<Button>("pause-options-button");
        backButton = root.Q<Button>("options-back-button");
        mainMenuButton = root.Q<Button>("main-menu-button");
        pauseTab = root.Q<Button>("pause-tab-button");
        optionsTab = root.Q<Button>("options-tab-button");

        // También oculta el cartón, el bolsillo y las pestañas.
        root.style.display = DisplayStyle.None;

        isReady = window != null && tabs != null &&
                  pausePanel != null && optionsPanel != null &&
                  resumeButton != null && optionsButton != null &&
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
        optionsButton.clicked += ShowOptions;
        backButton.clicked += ShowPausePanel;
        mainMenuButton.clicked += ReturnToMainMenu;
        pauseTab.clicked += ShowPausePanel;
        optionsTab.clicked += ShowOptions;

        window.SetEnabled(true);
        tabs.SetEnabled(true);
    }

    private void Update()
    {
        if (!isReady || isAnimating || Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (!isPaused)
                Pause();
            else if (showingOptions)
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
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Cada vez que pausamos, la hoja comienza guardada.
        currentPage = null;
        pausePanel.style.display = DisplayStyle.None;
        optionsPanel.style.display = DisplayStyle.None;

        root.style.display = DisplayStyle.Flex;
        ShowPausePanel();
    }

    private void ShowPausePanel()
    {
        ChangePage(false);
    }

    private void ShowOptions()
    {
        ChangePage(true);
    }

    private void ChangePage(bool openOptions)
    {
        if (!isPaused || isAnimating)
            return;

        VisualElement nextPage = openOptions
            ? optionsPanel
            : pausePanel;

        if (nextPage == currentPage)
            return;

        StartCoroutine(SwitchPage(nextPage, openOptions));
    }

    private IEnumerator SwitchPage(
        VisualElement nextPage,
        bool openOptions)
    {
        isAnimating = true;

        // Bloquea controles mientras se están moviendo.
        window.SetEnabled(false);
        tabs.SetEnabled(false);

        // Primero guarda la hoja que estaba abierta.
        if (currentPage != null)
        {
            yield return Slide(currentPage, openTop, hiddenTop);
            currentPage.style.display = DisplayStyle.None;
        }

        showingOptions = openOptions;
        UpdateTabImages();

        // Después extrae la nueva hoja desde el bolsillo.
        nextPage.style.top = hiddenTop;
        nextPage.style.display = DisplayStyle.Flex;

        yield return Slide(nextPage, hiddenTop, openTop);

        currentPage = nextPage;
        window.SetEnabled(true);
        tabs.SetEnabled(true);
        isAnimating = false;

        if (showingOptions)
            backButton.Focus();
        else
            resumeButton.Focus();
    }

    private IEnumerator Slide(
        VisualElement sheet,
        float from,
        float to)
    {
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            // Sigue avanzando aunque Time.timeScale sea cero.
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
        if (normalTabImage == null || activeTabImage == null)
            return;

        pauseTab.style.backgroundImage = new StyleBackground(
            showingOptions ? normalTabImage : activeTabImage
        );

        optionsTab.style.backgroundImage = new StyleBackground(
            showingOptions ? activeTabImage : normalTabImage
        );
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
        Time.timeScale = previousTimeScale;
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;

        isPaused = false;
        showingOptions = false;
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

    private void OnDisable()
    {
        StopAllCoroutines();
        isAnimating = false;

        if (resumeButton != null)
            resumeButton.clicked -= Resume;

        if (optionsButton != null)
            optionsButton.clicked -= ShowOptions;

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