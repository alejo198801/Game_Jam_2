// PROGRAMADOR B. La araña del pasillo: NavMeshAgent + Collider "Is Trigger" + Rigidbody kinematic.
// Necesita el paquete AI Navigation y un NavMeshSurface horneado en el pasillo.
using System.Collections;
using GameJam2.Core;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SpiderChase : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private float repathInterval = 0.2f;
        [SerializeField] private float startDelay = 1.5f;

        [Header("Dificultad")]
        [SerializeField] private float baseSpeed = 4.5f;
        [SerializeField] private float speedGainPerSecond = 0.05f;
        [SerializeField] private float maxSpeed = 5.5f;
        [Tooltip("Si se queda más lejos que esto, aparece más cerca. 0 = desactivado.")]
        [SerializeField] private float catchUpDistance = 0f;
        [SerializeField] private float catchUpOffset = 6f;

        [Header("Caída inicial (opcional)")]
        [Tooltip("Punto en el PISO (sobre el NavMesh azul) donde cae la araña. Vacío = no cae, empieza en el piso.")]
        [SerializeField] private Transform dropPoint;
        [SerializeField] private float dropDuration = 0.6f;
        public UnityEvent OnLanded;   // sonido de golpe, polvo, sacudida de cámara

        [Header("Opcional: solo avanza si el jugador se mueve")]
        [SerializeField] private bool onlyMoveWhenPlayerMoves = false;
        [SerializeField] private PlayerMovement playerMovement;

        public UnityEvent OnChaseStart;
        public UnityEvent OnPlayerCaught;

        private NavMeshAgent agent;
        private bool chasing;
        private float timer;
        private float chaseTime;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            // Si empieza arriba (fuera del NavMesh), el agente se apaga hasta que caiga.
            if (dropPoint != null) agent.enabled = false;
            else if (agent.isOnNavMesh) agent.isStopped = true;
        }

        /// Conéctenlo al OnDoorOpened de la puerta 2 o a un trigger del pasillo.
        public void StartChase()
        {
            if (chasing || dropping) return;
            if (dropPoint != null && !agent.enabled) StartCoroutine(DropThenChase());
            else Invoke(nameof(BeginChase), startDelay);
        }

        private bool dropping;

        private IEnumerator DropThenChase()
        {
            dropping = true;
            yield return new WaitForSeconds(startDelay);

            OnChaseStart?.Invoke();          // aquí puede sonar el chillido antes de caer
            Vector3 from = transform.position;
            Vector3 to = dropPoint.position;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime / dropDuration;
                float k = t * t;               // acelera como si cayera
                transform.position = Vector3.Lerp(from, to, Mathf.Clamp01(k));
                yield return null;
            }

            agent.enabled = true;
            agent.Warp(to);
            OnLanded?.Invoke();
            dropping = false;
            BeginChase();
        }

        private void BeginChase()
        {
            chasing = true;
            chaseTime = 0f;
            agent.speed = baseSpeed;
            agent.isStopped = false;
            if (dropPoint == null) OnChaseStart?.Invoke();
        }

        public void StopChase()
        {
            CancelInvoke(nameof(BeginChase));
            chasing = false;
            if (agent.enabled && agent.isOnNavMesh) agent.isStopped = true;
        }

        private void Update()
        {
            if (!chasing) return;

            chaseTime += Time.deltaTime;
            agent.speed = Mathf.Min(maxSpeed, baseSpeed + speedGainPerSecond * chaseTime);

            if (onlyMoveWhenPlayerMoves && playerMovement != null)
                agent.isStopped = playerMovement.CurrentSpeed < 0.1f;

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = repathInterval;
                agent.SetDestination(player.position);

                if (catchUpDistance > 0f && Vector3.Distance(transform.position, player.position) > catchUpDistance)
                {
                    Vector3 behind = player.position - player.forward * catchUpOffset;
                    if (NavMesh.SamplePosition(behind, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                        agent.Warp(hit.position);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!chasing || !other.CompareTag(GameTags.Player)) return;
            StopChase();
            OnPlayerCaught?.Invoke();
            if (GameManager.Instance != null) GameManager.Instance.Lose();
        }
    }
}
