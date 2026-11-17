using GameJam2.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

[RequireComponent(typeof(UIDocument))]
public class VictoryUI : MonoBehaviour
{
    [Header("Referencias de la escena")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PauseMenuController pauseMenu;
    [SerializeField] private string mainMenuScene = "MainMenu";

    private VisualElement victoryRoot;
    private VisualElement keypadRoot;
    private VisualElement noteRoot;
    private VisualElement prompt;
    private Button retryButton;
    private Button menuButton;
    private bool ready;
    private bool subscribed;
    private bool showing;
    private bool leavingScene;
    private bool ownsLock;
    private float previousTimeScale;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private void OnEnable()
    {
        BindUI();
        ConnectManager();

        
        if (ready && gameManager != null && gameManager.State == GameState.Won)
            ShowVictory();
    }

    private void Start()
    {
        BindUI();
        ConnectManager();

        if (!ready || gameManager == null)
        {
            Debug.LogError("VictoryUI: revisa los Name del documento y asigna Game Manager.", this);
            return;
        }

        if (gameManager.State == GameState.Won)
            ShowVictory();
    }

    private void BindUI()
    {
        if (ready)
            return;

        VisualElement root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null)
            return;

        victoryRoot = root.Q("victory-root");
        retryButton = victoryRoot?.Q<Button>("victory-retry-button");
        menuButton = victoryRoot?.Q<Button>("victory-menu-button");
        keypadRoot = root.Q("puzzle-root");
        noteRoot = root.Q("note-root");
        prompt = root.Q("interaction-prompt");

        if (victoryRoot != null)
            victoryRoot.style.display = DisplayStyle.None;

        if (victoryRoot == null || retryButton == null || menuButton == null)
            return;

        root.pickingMode = PickingMode.Ignore;
        victoryRoot.pickingMode = PickingMode.Position;
        retryButton.clicked += Retry;
        menuButton.clicked += ReturnToMenu;
        ready = true;
    }

    private void ConnectManager()
    {
        if (subscribed)
            return;

        if (gameManager == null)
            gameManager = GameManager.Instance;

        if (gameManager == null)
            return;

        gameManager.OnWin.AddListener(ShowVictory);
        subscribed = true;
    }

    public void ShowVictory()
    {
        if (!ready || showing || gameManager == null ||
            gameManager.State != GameState.Won)
            return;

        
        if (pauseMenu != null)
            pauseMenu.CloseForResult();

        previousTimeScale = Time.timeScale;
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;
        leavingScene = false;
        showing = true;

        PlayerControlLock.Lock(true);
        ownsLock = true;
        Time.timeScale = 0f;

        if (keypadRoot != null) keypadRoot.style.display = DisplayStyle.None;
        if (noteRoot != null) noteRoot.style.display = DisplayStyle.None;
        if (prompt != null) prompt.style.display = DisplayStyle.None;

        victoryRoot.style.display = DisplayStyle.Flex;
        retryButton.SetEnabled(true);
        menuButton.SetEnabled(true);
        retryButton.Focus();
    }

    private void Retry()
    {
        if (!showing || leavingScene || gameManager == null)
            return;

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

        PrepareSceneChange();
        SceneManager.LoadScene(mainMenuScene);
    }

    private void PrepareSceneChange()
    {
        leavingScene = true;
        ownsLock = false;
        gameManager.CancelInvoke(nameof(GameManager.Restart));
        Time.timeScale = 1f;
        PlayerControlLock.ResetAll();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        if (subscribed && gameManager != null)
            gameManager.OnWin.RemoveListener(ShowVictory);
        subscribed = false;

        if (ready)
        {
            retryButton.clicked -= Retry;
            menuButton.clicked -= ReturnToMenu;
        }

        if (victoryRoot != null)
            victoryRoot.style.display = DisplayStyle.None;

       
        if (showing && !leavingScene)
        {
            if (ownsLock) PlayerControlLock.Unlock();
            Time.timeScale = previousTimeScale;
            Cursor.lockState = previousCursorLock;
            Cursor.visible = previousCursorVisible;
        }

        ownsLock = false;
        showing = false;
        ready = false;
    }
}
