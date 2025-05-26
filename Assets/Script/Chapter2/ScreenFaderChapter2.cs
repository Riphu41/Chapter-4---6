using UnityEngine;
using System.Collections;

public class ScreenFaderChapter2 : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 0.5f;

    void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    public IEnumerator FadeOut()
    {
        canvasGroup.blocksRaycasts = true;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn()
    {
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime / fadeDuration;
            canvasGroup.alpha = Mathf.Clamp01(t);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
