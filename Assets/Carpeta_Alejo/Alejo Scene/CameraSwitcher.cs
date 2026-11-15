using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Referencias del Personaje Mixamo")]
    [Tooltip("Arrastra aquí el objeto raíz del jugador (el que tiene el CharacterController o Rigidbody)")]
    public Transform playerBody;
    
    [Tooltip("Arrastra aquí el hueso 'mixamorig:Head' dentro del esqueleto del personaje")]
    public Transform headBone;

    [Header("Ajuste de Posición (Ojos)")]
    [Tooltip("Desplazamiento desde el centro del hueso de la cabeza hacia los ojos (Y = altura, Z = hacia adelante para no ver el interior de la cara)")]
    public Vector3 eyeOffset = new Vector3(0f, 0.1f, 0.12f);

    [Header("Control de Cámara (Ratón)")]
    public float mouseSensitivity = 0.15f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    private float xRotation = 0f;

    void Start()
    {
        // Bloquea y oculta el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (playerBody == null || headBone == null) return;

        // 1. Posicionar la cámara exactamente en los ojos del hueso de la cabeza de Mixamo
        transform.position = headBone.TransformPoint(eyeOffset);

        // 2. Leer movimiento del ratón con el nuevo Input System
        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 mouseDelta = mouse.delta.ReadValue();
            float mouseX = mouseDelta.x * mouseSensitivity;
            float mouseY = mouseDelta.y * mouseSensitivity;

            // Rotación vertical (mirar arriba/abajo) aplicada solo a la cámara
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

            // Rotación horizontal (girar izquierda/derecha) aplicada al cuerpo del jugador
            playerBody.Rotate(Vector3.up * mouseX);

            // Aplicar la rotación final a la cámara alineada con el cuerpo
            transform.rotation = Quaternion.Euler(xRotation, playerBody.eulerAngles.y, 0f);
        }
    }
}