using System;
using System.Collections;
using Reactional.Experimental;
using Reactional.Playback;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Reactional_DeepAnalysis_GlobalLightControler : MonoBehaviour
{
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
        Debug.Log($"PostProcessBassEffect triggered for bass note with duration {offset}");
        StartCoroutine(lerpIntensity(INTENSITY));
    }

    public const float INTENSITY = 0.05f;
    
    private IEnumerator lerpIntensity(float StartIntensity)
    {

        float duration = 0.5f;
        float halfDuration = duration / 2.0f;
        float maxIntensity = 1;
        var elapsedTime = 0f;
        
        Debug.Log("Start Value Intensity " +StartIntensity + "and GL intesity " + globalLight.intensity);
        
        while (elapsedTime <= halfDuration)
        {
            globalLight.intensity = Mathf.Lerp(StartIntensity, maxIntensity, elapsedTime / halfDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
  
        
        elapsedTime = 0f;
        while (elapsedTime <= halfDuration)
        {
            globalLight.intensity = Mathf.Lerp(maxIntensity, StartIntensity, elapsedTime / halfDuration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Debug.Log("End Value Intensity " +StartIntensity + "and GL intesity " + globalLight.intensity);
    }
   
}
