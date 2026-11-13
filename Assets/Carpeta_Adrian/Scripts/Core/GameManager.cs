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
        [SerializeField] private float restartDelay = 2f;

        public UnityEvent OnWin;
        public UnityEvent OnLose;
        public UnityEvent OnRestart;

        public GameState State { get; private set; } = GameState.Playing;

        private void Awake()
        {
            // TODO: Singleton: guardar Instance y destruir duplicados.
            if(Instance != null && Instance != this) 
            {
                Destroy(gameObject);
                return; 
            }
            Instance = this; 

        }

        public void Win()
        {
            // TODO: Si se está jugando, pasar a Won e invocar OnWin.
            if(State != GameState.Playing)
            {
                return;
            }
            State = GameState.Won; 
            OnWin?.Invoke();
        }

        public void Lose()
        {
            // TODO: Si se está jugando, pasar a Lost, invocar OnLose y reiniciar tras restartDelay.
            if(State != GameState.Playing)
            {
                return;
            }
            State = GameState.Lost;
            OnLose?.Invoke();
            Invoke(nameof(Restart), restartDelay); 
        }

        /// Plantilla simple: recarga la escena. Cambiar a checkpoint si quieren reiniciar desde la habitación 2.
        public void Restart()
        {
            // TODO: Invocar OnRestart, liberar PlayerControlLock y recargar la escena o volver al checkpoint.
            OnRestart?.Invoke();
            PlayerControlLock.ResetAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }
    }
}
