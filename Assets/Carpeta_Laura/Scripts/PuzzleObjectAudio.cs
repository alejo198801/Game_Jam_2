using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PuzzleObjectAudio : MonoBehaviour
{
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip incorrectSound;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickup()
    {
        Play(pickupSound);
    }

    public void PlayCorrect()
    {
        Play(correctSound);
    }

    public void PlayIncorrect()
    {
        Play(incorrectSound);
    }

    private void Play(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}