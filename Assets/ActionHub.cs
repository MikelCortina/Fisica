using UnityEngine;
using UnityEngine.InputSystem;

public class ActionHub : MonoBehaviour
{
    public static ActionHub Instance { get; private set; }

    private GameInputActions actions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    public event System.Action JumpPressed;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        actions = new GameInputActions();

        // Suscripción a eventos
        actions.Player.Jump.performed += ctx => JumpPressed?.Invoke();

        actions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        actions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

        actions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
        actions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

        EnableGameplayInputs();
    }

    private void OnDestroy()
    {
        actions.Dispose();
    }

    // ==========================
    // ACTIVAR / DESACTIVAR INPUT
    // ==========================

    public void EnableGameplayInputs()
    {
        actions.Player.Enable();
    }

    public void DisableGameplayInputs()
    {
        actions.Player.Disable();
    }

    public void DisableAll()
    {
        actions.Disable();
    }

    public void EnableAll()
    {
        actions.Enable();
    }
}
