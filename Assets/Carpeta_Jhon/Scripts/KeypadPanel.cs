using GameJam2.Core;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static Readme;
using Cursor = UnityEngine.Cursor;

namespace GameJam2.Puzzles
{
    
    public class KeypadPanel : MonoBehaviour
    {
        [SerializeField] private CodeManager codeManager;
        [SerializeField] private UIDocument puzzleDocument;

        [Header("Interfaz antigua (solo para ocultarla)")]
        [SerializeField] private GameObject panelRoot;

        private VisualElement overlay;
        private Label display;
        private Label status;
        private readonly Button[] digitButtons = new Button[10];
        private readonly Action[] digitActions = new Action[10];
        private Button deleteButton;
        private Button submitButton;
        private Button closeButton;
        private string input = "";
        private bool ready;
        private CursorLockMode previousCursorLock;
        private bool previousCursorVisible;

        public bool IsOpen { get; private set; }

        
        private static int openPanels;
        private static int lastClosedFrame = -1;
        public static bool BlocksPauseThisFrame =>
            openPanels > 0 || lastClosedFrame == Time.frameCount;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetState()
        {
            openPanels = 0;
            lastClosedFrame = -1;
        }

        private void OnEnable()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);

            
            if (puzzleDocument != null)
                BindUI();
        }

        private void Start()
        {
            if (!ready)
                BindUI();
        }

        private bool BindUI()
        {
            if (ready)
                return true;

            if (puzzleDocument == null || codeManager == null)
            {
                Debug.LogError("KeypadPanel: asigna Puzzle Document y Code Manager.", this);
                return false;
            }

            VisualElement documentRoot = puzzleDocument.rootVisualElement;
            overlay = documentRoot.Q("puzzle-root");
            display = documentRoot.Q<Label>("keypad-display");
            status = documentRoot.Q<Label>("keypad-status");
            deleteButton = documentRoot.Q<Button>("keypad-delete-button");
            submitButton = documentRoot.Q<Button>("keypad-submit-button");
            closeButton = documentRoot.Q<Button>("keypad-close-button");

            bool valid = overlay != null && display != null && status != null &&
                deleteButton != null && submitButton != null && closeButton != null;

            for (int i = 0; i < digitButtons.Length; i++)
            {
                digitButtons[i] = documentRoot.Q<Button>($"digit-{i}-button");
                valid &= digitButtons[i] != null;
            }

            if (overlay != null)
                overlay.style.display = DisplayStyle.None;

            if (!valid)
            {
                Debug.LogError("KeypadPanel: faltan elementos. Revisa los Name de PuzzleUI.uxml.", this);
                return false;
            }

           
            documentRoot.pickingMode = PickingMode.Ignore;
            overlay.pickingMode = PickingMode.Position;

            for (int i = 0; i < digitButtons.Length; i++)
            {
                int digit = i;
                digitActions[i] = () => PressDigit(digit);
                digitButtons[i].clicked += digitActions[i];
            }

            deleteButton.clicked += Delete;
            submitButton.clicked += Submit;
            closeButton.clicked += Close;
            ready = true;
            Refresh();
            return true;
        }

        public void Show()
        {
            if (!isActiveAndEnabled || IsOpen || Time.timeScale == 0f || !BindUI())
                return;

            previousCursorLock = Cursor.lockState;
            previousCursorVisible = Cursor.visible;
            input = "";
            status.text = "BLOQUEADO · INGRESE CÓDIGO";
            Refresh();

            IsOpen = true;
            openPanels++;
            PlayerControlLock.Lock(true);
            overlay.style.display = DisplayStyle.Flex;
            digitButtons[1].Focus();
        }

        public void Close()
        {
            if (!IsOpen)
                return;

            if (overlay != null)
                overlay.style.display = DisplayStyle.None;

            IsOpen = false;
            openPanels = Mathf.Max(0, openPanels - 1);
            lastClosedFrame = Time.frameCount;
            
            PlayerControlLock.Unlock();
            Cursor.lockState = previousCursorLock;
            Cursor.visible = previousCursorVisible;
        }

        public void PressDigit(int digit)
        {
            if (!IsOpen || digit < 0 || digit > 9 || input.Length >= codeManager.CodeLength)
                return;

            input += digit.ToString();
            status.text = "BLOQUEADO · INGRESE CÓDIGO";
            Refresh();
        }

        public void Delete()
        {
            if (!IsOpen || input.Length == 0)
                return;

            input = input.Substring(0, input.Length - 1);
            status.text = "BLOQUEADO · INGRESE CÓDIGO";
            Refresh();
        }

        public void Submit()
        {
            if (!IsOpen)
                return;

            if (input.Length != codeManager.CodeLength)
            {
                status.text = "CÓDIGO INCOMPLETO";
                return;
            }

           
            if (codeManager.TrySubmit(input))
            {
                Close();
                return;
            }

            input = "";
            status.text = "CÓDIGO INCORRECTO · INTENTE DE NUEVO";
            Refresh();
        }

        private void Update()
        {
            if (IsOpen && Keyboard.current != null &&
                Keyboard.current.escapeKey.wasPressedThisFrame)
                Close();
        }

        private void Refresh()
        {
            if (display != null && codeManager != null)
                display.text = string.Join(" ", input.PadRight(codeManager.CodeLength, '_').ToCharArray());
        }

        private void OnDisable()
        {
            Close();

            if (!ready)
                return;

            for (int i = 0; i < digitButtons.Length; i++)
                digitButtons[i].clicked -= digitActions[i];

            deleteButton.clicked -= Delete;
            submitButton.clicked -= Submit;
            closeButton.clicked -= Close;
            ready = false;
        }
    }
}
