using System.Collections.Generic;
using UnityEngine;

public class CutsceneBGMManager : MonoBehaviour
{
    [System.Serializable]
    public class CutsceneBGMEntry
    {
        public string tag;
        public AudioSource audioSource;
    }

    public List<CutsceneBGMEntry> bgmList;
    public float fadeDuration = 1f;

    private AudioSource currentPlaying;

    public void PlayBGM(string tag)
    {
        StopAllCoroutines();
        StopAllBGMs();

        var entry = bgmList.Find(e => e.tag == tag);
        if (entry != null && entry.audioSource != null)
        {
            currentPlaying = entry.audioSource;
            StartCoroutine(FadeIn(entry.audioSource, fadeDuration));
        }
        else
        {
            Debug.LogWarning($"[BGMManager] Tidak ditemukan BGM dengan tag: {tag}");
        }
    }

    public void StopAllBGMs()
    {
        foreach (var entry in bgmList)
        {
            if (entry.audioSource != null && entry.audioSource.isPlaying)
            {
                entry.audioSource.Stop();
                entry.audioSource.volume = 1f;
            }
        }
    }

    private System.Collections.IEnumerator FadeIn(AudioSource source, float duration)
    {
        source.volume = 0f;
        source.Play();

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        source.volume = 1f;
    }

    public void FadeOutCurrent()
    {
        if (currentPlaying != null)
        {
            StartCoroutine(FadeOut(currentPlaying, fadeDuration));
        }
    }

    private System.Collections.IEnumerator FadeOut(AudioSource source, float duration)
    {
        float startVolume = source.volume;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, timer / duration);
            yield return null;
        }

        source.Stop();
        source.volume = 1f;
    }
}
