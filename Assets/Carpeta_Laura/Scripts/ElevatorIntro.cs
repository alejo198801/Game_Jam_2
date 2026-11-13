using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class ElevatorIntro : MonoBehaviour
{
    [Header("Tiempos en segundos")]
    [SerializeField, Min(0f)] private float initialDelay = 0.4f;
    [SerializeField, Min(0.1f)] private float openingDuration = 2f;

    [Header("Audio del ascensor")]
    [SerializeField] private AudioSource elevatorAudio;
    [SerializeField] private AudioClip arrivalBell;
    [SerializeField] private AudioClip doorOpening;

    private IEnumerator Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        VisualElement overlay = root.Q("elevator-overlay");
        VisualElement leftDoor = root.Q("elevator-door-left");
        VisualElement rightDoor = root.Q("elevator-door-right");
        VisualElement menuPanel = root.Q("menu-panel");

        if (overlay == null || leftDoor == null ||
            rightDoor == null || menuPanel == null)
        {
            Debug.LogError(
                "Revisa los nombres de las puertas y del menú en UI Builder.",
                this
            );
            yield break;
        }

        // Comienza con las puertas cerradas.
        overlay.style.display = DisplayStyle.Flex;
        leftDoor.style.left = Length.Percent(0f);
        rightDoor.style.right = Length.Percent(0f);

        // Evita activar botones con mouse o teclado durante la apertura.
        menuPanel.SetEnabled(false);

        // La campana anuncia la llegada.
        if (elevatorAudio != null && arrivalBell != null)
            elevatorAudio.PlayOneShot(arrivalBell);

        yield return new WaitForSecondsRealtime(initialDelay);

        // El sonido de puertas empieza junto con el movimiento.
        if (elevatorAudio != null && doorOpening != null)
            elevatorAudio.PlayOneShot(doorOpening);

        float elapsed = 0f;

        while (elapsed < openingDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / openingDuration);

            // Suaviza el arranque y el final del movimiento.
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);
            float position = Mathf.Lerp(0f, -60f, smoothProgress);

            // Cada puerta se desplaza hacia su lado.
            leftDoor.style.left = Length.Percent(position);
            rightDoor.style.right = Length.Percent(position);

            yield return null;
        }

        // Retira la capa para que no bloquee los clics.
        overlay.style.display = DisplayStyle.None;
        menuPanel.SetEnabled(true);

        root.Q<Button>("play-button")?.Focus();
    }
}