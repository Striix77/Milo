using System.Collections;
using UnityEngine;

public class FrostBite : MonoBehaviour
{
    [Header("Fade Settings")]
    [Range(1f, 2f)] public float fadeDuration = 1.5f;
    [Range(0f, 1f)] public float fadeDelay = 1f;
    public bool fadeOutOnStart = true;
    public bool destroyWhenFaded = true;

    private SpriteRenderer spriteRenderer;
    private Renderer[] childRenderers;

    private bool isFading = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        childRenderers = GetComponentsInChildren<Renderer>();

        if (fadeOutOnStart)
        {
            DelayedFadeOut();
        }
    }

    public void FadeOut()
    {
        if (isFading) return;
        StartCoroutine(nameof(FadeOutCoroutine), 0);
    }

    public void DelayedFadeOut()
    {
        StartCoroutine(nameof(DelayedFadeOutCoroutine));
    }

    private IEnumerator DelayedFadeOutCoroutine()
    {
        yield return new WaitForSeconds(fadeDelay);
        FadeOut();
    }

    private IEnumerator FadeOutCoroutine(float targetAlpha)
    {
        isFading = true;

        float startAlpha = 0;
        if (spriteRenderer != null)
        {
            startAlpha = spriteRenderer.color.a;
        }
        else if (childRenderers.Length > 0)
        {
            startAlpha = childRenderers[0].material.color.a;
        }

        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);

            SetAlpha(newAlpha);

            yield return null;
        }

        SetAlpha(targetAlpha);

        isFading = false;

        if (destroyWhenFaded && targetAlpha == 0)
        {
            Destroy(gameObject);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        foreach (Renderer renderer in childRenderers)
        {
            Color color = renderer.material.color;
            color.a = alpha;
            renderer.material.color = color;
        }
    }

}
