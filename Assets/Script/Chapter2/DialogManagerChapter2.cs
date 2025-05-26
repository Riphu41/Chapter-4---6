using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogSystemChapter2 : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public GameObject dialogUI;
    public Button nextButton;

    [Header("Text File")]
    public TextAsset dialogFile;

    [Header("Cutscene List")]
    public List<CutsceneEntry> cutscenes;

    [Header("Kamera Dinamis")]
    public Transform cameraTransform;
    public float cameraMoveSpeed = 3f;
    public List<CameraPoint> cameraPoints;

    [Header("Fade")]
    public Transform canvasTransform;
    public ScreenFader screenFader;

    [Header("Background")]
    public SpriteRenderer backgroundRenderer;
    public List<BackgroundEntry> backgroundByTag;

    [Header("Animator")]
    public Animator aislinAnimator;
    public Animator grozarAnimator;
    public Animator felixAnimator;

    [System.Serializable]
    public class CutsceneEntry
    {
        public string tag;
        public VideoPlayer videoPlayer;
    }

    [System.Serializable]
    public class CameraPoint
    {
        public int dialogIndex;
        public Transform targetTransform;
    }

    [System.Serializable]
    public class BackgroundEntry
    {
        public BackgroundType tag;
        public Sprite sprite;
    }

    private List<string> lines = new List<string>();
    private int currentLine = 0;
    private bool isCutscenePlaying = false;
    private Transform currentCamTarget;

    void Start()
    {
        Debug.Log("[DialogSystem] Memulai sistem dialog...");

        if (screenFader == null)
            CreateFadePanel();

        LoadDialog();

        if (lines.Count > 0 && lines[0].StartsWith("#"))
        {
            string tag = lines[0].Replace("#", "").Trim();
            Debug.Log($"[DialogSystem] Baris pertama adalah cutscene: {tag}");
            PlayCutscene(tag);
            currentLine++;
        }
        else
        {
            NextLine();
        }

        if (nextButton != null)
            nextButton.onClick.AddListener(OnNextButtonClicked);
    }

    void Update()
    {
        if (isCutscenePlaying) return;

        if (currentCamTarget != null)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, currentCamTarget.position, Time.deltaTime * cameraMoveSpeed);
            cameraTransform.rotation = Quaternion.Lerp(cameraTransform.rotation, currentCamTarget.rotation, Time.deltaTime * cameraMoveSpeed);
        }

        // 🔥 Manual tes animasi Talk2 untuk Felix
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("[Manual Trigger] Menjalankan Talk2");
            if (felixAnimator != null)
                felixAnimator.SetTrigger("Talk2");
        }
    }

    public void OnNextButtonClicked()
    {
        if (isCutscenePlaying) return;

        currentLine++;
        if (currentLine < lines.Count)
            NextLine();
        else
            EndDialog();
    }

    void LoadDialog()
    {
        string[] rawLines = dialogFile.text.Split('\n');
        foreach (var raw in rawLines)
        {
            string trimmed = raw.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                lines.Add(trimmed);
        }
        Debug.Log($"[DialogSystem] {lines.Count} baris dialog berhasil dimuat.");
    }

    void NextLine()
    {
        string line = lines[currentLine];
        Debug.Log($"[DialogSystem] Menampilkan baris dialog ke-{currentLine}: {line}");

        if (line.StartsWith("#"))
        {
            string tag = line.Replace("#", "").Trim();
            Debug.Log($"[DialogSystem] Mencoba memutar cutscene dengan tag: {tag}");
            PlayCutscene(tag);
            currentLine++;
            if (currentLine < lines.Count)
                NextLine();
            else
                EndDialog();
            return;
        }

        string[] parts = line.Split('|');
        if (parts.Length >= 2)
        {
            string speaker = parts[0].Trim();
            string sentence = parts[1].Trim();
            nameText.text = speaker;
            dialogText.text = sentence;

            if (parts.Length >= 3)
            {
                string bgTagStr = parts[2].Trim();
                if (System.Enum.TryParse(bgTagStr, out BackgroundType bgTag))
                {
                    ApplyBackgroundByTag(bgTag);
                }
            }

            if (parts.Length >= 4)
            {
                string animTrigger = parts[3].Trim();
                SetCharacterAnimation(speaker, animTrigger);
                Debug.Log("ppp");
            }
        }

        TryMoveCamera(currentLine);
    }

    void EndDialog()
    {
        dialogUI.SetActive(false);
        Debug.Log("[DialogSystem] Dialog selesai.");
    }

    void PlayCutscene(string tag)
    {
        foreach (var entry in cutscenes)
        {
            Debug.Log($"[DialogSystem] Mengecek entry tag '{entry.tag}'");

            if (entry.tag == tag && entry.videoPlayer != null)
            {
                Debug.Log("[DialogSystem] Menyiapkan cutscene dengan delay untuk diputar: " + entry.videoPlayer.clip?.name);
                entry.videoPlayer.gameObject.SetActive(true);
                StartCoroutine(PlayCutsceneDelayed(entry.videoPlayer));
                isCutscenePlaying = true;
                dialogUI.SetActive(false);
                return;
            }
        }

        Debug.LogWarning($"[DialogSystem] Cutscene dengan tag '{tag}' tidak ditemukan.");
        currentLine++;
        if (currentLine < lines.Count)
            NextLine();
        else
            EndDialog();
    }

    IEnumerator PlayCutsceneDelayed(VideoPlayer vp)
    {
        yield return null;
        vp.Play();
        Debug.Log("[DialogSystem] VideoPlayer diputar setelah 1 frame delay.");
        vp.loopPointReached += OnCutsceneEnd;
    }

    void OnCutsceneEnd(VideoPlayer vp)
    {
        vp.loopPointReached -= OnCutsceneEnd;
        vp.gameObject.SetActive(false);
        dialogUI.SetActive(true);
        isCutscenePlaying = false;

        Debug.Log("[DialogSystem] Cutscene selesai, melanjutkan dialog...");

        currentLine++;
        if (currentLine < lines.Count)
            NextLine();
        else
            EndDialog();
    }

    void TryMoveCamera(int index)
    {
        foreach (var point in cameraPoints)
        {
            if (point.dialogIndex == index)
            {
                Debug.Log($"[DialogSystem] Memindahkan kamera ke dialog index: {index}");
                StartCoroutine(SmoothCameraTransition(point.targetTransform));
                return;
            }
        }

        Debug.Log($"[DialogSystem] Tidak ada kamera point untuk index {index}.");
    }

    IEnumerator SmoothCameraTransition(Transform target)
    {
        if (screenFader != null)
        {
            screenFader.gameObject.SetActive(true);
            yield return StartCoroutine(screenFader.FadeOut());
        }

        cameraTransform.position = target.position;
        cameraTransform.rotation = target.rotation;
        currentCamTarget = target;

        if (screenFader != null)
            yield return StartCoroutine(screenFader.FadeIn());
    }

    void CreateFadePanel()
    {
        GameObject fadeGO = new GameObject("FadePanel");
        fadeGO.transform.SetParent(canvasTransform, false);

        RectTransform rt = fadeGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = fadeGO.AddComponent<Image>();
        img.color = Color.black;
        img.raycastTarget = false;

        CanvasGroup cg = fadeGO.AddComponent<CanvasGroup>();
        ScreenFader fader = fadeGO.AddComponent<ScreenFader>();
        fader.canvasGroup = cg;

        screenFader = fader;
        screenFader.gameObject.SetActive(false);
    }

    void ApplyBackgroundByTag(BackgroundType tag)
    {
        foreach (var entry in backgroundByTag)
        {
            if (entry.tag == tag && backgroundRenderer != null)
            {
                backgroundRenderer.sprite = entry.sprite;
                Debug.Log($"[DialogSystem] Background diganti pakai tag: {tag}");
                return;
            }
        }

        Debug.LogWarning($"[DialogSystem] Tag background '{tag}' tidak ditemukan.");
    }

    void SetCharacterAnimation(string speaker, string trigger)
    {
        if (speaker == "Aislin" && aislinAnimator != null)
        {
            aislinAnimator.SetTrigger(trigger);
        }
        else if (speaker == "Grozar" && grozarAnimator != null)
        {
            grozarAnimator.SetTrigger(trigger);
        }
        else if (speaker == "Felix" && felixAnimator != null)
        {
            felixAnimator.SetTrigger(trigger);
        }
    }
}
