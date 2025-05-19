using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Buttons : MonoBehaviour
{
    private TextMeshProUGUI buttonText;

    public void StartFade()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            StartCoroutine(FadeTextIn());
        }
    }

    public void StopFade()
    {
        if (buttonText != null)
        {
            StartCoroutine(FadeTextOut());
        }
    }

    private IEnumerator FadeTextIn()
    {
        Color textColor = buttonText.color;
        textColor.a = 0f;
        buttonText.color = textColor;

        while (textColor.a < 1f)
        {
            textColor.a += Time.deltaTime * 3.5f;
            buttonText.color = textColor;
            yield return null;
        }
    }

    private IEnumerator FadeTextOut()
    {
        Color textColor = buttonText.color;
        textColor.a = 1f;
        buttonText.color = textColor;

        while (textColor.a > 0f)
        {
            textColor.a -= Time.deltaTime * 3.5f;
            buttonText.color = textColor;
            yield return null;
        }
    }

    public void MakeButtonGolden()
    {
        Image buttonImage = GetComponent<Image>();
        if (buttonImage != null)
        {
            Color goldenColor = new Color(0.4886792f, 1f, 0.5158593f);
            buttonImage.color = goldenColor;
        }
    }
}
