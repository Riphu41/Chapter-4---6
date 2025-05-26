using UnityEngine;

public class BackgroundSpriteSwitcher : MonoBehaviour
{
    public GameObject backgroundToko;
    public GameObject backgroundKamar;

    private string lastSpriteName = "";

    void Update()
    {
        GameObject quad = GameObject.Find("BackgroundQuad");
        if (quad == null || quad.GetComponent<Renderer>() == null) return;

        var tex = quad.GetComponent<Renderer>().material.mainTexture;
        if (tex == null) return;

        string currentSpriteName = tex.name;

        if (currentSpriteName != lastSpriteName)
        {
            lastSpriteName = currentSpriteName;

            // Debug.Log untuk lihat perubahan
            Debug.Log("Background aktif: " + currentSpriteName);

            if (backgroundToko) backgroundToko.SetActive(currentSpriteName.Contains("Toko"));
            if (backgroundKamar) backgroundKamar.SetActive(currentSpriteName.Contains("Kamar"));
        }
    }
}
