using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    [Header("Pestañas")]
    [SerializeField] private Texture2D normalTabImage;
    [SerializeField] private Texture2D activeTabImage;

    [Header("Animación")]
    [SerializeField, Min(0.1f)] private float slideDuration = 0.4f;
    [SerializeField] private float openTop = 48f;
    [SerializeField] private float hiddenTop = 600f;

    private VisualElement dossier;
    private VisualElement window;
    private VisualElement tabs;
    private VisualElement homePanel;
    private VisualElement optionsPanel;
    private VisualElement creditsPanel;
    private VisualElement currentPage;

    private Button homeTab;
    private Button optionsTab;
    private Button creditsTab;
    private Button optionsButton;
    private Button creditsButton;
    private Button optionsBackButton;
    private Button creditsBackButton;
    private Button quitButton;

    private bool isAnimating;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        dossier = root.Q("dossier-root");
        window = root.Q("dossier-window");
        tabs = root.Q("dossier-tabs");

        homePanel = root.Q("menu-panel");
        optionsPanel = root.Q("options-panel");
        creditsPanel = root.Q("credits-panel");

        homeTab = root.Q<Button>("home-tab-button");
        optionsTab = root.Q<Button>("options-tab-button");
        creditsTab = root.Q<Button>("credits-tab-button");

        optionsButton = root.Q<Button>("options-button");
        creditsButton = root.Q<Button>("credits-button");
        optionsBackButton = root.Q<Button>("options-back-button");
        creditsBackButton = root.Q<Button>("credits-back-button");
        quitButton = root.Q<Button>("quit-button");

        if (dossier == null || window == null || tabs == null ||
            homePanel == null || optionsPanel == null ||
            creditsPanel == null || homeTab == null ||
            optionsTab == null || creditsTab == null ||
            optionsButton == null || creditsButton == null ||
            optionsBackButton == null || creditsBackButton == null ||
            quitButton == null)
        {
            Debug.LogError(
                "Faltan elementos del expediente. Revisa sus Name.",
                this
            );
            return;
        }

        homeTab.clicked += ShowHome;
        optionsTab.clicked += ShowOptions;
        creditsTab.clicked += ShowCredits;

        optionsButton.clicked += ShowOptions;
        creditsButton.clicked += ShowCredits;
        optionsBackButton.clicked += ShowHome;
        creditsBackButton.clicked += ShowHome;
        quitButton.clicked += QuitGame;

        // El expediente permanece visible detrás del ascensor.
        dossier.style.display = DisplayStyle.Flex;
        window.SetEnabled(true);
        tabs.SetEnabled(true);

        // La hoja inicial ya está desplegada.
        homePanel.style.visibility = Visibility.Visible;
        homePanel.style.top = openTop;
        homePanel.style.display = DisplayStyle.Flex;

        optionsPanel.style.display = DisplayStyle.None;
        creditsPanel.style.display = DisplayStyle.None;

        currentPage = homePanel;
        UpdateTabImages();
    }

    private void ShowHome()
    {
        ChangePage(homePanel);
    }

    private void ShowOptions()
    {
        ChangePage(optionsPanel);
    }

    private void ShowCredits()
    {
        ChangePage(creditsPanel);
    }

    private void ChangePage(VisualElement nextPage)
    {
        if (isAnimating || nextPage == currentPage)
            return;

        StartCoroutine(SwitchPage(nextPage));
    }

    private IEnumerator SwitchPage(VisualElement nextPage)
    {
        isAnimating = true;

        // Evita nuevas pulsaciones durante el movimiento.
        window.SetEnabled(false);
        tabs.SetEnabled(false);

        // Guarda la hoja actual.
        yield return Slide(currentPage, openTop, hiddenTop);
        currentPage.style.display = DisplayStyle.None;

        currentPage = nextPage;
        UpdateTabImages();

        // Extrae la siguiente hoja.
        currentPage.style.top = hiddenTop;
        currentPage.style.display = DisplayStyle.Flex;

        yield return Slide(currentPage, hiddenTop, openTop);

        window.SetEnabled(true);
        tabs.SetEnabled(true);
        isAnimating = false;

        if (currentPage == homePanel)
            optionsButton.Focus();
        else if (currentPage == optionsPanel)
            optionsBackButton.Focus();
        else
            creditsBackButton.Focus();
    }

    private IEnumerator Slide(
        VisualElement sheet,
        float from,
        float to)
    {
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
        if (normalTabImage == null || activeTabImage == null)
            return;

        SetTabImage(homeTab, currentPage == homePanel);
        SetTabImage(optionsTab, currentPage == optionsPanel);
        SetTabImage(creditsTab, currentPage == creditsPanel);
    }

    private void SetTabImage(Button tab, bool isActive)
    {
        tab.style.backgroundImage = new StyleBackground(
            isActive ? activeTabImage : normalTabImage
        );
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        isAnimating = false;

        if (homeTab != null)
            homeTab.clicked -= ShowHome;

        if (optionsTab != null)
            optionsTab.clicked -= ShowOptions;

        if (creditsTab != null)
            creditsTab.clicked -= ShowCredits;

        if (optionsButton != null)
            optionsButton.clicked -= ShowOptions;

        if (creditsButton != null)
            creditsButton.clicked -= ShowCredits;

        if (optionsBackButton != null)
            optionsBackButton.clicked -= ShowHome;

        if (creditsBackButton != null)
            creditsBackButton.clicked -= ShowHome;

        if (quitButton != null)
            quitButton.clicked -= QuitGame;
    }
}