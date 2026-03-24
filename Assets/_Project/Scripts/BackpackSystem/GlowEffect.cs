// ─────────────────────────────────────────────────────────────────
// FILE: GlowEffect.cs
// Deskripsi: Mengatur efek glow (emission) pada item evakuasi
// Cara pakai: Attach ke setiap item, sudah di-call dari ItemPickup.cs
// ─────────────────────────────────────────────────────────────────
using UnityEngine;
using System.Collections;

public class GlowEffect : MonoBehaviour
{
    // ── Enum: Warna Glow yang tersedia ────────────────────────────
    public enum GlowColor { Green, Red, Default }

    // ── Inspector Settings ────────────────────────────────────────
    [Header("Glow Settings")]
    [Tooltip("Durasi efek glow dalam detik")]
    public float glowDuration = 1.5f;

    [Tooltip("Intensitas cahaya glow (0 = mati, 2 = terang)")]
    [Range(0f, 3f)]
    public float glowIntensity = 1.5f;

    [Tooltip("Aktifkan animasi pulse (berkedip perlahan)")]
    public bool enablePulse = true;

    // ── Warna Glow (bisa diubah di Inspector) ─────────────────────
    [Header("Glow Colors")]
    public Color greenGlowColor = new Color(0f, 1f, 0.3f);   // Hijau
    public Color redGlowColor   = new Color(1f, 0.1f, 0.1f); // Merah

    // ── Private Variables ─────────────────────────────────────────
    private Renderer[] renderers;       // Semua mesh renderer di item ini
    private Material[] glowMaterials;   // Material yang dimodifikasi
    private Coroutine glowCoroutine;    // Coroutine untuk animasi glow
    private Color originalEmission;     // Warna emission awal
    private bool isGlowing = false;

    // ── Keyword Emission untuk URP/Standard shader ────────────────
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    // ── Awake: Dijalankan sebelum Start ───────────────────────────
    void Awake()
    {
        // Ambil semua Renderer (termasuk child objects)
        renderers = GetComponentsInChildren<Renderer>();

        // Buat salinan material (PENTING: jangan ubah material asli!)
        // Kalau tidak dibuat salinan, semua item yang share material ikut berubah
        glowMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            glowMaterials[i] = new Material(renderers[i].material);
            renderers[i].material = glowMaterials[i];

            // Enable emission keyword
            glowMaterials[i].EnableKeyword("_EMISSION");
        }

        // Simpan warna emission awal (biasanya hitam / mati)
        if (glowMaterials.Length > 0)
            originalEmission = glowMaterials[0].GetColor(EmissionColor);
    }

    // ── ShowGlow: Panggil dari script lain untuk aktifkan glow ─────
    // Contoh: glowEffect.ShowGlow(GlowEffect.GlowColor.Green);
    public void ShowGlow(GlowColor color)
    {
        // Hentikan glow yang sedang berjalan (jika ada)
        if (glowCoroutine != null)
            StopCoroutine(glowCoroutine);

        // Tentukan warna berdasarkan parameter
        Color targetColor = color switch
        {
            GlowColor.Green => greenGlowColor,
            GlowColor.Red   => redGlowColor,
            _               => Color.white
        };

        // Mulai animasi glow
        glowCoroutine = StartCoroutine(GlowCoroutine(targetColor));
    }

    // ── GlowCoroutine: Animasi glow (fade in → tahan → fade out) ──
    private IEnumerator GlowCoroutine(Color targetColor)
    {
        isGlowing = true;
        float elapsed = 0f;
        float halfDuration = glowDuration * 0.5f;

        // FASE 1: Fade IN (dari gelap ke terang)
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            Color currentColor = Color.Lerp(Color.black, targetColor * glowIntensity, t);
            SetEmissionColor(currentColor);
            yield return null;  // tunggu 1 frame
        }

        // Reset untuk fase berikutnya
        elapsed = 0f;

        // FASE 2 (Opsional): Pulse / berkedip saat di puncak
        if (enablePulse)
        {
            float pulseTime = 0.3f;
            while (elapsed < pulseTime)
            {
                elapsed += Time.deltaTime;
                float pulse = 0.8f + 0.2f * Mathf.Sin(elapsed * Mathf.PI / pulseTime);
                SetEmissionColor(targetColor * glowIntensity * pulse);
                yield return null;
            }
            elapsed = 0f;
        }

        // FASE 3: Fade OUT (dari terang ke gelap)
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            Color currentColor = Color.Lerp(targetColor * glowIntensity, Color.black, t);
            SetEmissionColor(currentColor);
            yield return null;
        }

        // Kembalikan ke state awal
        SetEmissionColor(originalEmission);
        isGlowing = false;
        glowCoroutine = null;
    }

    // ── SetEmissionColor: Update warna di semua material ──────────
    private void SetEmissionColor(Color color)
    {
        foreach (var mat in glowMaterials)
        {
            if (mat != null)
                mat.SetColor(EmissionColor, color);
        }
    }

    // ── StopGlow: Matikan glow seketika ───────────────────────────
    public void StopGlow()
    {
        if (glowCoroutine != null)
        {
            StopCoroutine(glowCoroutine);
            glowCoroutine = null;
        }
        SetEmissionColor(originalEmission);
        isGlowing = false;
    }

    // ── OnDestroy: Bersihkan material salinan saat object dihancurkan
    void OnDestroy()
    {
        foreach (var mat in glowMaterials)
        {
            if (mat != null)
                Destroy(mat);  // PENTING: hindari memory leak
        }
    }
}
