using GameJam2.Puzzles;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(AudioSource))]
public class KeypadAudio : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private UIDocument puzzleDocument;
    [SerializeField] private CodeManager codeManager;

    [Header("Sonidos")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip wrongAnswerSound;
    [SerializeField] private AudioClip rightAnswerSound;

    private AudioSource audioSource;
    private readonly Button[] digitButtons = new Button[10];
    private Button deleteButton;
    private bool connected;

    private void OnEnable()
    {
        Connect();
    }

    private void Start()
    {
        // Segundo intento por si el documento aún no estaba preparado.
        if (!connected)
            Connect();

        if (!connected)
        {
            Debug.LogError(
                "KeypadAudio: revisa Puzzle Document, Code Manager " +
                "y los nombres de los botones.",
                this
            );
        }
    }

    private void Connect()
    {
        if (connected || puzzleDocument == null || codeManager == null)
            return;

        audioSource = GetComponent<AudioSource>();
        VisualElement root = puzzleDocument.rootVisualElement;

        deleteButton = root.Q<Button>("keypad-delete-button");

        bool valid = deleteButton != null;

        for (int i = 0; i < digitButtons.Length; i++)
        {
            digitButtons[i] = root.Q<Button>($"digit-{i}-button");
            valid &= digitButtons[i] != null;
        }

        if (!valid)
            return;

        foreach (Button button in digitButtons)
            button.clicked += PlayClick;

        deleteButton.clicked += PlayClick;

        codeManager.OnCodeWrong.AddListener(PlayWrongAnswer);
        codeManager.OnCodeCorrect.AddListener(PlayRightAnswer);

        connected = true;
    }

    private void PlayClick()
    {
        Play(clickSound);
    }

    private void PlayWrongAnswer()
    {
        Play(wrongAnswerSound);
    }

    private void PlayRightAnswer()
    {
        Play(rightAnswerSound);
    }

    private void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    private void OnDisable()
    {
        if (!connected)
            return;

        foreach (Button button in digitButtons)
            button.clicked -= PlayClick;

        deleteButton.clicked -= PlayClick;

        if (codeManager != null)
        {
            codeManager.OnCodeWrong.RemoveListener(PlayWrongAnswer);
            codeManager.OnCodeCorrect.RemoveListener(PlayRightAnswer);
        }

        connected = false;
    }
}