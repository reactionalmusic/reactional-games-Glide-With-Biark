using System;
using System.Collections.Generic;
using Reactional.Core;
using Reactional.Experimental;
using UnityEngine;
/// <summary>
/// This Script is used to subscribe on the events of instruments analysed with deep analysis
/// When a instrument is playing each note will be sent as an event and if you subscribe to
/// the instrument you can use those notes for ex. generating procedural levels.
/// If more instrument is added in your offlineMusicDataAsset you will need to add similar ActionEvents for that specific instrument here.
/// To Generate a "offlineMusicDataAsset" go to: Tools->Reactional->DeepAnalysis->ExportnalysisData
/// TODO: Make sure that this script is aware of what song and thus what offlineMusicDataAsset should be used,
/// TODO: for now slot one or more in the List in the Inspector.
/// </summary>
public class Reactional_DeepAnalysis_EventDispatcher : MonoBehaviour
{
    
    [Header("Reactional Data Asset")] 
    public OfflineMusicDataAsset offlineMusicDataAsset;
    public List<OfflineMusicDataAsset> offlineMusicDataAssetsList ;

    // Index for Calculating where we are in Real Time Beat Matching for Instruments
    private int _currentVocalIndex;
    private int _currentBassIndex;
    private int _currentDrumIndex;

    // Events for triggering when a note is hit
    public static event Action<float, vocals> OnVocalNoteHit; // Event for when a vocal note is hit
    public static event Action<float, bass> OnBassNoteHit; // Event for when a bass note is hit
    public static event Action<float, drums> OnDrumNoteHit; // Event for when a drum note is hit

   
    private void Update()
    {
        FindVocalNoteHit();
        FindbassNoteHit();
        FindDrumNoteHit();
    }

    /// <summary>
    ///     Unsubscribe from the events when the object is destroyed to avoid memory leaks.
    /// </summary>
    private void FindVocalNoteHit()
    {
        if (!offlineMusicDataAsset) return;

        var currentBeat = ReactionalEngine.Instance.CurrentBeat; // Get current beat

        // Check if there are any remaining vocals to process
        while (_currentVocalIndex < offlineMusicDataAsset.vocals.Count)
        {
            var vocal = offlineMusicDataAsset.vocals[_currentVocalIndex];

            // Check if the current beat matches the note's beat (vocal.offset)
            if (currentBeat >= vocal.offset)
            {
                // Fire the event for this vocal note
                OnVocalNoteHit?.Invoke(vocal.offset, vocal);
                Debug.Log("Vocal Note Hit Purple!" +  vocal.offset);

                // Move to the next note
                _currentVocalIndex++;
            }
            else
            {
                // No match, wait for the right beat
                break;
            }
        }
    }

    private void FindbassNoteHit()
    {
        if (!offlineMusicDataAsset) return;

        var currentBeat = ReactionalEngine.Instance.CurrentBeat; // Get current beat

        // Check if there are any remaining bass to process
        while (_currentBassIndex < offlineMusicDataAsset.bass.Count)
        {
            var bass = offlineMusicDataAsset.bass[_currentBassIndex];

            // Check if the current beat matches the note's beat (bass.offset)
            if (currentBeat >= bass.offset)
            {
                // Fire the event for this vocal note
                OnBassNoteHit?.Invoke(bass.offset, bass);
                // Debug.Log("Bass Note Hit Green!");

                // Move to the next note
                _currentBassIndex++;
            }
            else
            {
                // No match, wait for the right beat
                break;
            }
        }
    }

    private void FindDrumNoteHit()
    {
        if (!offlineMusicDataAsset) return;

        var currentBeat = ReactionalEngine.Instance.CurrentBeat; // Get current beat

        // Check if there are any remaining drums to process
        while (_currentDrumIndex < offlineMusicDataAsset.drums.Count)
        {
            var drums = offlineMusicDataAsset.drums[_currentDrumIndex]; //TODO add +1 to be one step before maybe?

            // Check if the current beat matches the note's beat (drums.offset)
            if (currentBeat >= drums.offset)
            {
                // Fire the event for this vocal note
                OnDrumNoteHit?.Invoke(drums.offset, drums);
                //Debug.Log("Drum Note Hit Blue!");

                // Move to the next note
                _currentDrumIndex++;
            }
            else
            {
                // No match, wait for the right beat
                break;
            }
        }
    }
}