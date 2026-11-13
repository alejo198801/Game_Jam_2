// PROGRAMADOR B. Al final del pasillo: Collider "Is Trigger". Si el jugador entra, gana.
using GameJam2.Core;
using UnityEngine;
using UnityEngine.Events;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(Collider))]
    public class ExitTrigger : MonoBehaviour
    {
        [SerializeField] private SpiderChase spider;
        public UnityEvent OnPlayerEscaped;

        private void Reset() => GetComponent<Collider>().isTrigger = true;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(GameTags.Player)) return;

            if (spider != null) spider.StopChase();
            OnPlayerEscaped?.Invoke();
            if (GameManager.Instance != null) GameManager.Instance.Win();
        }
    }
}