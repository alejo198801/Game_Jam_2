// PROGRAMADOR B. La araña del pasillo: NavMeshAgent + Collider "Is Trigger" + Rigidbody kinematic.
// Necesita el paquete AI Navigation y un NavMeshSurface horneado en el pasillo.
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
            // TODO: Obtener el NavMeshAgent y dejarlo detenido.
            agent = GetComponent<NavMeshAgent>();
            SetStopped(true);
        }

        /// Conéctenlo al OnDoorOpened de la puerta 2 o a un trigger del pasillo.
        public void StartChase()
        {
            // TODO: Iniciar la persecución tras startDelay. Conectar al OnDoorOpened de la puerta 2.
            if(chasing)
            {
                return;
            }
            CancelInvoke(nameof(BeginChase));
            Invoke(nameof(BeginChase), startDelay);
        }

        private void BeginChase()
        {
            // TODO: Activar el agente con baseSpeed e invocar OnChaseStart.
            chasing = true;
            chaseTime = 0f; 
            timer = repathInterval;
            agent.speed = baseSpeed;
            SetStopped(false);
            OnChaseStart?.Invoke();
        }

        public void StopChase()
        {
            // TODO: Cancelar el inicio pendiente y detener el agente.
            CancelInvoke(nameof(BeginChase));
            chasing = false;
            SetStopped(true);
        }

        private void Update()
        {
        if (!chasing || player == null) return;

    // 1. Sube la velocidad con el tiempo, sin pasar de maxSpeed
        chaseTime += Time.deltaTime;
        agent.speed = Mathf.Min(baseSpeed + speedGainPerSecond * chaseTime, maxSpeed);

    // 2. Opcional: solo avanza si el jugador se mueve
        if (onlyMoveWhenPlayerMoves && playerMovement != null)
          SetStopped(playerMovement.CurrentSpeed < 0.1f);

    // 3. Cada repathInterval actualiza el destino
        timer += Time.deltaTime;
        if (timer >= repathInterval)
        {   
            timer = 0f;
            if (agent.isOnNavMesh) agent.SetDestination(player.position);
        }

    // 4. Si se queda muy lejos, reaparece más cerca del jugador
            if (catchUpDistance > 0f &&
            Vector3.Distance(transform.position, player.position) > catchUpDistance)
            {
            Vector3 away = (transform.position - player.position).normalized;
            Vector3 target = player.position + away * catchUpOffset;
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                agent.Warp(hit.position);
            }
            }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Si toca al Player mientras persigue: detenerse, invocar OnPlayerCaught y GameManager.Lose().
            if(!chasing || !other.CompareTag(GameTags.Player))
            {
                return ;
            }
            StopChase();
            OnPlayerCaught?.Invoke();
            if(GameManager.Instance != null )
            {
                GameManager.Instance.Lose();
            }
        }
        // Evita errores si el agente aún no está sobre el NavMesh
        private void SetStopped(bool stopped)
        {
            if (agent != null && agent.isOnNavMesh) agent.isStopped = stopped;
        }
    }
}
