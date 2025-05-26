using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BackgroundUIControllerChapter2 : MonoBehaviour
{
    public Image backgroundImage;
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1f;

    [System.Serializable]
    public class BackgroundEntry
    {
        public BackgroundType tag;
        public Sprite sprite;
    }

    public List<BackgroundEntry> backgrounds;

    public void SetBackgroundByTag(BackgroundType tag)
    {
        Sprite target = backgrounds.Find(b => b.tag == tag)?.sprite;

        if (target != null)
        {
            StartCoroutine(FadeTo(target));
        }
        else
        {
            Debug.LogWarning("Tag background tidak ditemukan: " + tag);
        }
    }

    IEnumerator FadeTo(Sprite newSprite)
    {
        // Fade out
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1 - (t / fadeDuration);
            yield return null;
        }

        backgroundImage.sprite = newSprite;

        // Fade in
        t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = (t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }
}

