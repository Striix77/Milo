using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneAnimator : MonoBehaviour
{
    private int cloud = Animator.StringToHash("HurricaneCloud");
    private int form = Animator.StringToHash("HurricaneForm");
    private int loop = Animator.StringToHash("HurricaneLoop");
    private int stop = Animator.StringToHash("HurricaneStop");

    private Animator animator;
    private AnimationClip formClip;

    public bool canDamage = false;

    [Range(0.0f, 3.0f)] public float delayBeforeForm = 3f;
    [Range(0.0f, 3.0f)] public float crossfadeDuration = 0.5f;

    [Range(0.5f, 3.0f)] public float fadeOutDuration = 1.0f;
    public bool destroyAfterFade = true;

    private SpriteRenderer spriteRenderer;
    private Renderer[] renderers;
    private Dictionary<string, AudioSource> audioSources = new Dictionary<string, AudioSource>();

    public ParticleSystem HurricaneParticles;

    void Start()
    {
        canDamage = false;
        animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        foreach (AudioSource source in GetComponents<AudioSource>())
        {
            audioSources[source.clip != null ? source.clip.name : "unnamed"] = source;
        }

        PlaySound("MiloHurricaneV2");
        PlaySound("Hurricane");

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "HurricaneForm")
            {
                formClip = clip;
                break;
            }
        }

        StartCoroutine(nameof(DelayedFormAnimation));
    }

    public void PlaySound(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].Play();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found on Hurricane");
        }
    }

    public void StopSound(string soundName)
    {
        if (audioSources.ContainsKey(soundName))
        {
            audioSources[soundName].Stop();
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found on Hurricane");
        }
    }

    private IEnumerator DelayedFormAnimation()
    {
        canDamage = false;
        yield return new WaitForSeconds(delayBeforeForm);
        canDamage = true;
        animator.CrossFade(form, crossfadeDuration);
        if (formClip != null)
        {
            yield return new WaitForSeconds(formClip.length - crossfadeDuration);
        }
        else
        {
            yield return new WaitForSeconds(1.0f);
        }
        animator.CrossFade(loop, crossfadeDuration);
        ParticleSystem particles = Instantiate(HurricaneParticles, transform.position, Quaternion.identity);
        particles.transform.SetParent(transform);
        particles.transform.localPosition = Vector3.zero;
        particles.Play();
    }

    public void StopHurricane()
    {
        animator.CrossFade(stop, crossfadeDuration);
        StartCoroutine(nameof(FadeOut));
    }

    private IEnumerator FadeOut()
    {

        Color[] originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            originalColors[i] = renderers[i].material.color;
        }


        float elapsedTime = 0;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / fadeOutDuration;
            float alpha = Mathf.Lerp(1f, 0f, normalizedTime);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }

            foreach (Renderer renderer in renderers)
            {
                Color color = renderer.material.color;
                color.a = alpha;
                renderer.material.color = color;
            }

            yield return null;
        }

        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a = 0f;
            renderer.material.color = color;
        }

        if (destroyAfterFade)
        {
            Destroy(gameObject, 0.5f);
        }

    }
}
