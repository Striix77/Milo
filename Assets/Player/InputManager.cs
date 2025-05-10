using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInput PlayerInput;
    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool JumpWasReleased;
    public static bool JumpIsHeld;

    public static bool FireWasPressed;
    public static bool FireIsHeld;
    public static bool DashWasPressed;
    public static bool MeleeWasPressed;
    public static bool Ability1WasPressed;
    public static bool UltimateWasPressed;


    private InputAction jumpAction;
    private InputAction movementAction;
    private InputAction fireAction;
    private InputAction dashAction;
    private InputAction meleeAction;
    private InputAction ability1Action;
    private InputAction ultimateAction;


    public PlayerAbilities playerAbilities;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();
        jumpAction = PlayerInput.actions["Jump"];
        movementAction = PlayerInput.actions["Move"];
        fireAction = PlayerInput.actions["Fire"];
        dashAction = PlayerInput.actions["Dash"];
        meleeAction = PlayerInput.actions["Melee"];
        ability1Action = PlayerInput.actions["Ability1"];
        ultimateAction = PlayerInput.actions["Ultimate"];
    }

    private void Update()
    {
        Debug.Log("PlayerAbilities: " + playerAbilities.isUlting);
        if (!playerAbilities.isUlting)
        {
            Movement = movementAction.ReadValue<Vector2>();
            if (Movement.x > 0)
            {
                Movement.x = 1;
            }
            else if (Movement.x < 0)
            {
                Movement.x = -1;
            }
            JumpWasPressed = jumpAction.WasPressedThisFrame();
            JumpWasReleased = jumpAction.WasReleasedThisFrame();
            JumpIsHeld = jumpAction.IsPressed();
            FireWasPressed = fireAction.WasPressedThisFrame();
            FireIsHeld = fireAction.IsPressed();
            DashWasPressed = dashAction.WasPressedThisFrame();
            MeleeWasPressed = meleeAction.WasPressedThisFrame();
            Ability1WasPressed = ability1Action.WasPressedThisFrame();
            UltimateWasPressed = ultimateAction.WasPressedThisFrame();
        }
        else
        {
            Movement = Vector2.zero;
            JumpWasPressed = false;
            JumpWasReleased = false;
            JumpIsHeld = false;
            FireWasPressed = false;
            FireIsHeld = false;
            DashWasPressed = false;
            MeleeWasPressed = false;
            Ability1WasPressed = false;
            UltimateWasPressed = false;
        }
    }


}
