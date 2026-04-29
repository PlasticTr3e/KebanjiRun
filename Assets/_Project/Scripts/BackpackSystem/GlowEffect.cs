using UnityEngine;
using System.Collections;

public class GlowEffect : MonoBehaviour
{
    public enum GlowColor { Green, Red, Default }

    [Header("Glow Settings")]
    [Tooltip("Durasi efek glow dalam detik")]
    public float glowDuration = 1.5f;

    [Tooltip("Intensitas cahaya glow (0 = mati, 2 = terang)")]
    [Range(0f, 3f)]
    public float glowIntensity = 1.5f;

    [Tooltip("Aktifkan animasi pulse (berkedip perlahan)")]
    public bool enablePulse = true;

    [Header("Glow Colors")]
    public Color greenGlowColor = new Color(0f, 1f, 0.3f);  
    public Color redGlowColor   = new Color(1f, 0.1f, 0.1f); 

    private Renderer[] renderers;      
    private Material[] glowMaterials;  
    private Coroutine glowCoroutine;   
    private Color originalEmission;    
    private bool isGlowing = false;

    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        glowMaterials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            glowMaterials[i] = new Material(renderers[i].material);
            renderers[i].material = glowMaterials[i];

            glowMaterials[i].EnableKeyword("_EMISSION");
        }

        if (glowMaterials.Length > 0)
            originalEmission = glowMaterials[0].GetColor(EmissionColor);
    }

    public void ShowGlow(GlowColor color)
    {
        if (glowCoroutine != null)
            StopCoroutine(glowCoroutine);

        Color targetColor = color switch
        {
            GlowColor.Green => greenGlowColor,
            GlowColor.Red   => redGlowColor,
            _               => Color.white
        };

        glowCoroutine = StartCoroutine(GlowCoroutine(targetColor));
    }

    private IEnumerator GlowCoroutine(Color targetColor)
    {
        isGlowing = true;
        float elapsed = 0f;
        float halfDuration = glowDuration * 0.5f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            Color currentColor = Color.Lerp(Color.black, targetColor * glowIntensity, t);
            SetEmissionColor(currentColor);
            yield return null; 
        }

        elapsed = 0f;

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

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            Color currentColor = Color.Lerp(targetColor * glowIntensity, Color.black, t);
            SetEmissionColor(currentColor);
            yield return null;
        }
        SetEmissionColor(originalEmission);
        isGlowing = false;
        glowCoroutine = null;
    }

    private void SetEmissionColor(Color color)
    {
        foreach (var mat in glowMaterials)
        {
            if (mat != null)
                mat.SetColor(EmissionColor, color);
        }
    }

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

    void OnDestroy()
    {
        foreach (var mat in glowMaterials)
        {
            if (mat != null)
                Destroy(mat); 
        }
    }
}
