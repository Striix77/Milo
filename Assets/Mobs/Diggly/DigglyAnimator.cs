using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DigglyAnimator : MonoBehaviour
{
    private int appearAnimation = Animator.StringToHash("DigglyAppear");
    private int attackAnimation = Animator.StringToHash("DigglyAttack");
    private int disappearAnimation = Animator.StringToHash("DigglyDisappear");


    public float disappearDelay = 1.0f;
    private float crossfadeDuration = 0.1f;

    private Animator animator;
    private DigglyAttack digglyAttack;
    private SpriteRenderer spriteRenderer;
    private AnimationClip disappearClip;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        digglyAttack = GetComponent<DigglyAttack>();
    }

    private void Start()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "DigglyDisappear")
            {
                disappearClip = clip;
                break;
            }
        }

    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash == attackAnimation && !digglyAttack.isAttacking)
        {
            digglyAttack.isAttacking = true;
            StartCoroutine(nameof(CrossfadeToDisappearAnimation));
        }
    }

    private IEnumerator CrossfadeToDisappearAnimation()
    {
        yield return new WaitForSeconds(disappearDelay);
        Debug.Log("Crossfade to disappear animation");
        animator.CrossFade(disappearAnimation, crossfadeDuration);
        yield return new WaitForSeconds(disappearClip.length - crossfadeDuration);
        spriteRenderer.enabled = false;
    }

    public void PlayAppearAnimation()
    {
        spriteRenderer.enabled = true;
        animator.CrossFade(appearAnimation, crossfadeDuration);
    }

}
