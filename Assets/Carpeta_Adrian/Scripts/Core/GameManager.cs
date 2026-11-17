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

        [SerializeField] private Transform room2Checkpoint;

        [Tooltip("Desmárcalo si usas una pantalla de derrota con botones.")]
        [SerializeField] private bool autoRestart = false;

        [SerializeField] private float restartDelay = 2f;

        public UnityEvent OnWin;
        public UnityEvent OnLose;
        public UnityEvent OnRestart;

        public GameState State { get; private set; } = GameState.Playing;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Win()
        {
            if (State != GameState.Playing)
                return;

            State = GameState.Won;
            OnWin?.Invoke();
        }

        public void Lose()
        {
            if (State != GameState.Playing)
                return;

            State = GameState.Lost;
            OnLose?.Invoke();

            // Actívalo solo si quieres reiniciar sin esperar al jugador.
            if (autoRestart)
                Invoke(nameof(Restart), restartDelay);
        }

        // Recarga la escena actual.
        public void Restart()
        {
            // Evita que quede otro reinicio automático pendiente.
            CancelInvoke(nameof(Restart));

            OnRestart?.Invoke();

            Time.timeScale = 1f;
            PlayerControlLock.ResetAll();

            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }

        // El nombre del menú debe coincidir con una escena de la Scene List.
        public void GoToMenu(string menuSceneName)
        {
            CancelInvoke(nameof(Restart));

            PlayerControlLock.ResetAll();
            Time.timeScale = 1f;

            SceneManager.LoadScene(menuSceneName);
        }
    }
}