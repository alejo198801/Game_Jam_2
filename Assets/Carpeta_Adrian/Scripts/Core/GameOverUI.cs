// PROGRAMADOR B. Pantalla de fin: al perder (o ganar) muestra un panel con
// "Reiniciar" y "Menú principal". Se engancha solo a GameManager.OnLose / OnWin.
// Va en el Canvas (o en cualquier objeto que siempre esté activo, NO dentro del panel).
using System.Collections;
using GameJam2.Core;
using UnityEngine;

namespace GameJam2.Flow
{
    public class GameOverUI : MonoBehaviour
    {
        [Header("Paneles (empiezan desactivados)")]
        [SerializeField] private GameObject losePanel;
        [Tooltip("Opcional: panel de victoria con los mismos botones.")]
        [SerializeField] private GameObject winPanel;

        [Header("Ajustes")]
        [Tooltip("Espera antes de mostrar el panel, para que se vea el mordisco.")]
        [SerializeField] private float showDelay = 1.2f;
        [Tooltip("Congela el juego cuando aparece el panel.")]
        [SerializeField] private bool pauseGame = true;
        [Tooltip("Nombre EXACTO de la escena del menú principal.")]
        [SerializeField] private string menuSceneName = "MainMenu";

        private bool shown;

        private void Awake()
        {
            if (losePanel != null) losePanel.SetActive(false);
            if (winPanel != null) winPanel.SetActive(false);
        }

        private void Start()
        {
            var gm = GameManager.Instance;
            if (gm == null)
            {
                Debug.LogWarning("[GameOverUI] No hay GameManager en la escena. Crea un objeto vacío con el componente GameManager.");
                return;
            }
            gm.OnLose.AddListener(() => StartCoroutine(Show(losePanel)));
            gm.OnWin.AddListener(() => StartCoroutine(Show(winPanel)));
        }

        private IEnumerator Show(GameObject panel)
        {
            if (shown || panel == null) yield break;
            shown = true;

            PlayerControlLock.Lock(showCursor: true); // congela al jugador y suelta el mouse
            yield return new WaitForSecondsRealtime(showDelay);

            panel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (pauseGame) Time.timeScale = 0f;
        }

        // ---- Botones (OnClick en el Inspector) ----
        public void OnRestartButton()
        {
            if (GameManager.Instance != null) GameManager.Instance.Restart();
        }

        public void OnMenuButton()
        {
            if (GameManager.Instance != null) GameManager.Instance.GoToMenu(menuSceneName);
        }
    }
}
