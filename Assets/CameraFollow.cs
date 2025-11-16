using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;          // El objeto a seguir (el player)
    public float distance = 5f;       // Distancia detrás del player
    public float height = 2f;         // Altura de la cámara
    public float rotationSpeed = 120f;// Velocidad de giro con el ratón
    public float followSmooth = 10f;  // Suavidad del seguimiento

    float yaw;  // rotación horizontal
    float pitch;// rotación vertical

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // ---- ROTACIÓN CON MOUSE ----
       yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -20f, 70f); // Limita la inclinación

        // rotación final
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // ---- POSICIÓN DETRÁS DEL PLAYER ----
        Vector3 desiredPos =
            target.position
            - rotation * Vector3.forward * distance
            + Vector3.up * height;

        // mover suavemente
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSmooth * Time.deltaTime);

        // siempre mira al player
        transform.LookAt(target.position + Vector3.up * height);
    }
}
