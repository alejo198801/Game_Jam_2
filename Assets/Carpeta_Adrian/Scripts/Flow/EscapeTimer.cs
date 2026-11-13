// PROGRAMADOR B. OPCIONAL: cuenta regresiva del escape. Si llega a 0, pierde.
using GameJam2.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Flow
{
    public class EscapeTimer : MonoBehaviour
    {
        [SerializeField] private float duration = 60f;
        [SerializeField] private TMP_Text label;

        public UnityEvent<float> OnTick;   // segundos restantes
        public UnityEvent OnTimeUp;

        public float Remaining { get; private set; }
        private bool running;

        public void StartTimer()
        {
            // TODO: Poner Remaining = duration y empezar a contar.
            Remaining = duration;
            running = true;
            UpdateLabel();
        }
        public void StopTimer()
        {
            // TODO: Detener la cuenta.
            running = false;

        }

        private void Update()
        {
            // TODO: Restar tiempo, actualizar el texto, invocar OnTick; al llegar a 0 invocar OnTimeUp y GameManager.Lose().
            if(!running )
            {
                return;
            }
            Remaining -= Time.deltaTime;
            if(Remaining < 0f) 
            {
                Remaining = 0f;
            }

            UpdateLabel();
            OnTick?.Invoke(Remaining);
            if(Remaining <= 0f)
            {
                running = false;
                OnTimeUp?.Invoke();
                if(GameManager.Instance != null)
                {
                    GameManager.Instance.Lose();
                }
            }

        }

        private void UpdateLabel()
        {
            if(label == null)
            {
                return;
            }
            int total = Mathf.CeilToInt(Remaining);
            int minutes = total /60;
            int seconds = total % 60;
            label.text =  $"{minutes:00}:{seconds:00}";
        }
    }
}
