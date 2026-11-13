using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private VisualElement menuPanel;
    private VisualElement optionsPanel;
    private VisualElement creditsPanel;
    private Label gameTitle;

    private Button optionsButton;
    private Button backButton;    
    private Button creditsButton;
    private Button creditsBackButton;
    private Button quitButton;

    private void OnEnable()
    {
        // Busca los elementos por su Name en UI Builder.
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        menuPanel = root.Q<VisualElement>("menu-panel");
        optionsPanel = root.Q<VisualElement>("options-panel");
        gameTitle = root.Q<Label>("game-title");

        optionsButton = root.Q<Button>("options-button");
        backButton = root.Q<Button>("options-back-button");

        creditsPanel = root.Q<VisualElement>("credits-panel");
        creditsButton = root.Q<Button>("credits-button");
        creditsBackButton = root.Q<Button>("credits-back-button");
        quitButton = root.Q<Button>("quit-button");

        // Detecta nombres incorrectos o elementos que falten.
        if (menuPanel == null || optionsPanel == null ||
            gameTitle == null || optionsButton == null ||
            backButton == null || creditsPanel == null ||
            creditsButton == null || creditsBackButton == null ||
            quitButton == null)
        {
            Debug.LogError(
                "Faltan elementos del menú. Revisa sus Name en UI Builder.",
                this
            );
            return;
        }

        // Conecta los botones.
        optionsButton.clicked += ShowOptions;
        backButton.clicked += ShowMainMenu;
        creditsButton.clicked += ShowCredits;
        creditsBackButton.clicked += ShowMainMenu;
        quitButton.clicked += QuitGame;

        // Al iniciar, muestra la carpeta.
        ShowMainMenu();
    }

    private void OnDisable()
    {
        // Desconecta los eventos para evitar registros duplicados.
        if (optionsButton != null)
            optionsButton.clicked -= ShowOptions;

        if (backButton != null)
            backButton.clicked -= ShowMainMenu;

        if (creditsButton != null)
            creditsButton.clicked -= ShowCredits;

        if (creditsBackButton != null)
            creditsBackButton.clicked -= ShowMainMenu;

        if (quitButton != null)
            quitButton.clicked -= QuitGame;
    }

    private void ShowOptions()
    {
        // Conserva la carpeta como fondo, pero oculta sus controles.
        creditsPanel.style.display = DisplayStyle.None;
        menuPanel.style.visibility = Visibility.Hidden;
        gameTitle.style.display = DisplayStyle.None;
        optionsPanel.style.display = DisplayStyle.Flex;

        backButton.Focus();
    }

    private void ShowMainMenu()
    {
        creditsPanel.style.display = DisplayStyle.None;
        optionsPanel.style.display = DisplayStyle.None;
        menuPanel.style.visibility = Visibility.Visible;
        gameTitle.style.display = DisplayStyle.Flex;

        optionsButton.Focus();
    }

    private void ShowCredits()
    {
        menuPanel.style.visibility = Visibility.Hidden;
        gameTitle.style.display = DisplayStyle.None;
        optionsPanel.style.display = DisplayStyle.None;
        creditsPanel.style.display = DisplayStyle.Flex;

        creditsBackButton.Focus();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        // En Unity, detiene Play sin cerrar el editor.
        UnityEditor.EditorApplication.isPlaying = false;
#else
    // En la versión de escritorio, cierra el juego.
    Application.Quit();
#endif
    }
}