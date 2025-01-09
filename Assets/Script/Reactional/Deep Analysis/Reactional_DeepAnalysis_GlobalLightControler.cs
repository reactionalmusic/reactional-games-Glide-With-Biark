using System;
using System.Collections;
using Reactional.Experimental;
using Reactional.Playback;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
/// <summary>
/// Subscribing to the Reactional Deep Analysis Event Dispatcher Instruments
/// And taking a Global light and alter the intensity value from the set editor value to a new value.
/// The value lerps from startvalue to MAX_INTENSITY 
/// </summary>
public class Reactional_DeepAnalysis_GlobalLightControler : MonoBehaviour
{
   
    [SerializeField] private float MAX_INTENSITY = 1.5f;
    [SerializeField] private float MIN_INTENSITY = 0.2f;
    
    public Light2D globalLight;
    void OnEnable()
    {
        // Subscriba på eventen
        //Reactional_DeepAnalysis_EventDispatcher.OnVocalNoteHit += HandleVocalNoteHit;
        //Reactional_DeepAnalysis_EventDispatcher.OnBassNoteHit += PostProcessBasEffect;
        Reactional_DeepAnalysis_EventDispatcher.OnDrumNoteHit += PostProcessDrumEffect;
    }

    void OnDisable()
    {
        // Unsubscriba från eventen för att undvika memory leaks
        //Reactional_DeepAnalysis_EventDispatcher.OnVocalNoteHit -= HandleVocalNoteHit;
       // Reactional_DeepAnalysis_EventDispatcher.OnBassNoteHit -= PostProcessBasEffect;
        Reactional_DeepAnalysis_EventDispatcher.OnDrumNoteHit -= PostProcessDrumEffect;
    }

    void PostProcessDrumEffect(float offset, drums bass)
    {
        //Debug.Log($"PostProcessBassEffect triggered for bass note with duration {offset}");
        StartCoroutine(lerpIntensity(globalLight.intensity));
    }

    
    private IEnumerator lerpIntensity(float StartIntensity)
    {

        float duration = 0.5f;
        float halfDuration = duration / 2.0f;
        //float maxIntensity = 5;
        var elapsedTime = 0f;
        
        //Debug.Log("Start Value Intensity " +StartIntensity + "and GL intesity " + globalLight.intensity);
        
        while (elapsedTime <= halfDuration)
        {
            globalLight.intensity = Mathf.Lerp(StartIntensity, MAX_INTENSITY, elapsedTime / halfDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
  
        
        elapsedTime = 0f;
        while (elapsedTime <= halfDuration)
        {
            globalLight.intensity = Mathf.Lerp(MAX_INTENSITY, MIN_INTENSITY, elapsedTime / halfDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        //Debug.Log("End Value Intensity " +StartIntensity + "and GL intesity " + globalLight.intensity);
    }
   
}
