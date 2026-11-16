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

    [Header("Sonido de papel")]
    [SerializeField] private AudioSource paperAudio;
    [SerializeField] private AudioClip paperSlide;
    [SerializeField, Range(0f, 1f)] private float paperVolume = 0.3f;

    private VisualElement dossier;
    private VisualElement window;
    private VisualElement tabs;

    private VisualElement homePanel;
    private VisualElement optionsPanel;
    private VisualElement controlsPanel;
    private VisualElement creditsPanel;
    private VisualElement currentPage;

    private Button homeTab;
    private Button optionsTab;
    private Button controlsTab;
    private Button creditsTab;

    private Button optionsBackButton;
    private Button controlsBackButton;
    private Button creditsBackButton;
    private Button playButton;
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
        controlsPanel = root.Q("controls-panel");
        creditsPanel = root.Q("credits-panel");

        
        homeTab = root.Q<Button>("home-tab-button");
        optionsTab = root.Q<Button>("options-tab-button");
        controlsTab = root.Q<Button>("controls-tab-button");
        creditsTab = root.Q<Button>("credits-tab-button");

        
        optionsBackButton = root.Q<Button>("options-back-button");
        controlsBackButton = root.Q<Button>("controls-back-button");
        creditsBackButton = root.Q<Button>("credits-back-button");
        playButton = root.Q<Button>("play-button");
        quitButton = root.Q<Button>("quit-button");

        // Comprueba que todos los elementos existan.
        if (dossier == null || window == null || tabs == null ||
            homePanel == null || optionsPanel == null || controlsPanel == null || creditsPanel == null ||
            homeTab == null || optionsTab == null || controlsTab == null || creditsTab == null ||
            playButton == null || quitButton == null || optionsBackButton == null ||
            controlsBackButton == null || creditsBackButton == null)
        {
            Debug.LogError(
                "Faltan elementos del expediente. " +
                "Revisa sus Name en MainMenu.uxml.",
                this
            );
            return;
        }

       
        homeTab.clicked += ShowHome;
        optionsTab.clicked += ShowOptions;
        controlsTab.clicked += ShowControls;
        creditsTab.clicked += ShowCredits;

        
        optionsBackButton.clicked += ShowHome;
        controlsBackButton.clicked += ShowHome;
        creditsBackButton.clicked += ShowHome;
        quitButton.clicked += QuitGame;

        
        dossier.style.display = DisplayStyle.Flex;
        window.SetEnabled(true);
        tabs.SetEnabled(true);

        isAnimating = false;

        // Inicio aparece desplegado.
        homePanel.style.visibility = Visibility.Visible;
        homePanel.style.top = openTop;
        homePanel.style.display = DisplayStyle.Flex;

        
        optionsPanel.style.display = DisplayStyle.None;
        controlsPanel.style.display = DisplayStyle.None;
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

    private void ShowControls()
    {
        ChangePage(controlsPanel);
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
            playButton.Focus();
        else if (currentPage == optionsPanel)
            optionsBackButton.Focus();
        else if (currentPage == controlsPanel)
            controlsBackButton.Focus();
        else
            creditsBackButton.Focus();
    }

    private IEnumerator Slide(
        VisualElement sheet,
        float from,
        float to)
    {
        // Reproduce el papel una vez al comenzar el movimiento.
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
        if (normalTabImage == null || activeTabImage == null)
            return;

        SetTabImage(homeTab, currentPage == homePanel);
        SetTabImage(optionsTab, currentPage == optionsPanel);
        SetTabImage(controlsTab, currentPage == controlsPanel);
        SetTabImage(creditsTab, currentPage == creditsPanel);
    }

    private void SetTabImage(Button tab, bool isActive)
    {
        tab.style.backgroundImage = new StyleBackground(
            isActive ? activeTabImage : normalTabImage
        );

       
        Color textColor = isActive
            ? new Color32(229, 225, 223, 255) 
            : new Color32(37, 33, 36, 255);   

        
        tab.style.color = textColor;
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

        if (controlsTab != null)
            controlsTab.clicked -= ShowControls;

        if (creditsTab != null)
            creditsTab.clicked -= ShowCredits;

        

        if (optionsBackButton != null)
            optionsBackButton.clicked -= ShowHome;

        if (controlsBackButton != null)
            controlsBackButton.clicked -= ShowHome;

        if (creditsBackButton != null)
            creditsBackButton.clicked -= ShowHome;

        if (quitButton != null)
            quitButton.clicked -= QuitGame;
    }
}