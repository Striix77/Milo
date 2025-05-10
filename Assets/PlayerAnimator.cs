using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{

    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("MiloStart"))
        {
            AnimationControl();
        }
    }

    private void AnimationControl()
    {

        if (PlayerAbilities.isMeleeing)
        {
            Debug.Log("Meleeing");
            PlayerAnimations.CrossFade(animator, PlayerAnimations.Melee, 0.1f);
        }
        else
        {
            if (PlayerMovement.isGrounded)
            {
                if (PlayerMovement.isDashing)
                {
                    PlayerAnimations.CrossFade(animator, PlayerAnimations.Dash, 0.1f);
                }
                else if (InputManager.Movement.x != 0)
                {
                    PlayerAnimations.CrossFade(animator, PlayerAnimations.Walk, 0.1f);
                }
                else if (PlayerAnimations.GetState() == PlayerAnimations.Fall)
                {
                    PlayerAnimations.CrossFade(animator, PlayerAnimations.Land, 0.1f);
                }
                else
                {
                    if (PlayerAnimations.GetState() == PlayerAnimations.Land)
                    {
                        PlayerAnimations.CrossFade(animator, PlayerAnimations.Idle, 0.03f);
                    }
                    else
                    {
                        PlayerAnimations.CrossFade(animator, PlayerAnimations.Idle, 0.1f);
                    }
                }
            }
            else
            {
                if (PlayerMovement.isDashing)
                {
                    PlayerAnimations.CrossFade(animator, PlayerAnimations.Dash, 0.1f);
                }
                else if (PlayerMovement.isFalling || PlayerMovement.isFastFalling || PlayerMovement.isDashFastFalling || PlayerMovement.isPastApex)
                {
                    PlayerAnimations.CrossFade(animator, PlayerAnimations.Fall, 0.1f);
                }
                else if (PlayerMovement.isJumping)
                {
                    PlayerAnimations.CrossFade(animator, PlayerAnimations.Jump, 0.1f);
                }
            }
        }
        // if (isJumping)
        // {
        //     animator.SetBool("Jumping", true);
        // }
        // else
        // {
        //     animator.SetBool("Jumping", false);
        // }
        // if (isFalling || isFastFalling || isPastApex)
        // {
        //     Debug.Log("Falling");
        //     animator.SetBool("Falling", true);
        // }
        // else
        // {
        //     animator.SetBool("Falling", false);
        // }
    }
}
