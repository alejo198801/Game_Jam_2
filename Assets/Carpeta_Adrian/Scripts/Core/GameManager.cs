// COMPARTIDO (dueño: Programador B). Cualquiera puede llamar Win() o Lose().
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace GameJam2.Core
{
    public enum GameState { Playing, Won, Lost }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private Transform room2Checkpoint;   // dónde reaparece al morir
        [Tooltip("Desmárcalo si usas GameOverUI (el jugador elige reiniciar o ir al menú).")]
        [SerializeField] private bool autoRestart = false;
        [SerializeField] private float restartDelay = 2f;

        public UnityEvent OnWin;
        public UnityEvent OnLose;
        public UnityEvent OnRestart;

        public GameState State { get; private set; } = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Win()
        {
            if (State != GameState.Playing) return;
            State = GameState.Won;
            OnWin?.Invoke();
        }

        public void Lose()
        {
            if (State != GameState.Playing) return;
            State = GameState.Lost;
            OnLose?.Invoke();
            if (autoRestart) Invoke(nameof(Restart), restartDelay);
        }

        /// Plantilla simple: recarga la escena. Cambiar a checkpoint si quieren reiniciar desde la habitación 2.
        public void Restart()
        {
            OnRestart?.Invoke();
            PlayerControlLock.ResetAll();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// Para el botón "Menú principal". Pon el nombre exacto de la escena del menú (debe estar en Build Profiles/Scene List).
        public void GoToMenu(string menuSceneName)
        {
            PlayerControlLock.ResetAll();
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }
    }
}
