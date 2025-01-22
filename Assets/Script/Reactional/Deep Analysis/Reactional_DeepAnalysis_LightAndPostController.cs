using System;
using System.Collections;
using Reactional.Experimental;
using Reactional.Playback;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Subscribing to the Reactional Deep Analysis Event Dispatcher Instruments
/// And taking a Global light and alter the intensity value from the set editor value to a new value.
/// The value lerps from startvalue to MAX_INTENSITY
/// Also Alters the Chromatic Abboration value on the Post Process.
/// </summary>
public class Reactional_DeepAnalysis_LightAndPostController : MonoBehaviour
{
    [Header("Global Light")]
    
    [SerializeField] private Light2D globalLight;
    [SerializeField] private float MAX_GLOBALLIGHT_INTENSITY = 1.5f;
    [SerializeField] private float MIN_GLOBALLIGHT_INTENSITY = 0.2f;
    
    [Header("Post Process")]
    
    [SerializeField] private Volume postprocess;
    [SerializeField] private float MIN_CHROMATICABBERATION = 0f;
    [SerializeField] private float MAX_CHROMATICABBERATION = 1f;
    [SerializeField] private float scoreActivationTreshhold = 5f;
    
    private Bloom bloom;
    private ChromaticAberration chromaticAberration;
    private GameManager _gameManager;


    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        postprocess.profile.TryGet(out bloom);
        postprocess.profile.TryGet(out chromaticAberration);
        chromaticAberration.intensity.overrideState = true;
        chromaticAberration.intensity.value = 0f;
    }

    private void OnEnable()
    {
        //Reactional_DeepAnalysis_EventDispatcher.OnVocalNoteHit += HandleVocalNoteHit;
        //Reactional_DeepAnalysis_EventDispatcher.OnBassNoteHit += PostProcessBasEffect;
        Reactional_DeepAnalysis_EventDispatcher.OnDrumNoteHit += PostProcessDrumEffect;
    }

    private void OnDisable()
    {
        //Reactional_DeepAnalysis_EventDispatcher.OnVocalNoteHit -= HandleVocalNoteHit;
        // Reactional_DeepAnalysis_EventDispatcher.OnBassNoteHit -= PostProcessBasEffect;
        Reactional_DeepAnalysis_EventDispatcher.OnDrumNoteHit -= PostProcessDrumEffect;
    }

    private void PostProcessDrumEffect(float offset, drums bass)
    {
        //Debug.Log($"PostProcessBassEffect triggered for bass note with duration {offset}");
        StartCoroutine(lerpIntensity(globalLight.intensity));
    }


    private IEnumerator lerpIntensity(float StartIntensity)
    {
        var duration = 0.5f;
        var halfDuration = duration / 2.0f;
        var elapsedTime = 0f;

        //Debug.Log("Start Value Intensity " +StartIntensity + "and GL intesity " + globalLight.intensity);


        while (elapsedTime <= halfDuration)
        {
            globalLight.intensity = Mathf.Lerp(StartIntensity, MAX_GLOBALLIGHT_INTENSITY, elapsedTime / halfDuration);
            if (_gameManager.totalScore >= scoreActivationTreshhold)
            {  //Debug.Log("ActivateChrommaticAberration");
                chromaticAberration.intensity.value = Mathf.Lerp(MAX_CHROMATICABBERATION, MIN_CHROMATICABBERATION,
                    elapsedTime / halfDuration);
            }


            elapsedTime += Time.deltaTime;
            yield return null;
        }


        elapsedTime = 0f;
        while (elapsedTime <= halfDuration)
        {
            globalLight.intensity = Mathf.Lerp(MAX_GLOBALLIGHT_INTENSITY, MIN_GLOBALLIGHT_INTENSITY,
                elapsedTime / halfDuration);

            if (_gameManager.totalScore >= scoreActivationTreshhold)
            {
                chromaticAberration.intensity.value = Mathf.Lerp(MIN_CHROMATICABBERATION, MAX_CHROMATICABBERATION,
                    elapsedTime / halfDuration);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("End Value Intensity " +StartIntensity + "and GL intesity " + globalLight.intensity);
    }
}
