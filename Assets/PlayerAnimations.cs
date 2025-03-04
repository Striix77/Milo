using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    public static readonly int Idle = Animator.StringToHash("PlayerIdle");
    public static readonly int Walk = Animator.StringToHash("PlayerWalking");
    public static readonly int Jump = Animator.StringToHash("PlayerJump");
    public static readonly int Fall = Animator.StringToHash("PlayerFalling");
    public static readonly int Land = Animator.StringToHash("PlayerLanding");
    public static readonly int Dash = Animator.StringToHash("PlayerDashing");

    private static int currentState = Idle;

    public static void SetState(int state)
    {

        currentState = state;
    }

    public static int GetState()
    {
        return currentState;
    }

    public static void CrossFade(Animator animator, int state, float transitionTime)
    {
        if (currentState == state)
        {
            return;
        }
        animator.CrossFade(state, transitionTime);
        currentState = state;
    }
}
