using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpWasReleased;
    public static bool JumpIsHeld;

    public static bool DashWasPressed;


    private InputAction jumpAction;
    private InputAction movementAction;
    private InputAction dashAction;




    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        jumpAction = PlayerInput.actions["Jump"];
        movementAction = PlayerInput.actions["Move"];
        dashAction = PlayerInput.actions["Dash"];
    }

    private void Update()
    {
        Movement = movementAction.ReadValue<Vector2>();
        JumpWasPressed = jumpAction.WasPressedThisFrame();
        JumpWasReleased = jumpAction.WasReleasedThisFrame();
        JumpIsHeld = jumpAction.IsPressed();
        DashWasPressed = dashAction.WasPressedThisFrame();
    }


}
