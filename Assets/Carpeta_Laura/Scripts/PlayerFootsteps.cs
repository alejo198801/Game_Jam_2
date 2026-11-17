using GameJam2.Core;
using GameJam2.Flow;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private AudioClip footstepsClip;
    [SerializeField, Min(0.01f)] private float minimumSpeed = 0.1f;

    private CharacterController controller;
    private AudioSource footstepsAudio;

    private void Awake()
    {
        footstepsAudio = GetComponent<AudioSource>();
        footstepsAudio.playOnAwake = false;
        footstepsAudio.loop = true;
        footstepsAudio.spatialBlend = 0f;

        if (playerMovement == null)
            playerMovement = GetComponentInParent<PlayerMovement>();

        if (playerMovement != null)
            controller = playerMovement.GetComponent<CharacterController>();

        if (playerMovement == null || controller == null || footstepsClip == null)
            Debug.LogError("PlayerFootsteps: asigna Player Movement y el clip de pasos.", this);
    }

    private void LateUpdate()
    {
        
        bool canWalk = playerMovement != null && controller != null &&
            playerMovement.isActiveAndEnabled && controller.enabled &&
            footstepsClip != null && !PlayerControlLock.IsLocked &&
            Time.timeScale > 0f;

        GameManager manager = GameManager.Instance;
        if (manager != null && manager.State != GameState.Playing)
            canWalk = false;

        bool walking = false;
        if (canWalk && controller.isGrounded)
        {
            Vector3 actualVelocity = controller.velocity;
            actualVelocity.y = 0f;

            
            walking = playerMovement.CurrentSpeed > minimumSpeed &&
                actualVelocity.sqrMagnitude > minimumSpeed * minimumSpeed;
        }

        if (walking)
        {
            if (!footstepsAudio.isPlaying)
            {
                footstepsAudio.clip = footstepsClip;
                footstepsAudio.Play();
            }
        }
        else if (footstepsAudio.isPlaying)
        {
            footstepsAudio.Stop();
        }
    }

    private void OnDisable()
    {
        if (footstepsAudio != null)
            footstepsAudio.Stop();
    }
}
