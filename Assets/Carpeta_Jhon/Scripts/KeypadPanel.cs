// PROGRAMADOR A. Lógica del teclado. Los botones del Canvas llaman PressDigit, Delete, Submit y Close.
using GameJam2.Core;
using TMPro;
using UnityEngine;

namespace GameJam2.Puzzles
{
    public class KeypadPanel : MonoBehaviour
    {
        [SerializeField] private CodeManager codeManager;
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private TMP_Text display;

        private string input = "";
        public bool IsOpen => panelRoot.activeSelf;

        private void Start()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);
        }

        public void Show()
        {
            input = "";
            Refresh();
            if (panelRoot != null)
                panelRoot.SetActive(true);

            PlayerControlLock.Lock();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Close()
        {
            if (panelRoot != null)
                panelRoot.SetActive(false);

            PlayerControlLock.Unlock();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void PressDigit(int digit)
        {
            if (codeManager != null && input.Length < codeManager.CodeLength)
            {
                input += digit.ToString();
                Refresh();
            }
        }

        public void Delete()
        {
            if (input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Refresh();
            }
        }

        public void Submit()
        {
            if (codeManager != null)
            {
                bool success = codeManager.TrySubmit(input);
                if (success)
                {
                    Close();
                }
                else
                {
                    input = "";
                    Refresh();
                }
            }
        }

        private void Update()
        {
            if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
        }

        private void Refresh()
        {
            if (display != null && codeManager != null)
            {
                display.text = input.PadRight(codeManager.CodeLength, '_');
            }
        }
    }
}