using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class GlideAndLateralControllerRB : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;

    [Header("Flight Settings")]
    public float forwardSpeed = 20f;
    public float maxDiveSpeed = 50f;
    public float maxGlideLift = 15f;
    public float rotationSmooth = 5f;

    [Header("Lateral Banking")]
    public float lateralRotationZ = 30f;
    public float lateralSpinDuration = 1f;
    public float lateralMoveUnits = 3f;
    public float lateralCooldown = 2f;

    [Header("Front Reference (Hijo)")]
    public Transform front;

    private Rigidbody rb;
    private float yaw;
    private float pitch;
    private Vector3 direction;
    private float currentVerticalSpeed;

    private bool isSpinningRight = false;
    private bool isSpinningLeft = false;
    private float spinAngle = 0f;
    private Vector3 lateralOffset = Vector3.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 2f;

        if (front == null)
            Debug.LogWarning("Asigna un hijo como 'front' (mesh del cubo).");

        direction = transform.forward.normalized;
    }

    void Update()
    {
        HandleMouseLook();
        HandleLateralSpinInput();
    }

    void FixedUpdate()
    {
        HandleForwardMovementRB();
        HandleRotationRB();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -70f, 40f);

        Camera.main.transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleForwardMovementRB()
    {
        // Dirección hacia adelante
        direction = transform.forward;

        // Ajuste vertical por pitch
        if (pitch < 0)
        {
            float diveFactor = Mathf.Abs(pitch / 70f);
            currentVerticalSpeed = -diveFactor * maxDiveSpeed;
        }
        else
        {
            float glideFactor = pitch / 40f;
            currentVerticalSpeed = glideFactor * maxGlideLift;
        }

        // Velocidad deseada
        Vector3 desiredVelocity = direction * forwardSpeed + Vector3.up * currentVerticalSpeed;

        // Aplicamos fuerza proporcional a la diferencia con la velocidad actual
        Vector3 force = (desiredVelocity - rb.linearVelocity);
        rb.AddForce(force, ForceMode.Acceleration); // simula empuje físico realista

        // Offset lateral temporal
        if (lateralOffset != Vector3.zero)
        {
            rb.MovePosition(rb.position + lateralOffset);
        }
    }

    void HandleRotationRB()
    {
        Vector3 moveDirection = direction + Vector3.up * (currentVerticalSpeed / (currentVerticalSpeed < 0 ? maxDiveSpeed : maxGlideLift));
        moveDirection.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        if (ActionHub.Instance != null)
        {
            Vector2 moveInput = ActionHub.Instance.MoveInput;
            if (moveInput.x != 0)
            {
                float zRotation = lateralRotationZ * -Mathf.Sign(moveInput.x);
                Quaternion lateralRot = Quaternion.Euler(0f, 0f, zRotation);
                targetRotation *= lateralRot;
            }
        }

        // Slerp para rotación suave usando Rigidbody
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotationSmooth * Time.fixedDeltaTime));

        // Aplica giro lateral acumulativo sobre Z
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, 0f, spinAngle));

        if (front != null)
            front.rotation = rb.rotation;
    }

    void HandleLateralSpinInput()
    {
        if (ActionHub.Instance == null) return;

        Vector2 moveInput = ActionHub.Instance.MoveInput;

        if (moveInput.x > 0 && !isSpinningRight)
            StartCoroutine(LateralSpinCoroutine(1));
        else if (moveInput.x < 0 && !isSpinningLeft)
            StartCoroutine(LateralSpinCoroutine(-1));
    }

    IEnumerator LateralSpinCoroutine(float direction)
    {
        if (direction > 0) isSpinningRight = true;
        else isSpinningLeft = true;

        float elapsed = 0f;
        float startSpin = 0f;
        float endSpin = 360f * direction;

        Vector3 startOffset = Vector3.zero;
        Vector3 endOffset = transform.right * lateralMoveUnits * direction;

        while (elapsed < lateralSpinDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lateralSpinDuration);

            spinAngle = Mathf.Lerp(startSpin, endSpin, t);
            lateralOffset = Vector3.Lerp(startOffset, endOffset, t);

            yield return null;
        }

        spinAngle = 0f;
        lateralOffset = Vector3.zero;

        yield return new WaitForSeconds(lateralCooldown);

        if (direction > 0) isSpinningRight = false;
        else isSpinningLeft = false;
    }
}
