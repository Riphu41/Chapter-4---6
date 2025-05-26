using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour
{
    [System.Serializable]
    public class BackgroundEntry
    {
        public BackgroundType tag;
        public Sprite sprite;
    }

    public SpriteRenderer backgroundRenderer;
    public List<BackgroundEntry> backgroundByTag;

    public void ChangeBackground(BackgroundType tag)
    {
        Debug.Log($"[BackgroundManager] Permintaan ganti background ke tag: {tag}");

        var entry = backgroundByTag.Find(b => b.tag == tag);

        if (entry != null)
        {
            if (entry.sprite != null && backgroundRenderer != null)
            {
                backgroundRenderer.sprite = entry.sprite;
                Debug.Log($"[BackgroundManager] ✅ Background berhasil diganti ke: {tag} dengan sprite: {entry.sprite.name}");
            }
            else
            {
                Debug.LogWarning($"[BackgroundManager] ⚠️ Sprite null atau SpriteRenderer belum di-assign. Tag: {tag}");
            }
        }
        else
        {
            Debug.LogWarning($"[BackgroundManager] ❌ Tag '{tag}' tidak ditemukan di list!");
        }
    }
}
