// PROGRAMADOR B. Video de introducción al iniciar la escena.
// Mientras se ve: jugador bloqueado (o apagado). Al terminar (o al saltarlo):
// se apaga el video, aparece el jugador y se puede jugar.
// Va en un objeto "IntroVideo" que tenga el componente Video Player.
using System.Collections;
using GameJam2.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace GameJam2.Flow
{
    [RequireComponent(typeof(VideoPlayer))]
    public class IntroVideo : MonoBehaviour
    {
        [Header("Video")]
        [Tooltip("Arrastra el .mp4 aquí. (Si exportan a WebGL, deja esto vacío y usa Streaming File Name.)")]
        [SerializeField] private VideoClip clip;
        [Tooltip("Solo WebGL: nombre del archivo dentro de Assets/StreamingAssets, ej. intro.mp4")]
        [SerializeField] private string streamingFileName = "";
        [Tooltip("Cámara donde se dibuja el video. Vacío = una cámara propia que crea el script.")]
        [SerializeField] private Camera videoCamera;

        [Header("Jugador")]
        [Tooltip("Objeto del jugador (Y Bot). Se apaga durante el video y se prende al final.")]
        [SerializeField] private GameObject player;
        [Tooltip("Desmarca si prefieres que el jugador esté visible pero congelado durante el video.")]
        [SerializeField] private bool hidePlayerDuringVideo = true;

        [Header("Saltar")]
        [SerializeField] private bool allowSkip = true;
        [Tooltip("Espera antes de permitir saltar, para que no se salte sin querer.")]
        [SerializeField] private float skipAfterSeconds = 1f;

        public UnityEvent OnIntroFinished;

        // Se queda en true aunque se recargue la escena (Reiniciar), hasta cerrar el juego.
        private static bool alreadyPlayed;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatic() => alreadyPlayed = false;

        /// Por si desde el menú principal quieren que "Nuevo juego" vuelva a mostrar la intro.
        public static void ResetIntro() => alreadyPlayed = false;

        private VideoPlayer vp;
        private bool finished;
        private bool createdCamera;
        private float startTime;

        private void Awake()
        {
            vp = GetComponent<VideoPlayer>();
            vp.playOnAwake = false;

            if (alreadyPlayed)
            {
                finished = true;
                if (player != null) player.SetActive(true);
                OnIntroFinished?.Invoke(); // lo que esté conectado arranca igual al reintentar
                gameObject.SetActive(false);
                return;
            }
            alreadyPlayed = true;

            vp.playOnAwake = false;
            vp.isLooping = false;

            if (clip != null) { vp.source = VideoSource.VideoClip; vp.clip = clip; }
            else if (!string.IsNullOrEmpty(streamingFileName))
            {
                vp.source = VideoSource.Url;
                vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, streamingFileName);
            }

            if (videoCamera == null)
            {
                var go = new GameObject("IntroVideoCamera");
                videoCamera = go.AddComponent<Camera>();
                videoCamera.clearFlags = CameraClearFlags.SolidColor;
                videoCamera.backgroundColor = Color.black;
                videoCamera.depth = 100; // encima de la cámara del jugador
                createdCamera = true;
            }
            vp.renderMode = VideoRenderMode.CameraNearPlane;
            vp.targetCamera = videoCamera;
            vp.aspectRatio = VideoAspectRatio.FitInside;

            if (player != null && hidePlayerDuringVideo) player.SetActive(false);
            PlayerControlLock.Lock();
        }

        private IEnumerator Start()
        {
            vp.loopPointReached += _ => Finish();
            vp.errorReceived += (_, msg) => { Debug.LogWarning("[IntroVideo] Error: " + msg); Finish(); };

            vp.Prepare();
            while (!vp.isPrepared && !finished) yield return null;
            if (finished) yield break;

            startTime = Time.unscaledTime;
            vp.Play();
        }

        private void Update()
        {
            if (finished || !allowSkip || Time.unscaledTime - startTime < skipAfterSeconds) return;
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            if ((kb != null && (kb.spaceKey.wasPressedThisFrame || kb.enterKey.wasPressedThisFrame)) ||
                (mouse != null && mouse.leftButton.wasPressedThisFrame))
                Finish();
        }

        private void Finish()
        {
            if (finished) return;
            finished = true;

            vp.Stop();
            if (createdCamera) Destroy(videoCamera.gameObject);

            if (player != null) player.SetActive(true);
            PlayerControlLock.Unlock();
            OnIntroFinished?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
