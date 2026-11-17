using UnityEngine;

public class EnemyAudioPause : MonoBehaviour
{
    private AudioSource[] sources;
    private bool[] pausedByThisScript;

    private void Awake()
    {
        
        sources = GetComponentsInChildren<AudioSource>(true);
        pausedByThisScript = new bool[sources.Length];
    }

    private void LateUpdate()
    {
        bool gamePaused = Time.timeScale == 0f;

        for (int i = 0; i < sources.Length; i++)
        {
            AudioSource source = sources[i];

            if (source == null)
                continue;

            if (gamePaused)
            {
                if (!pausedByThisScript[i] && source.isPlaying)
                {
                    source.Pause();
                    pausedByThisScript[i] = true;
                }
            }
            else if (pausedByThisScript[i])
            {
                source.UnPause();
                pausedByThisScript[i] = false;
            }
        }
    }

    private void OnDisable()
    {
        if (sources == null)
            return;

        for (int i = 0; i < sources.Length; i++)
        {
            if (sources[i] != null && pausedByThisScript[i])
                sources[i].UnPause();

            pausedByThisScript[i] = false;
        }
    }
}