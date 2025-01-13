using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.VFX;

/// <summary>
/// Manager to decrease the speedlines radius and to increase the intensity of
/// chromatic abberation
/// </summary>

public class VFXManager : MonoBehaviour
{
    [SerializeField] private VisualEffect speedLines;
    [SerializeField] private Volume postProcessVolume;
    
    
    private GameManager _gameManager;
    private ChromaticAberration _chromaticAberration;
    
    [SerializeField] private float aberrationStart = 20f;  // Score at which Chromatic Aberration begins to increase
    [SerializeField] private float maxScore = 50f;         // Max score to clamp at
    [SerializeField] private float minRadius = 7.5f;       // Min radius when score is high
    [SerializeField] private float maxRadius = 13f;        // Max radius when score is 0
    
    void Start()
    {
        _gameManager = FindFirstObjectByType<GameManager>();
        
        // Extract Chromatic Aberration from the Post Process Volume
        if (postProcessVolume.profile.TryGet<ChromaticAberration>(out var chromatic))
        {
            _chromaticAberration = chromatic;
        }
        else
        {
            Debug.LogWarning("Chromatic Aberration not found in Post Process Volume.");
        }
    }

    void Update()
    {
        EffectsFromPoints(_gameManager.totalScore);
    }

    void EffectsFromPoints(int points)
    {
        // Clamp score between 0 and MaxScore
        float clampedPoints = Mathf.Clamp(points, 0, maxScore);

        // Interpolate Speed Lines Radius
        float radius = Mathf.Lerp(maxRadius, minRadius, clampedPoints / maxScore);
        speedLines.SetFloat("Radius", radius);  // Assumes the property is named "Radius"

        // Interpolate Chromatic Aberration Intensity
        if (_chromaticAberration != null)
        {
            if (clampedPoints >= aberrationStart)
            {
                // Remap score from aberrationStart to maxScore between 0 and 1
                float t = Mathf.InverseLerp(aberrationStart, maxScore, clampedPoints);
                _chromaticAberration.intensity.value = Mathf.Lerp(0f, 1f, t);
            }
            else
            {
                // Before reaching the threshold, intensity remains at 0
                _chromaticAberration.intensity.value = 0f;
            }
        }
    }
}
