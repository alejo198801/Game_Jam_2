// PROGRAMADOR B. Animaciones y orientación de la araña: idle, caminar y ataque.
// Va en la araña (el mismo objeto que NavMeshAgent y SpiderChase).
// Sirve con Animator (Mecanim) o con el componente Animation antiguo (Legacy).
using UnityEngine;
using UnityEngine.AI;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class SpiderAnimation : MonoBehaviour
    {
        [Tooltip("Por encima de esta velocidad se considera que camina.")]
        [SerializeField] private float walkThreshold = 0.2f;

        [Header("Orientación (el script gira la araña, no el NavMeshAgent)")]
        [Tooltip("0 si el modelo mira hacia su eje Z azul. 180 si camina de espaldas.")]
        [SerializeField] private float modelYawOffset = 180f;
        [Tooltip("Si está quieta, mira hacia aquí (arrastra al jugador). Opcional.")]
        [SerializeField] private Transform lookTarget;
        [SerializeField] private float turnSpeed = 540f;

        [Header("Si usa Animator (Mecanim)")]
        [SerializeField] private Animator animator;
        [SerializeField] private string speedParam = "Speed";     // Float
        [SerializeField] private string attackTrigger = "Attack"; // Trigger

        [Header("Si usa Animation (Legacy)")]
        [SerializeField] private Animation legacy;
        [SerializeField] private string idleClip = "idle";
        [SerializeField] private string walkClip = "walk";
        [SerializeField] private string attackClip = "attack";
        [SerializeField] private float walkAnimSpeedPerUnit = 0.3f;

        [Header("Diagnóstico")]
        [Tooltip("Escribe en la Console qué está pasando. Apágalo cuando funcione.")]
        [SerializeField] private bool debugLog = true;

        private NavMeshAgent agent;
        private bool attacking;
        private float logTimer;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (legacy == null) legacy = GetComponentInChildren<Animation>();
            agent.updateRotation = false; // la rotación la hace este script

            if (animator != null) animator.applyRootMotion = false;

            if (debugLog)
            {
                string ctrl = animator != null && animator.runtimeAnimatorController != null
                    ? animator.runtimeAnimatorController.name : "NINGUNO";
                bool hasParam = false;
                if (animator != null && animator.runtimeAnimatorController != null)
                    foreach (var p in animator.parameters)
                        if (p.name == speedParam && p.type == AnimatorControllerParameterType.Float) hasParam = true;

                Debug.Log($"[SpiderAnimation] Animator en: {(animator ? animator.gameObject.name : "NINGUNO")} | " +
                          $"Controller: {ctrl} | Parámetro float '{speedParam}': {(hasParam ? "OK" : "NO EXISTE")} | " +
                          $"Animators en la araña: {GetComponentsInChildren<Animator>(true).Length}", this);
            }
        }

        private void Update()
        {
            float speed = agent.enabled ? agent.velocity.magnitude : 0f;

            if (!attacking)
            {
                if (animator != null && animator.runtimeAnimatorController != null)
                {
                    animator.SetFloat(speedParam, speed);
                }
                else if (legacy != null)
                {
                    if (speed > walkThreshold)
                    {
                        legacy.CrossFade(walkClip, 0.2f);
                        legacy[walkClip].speed = Mathf.Max(0.5f, speed * walkAnimSpeedPerUnit);
                    }
                    else legacy.CrossFade(idleClip, 0.2f);
                }
            }

            if (debugLog)
            {
                logTimer -= Time.deltaTime;
                if (logTimer <= 0f)
                {
                    logTimer = 1f;
                    string state = "-";
                    if (animator != null && animator.runtimeAnimatorController != null)
                    {
                        var info = animator.GetCurrentAnimatorStateInfo(0);
                        state = info.IsName("rig|walk") ? "walk" : info.IsName("rig|idle_01") ? "idle" :
                                info.IsName("rig|attack_01") ? "attack" : "otro";
                        state += $" | Speed en Animator = {animator.GetFloat(speedParam):0.00}";
                    }
                    Debug.Log($"[SpiderAnimation] agent.enabled={agent.enabled} velocidad={speed:0.00} " +
                              $"estado={state}", this);
                }
            }
        }

        private void LateUpdate()
        {
            Vector3 dir = Vector3.zero;
            if (agent.enabled)
            {
                dir = agent.velocity;
                if (dir.sqrMagnitude < 0.01f) dir = agent.desiredVelocity;
            }
            if (dir.sqrMagnitude < 0.01f && lookTarget != null) dir = lookTarget.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.0001f) return;

            Quaternion target = Quaternion.LookRotation(dir.normalized, Vector3.up) * Quaternion.Euler(0f, modelYawOffset, 0f);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }

        /// Conectar al evento OnPlayerCaught de SpiderChase.
        public void PlayAttack()
        {
            attacking = true;
            if (animator != null && animator.runtimeAnimatorController != null)
                animator.SetTrigger(attackTrigger);
            else if (legacy != null)
                legacy.CrossFade(attackClip, 0.1f);
        }

        public void ResetAnim() => attacking = false;
    }
}
