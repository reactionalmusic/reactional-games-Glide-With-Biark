using System.Collections.Generic;
using UnityEngine;
using Reactional.Core;
using Reactional.Experimental;
using System;

public class ReactionalInstrumentEventDispatcher : MonoBehaviour
{
    // Public variables for prefabs
    public SongChunker songChunker;
    public GameObject VocalPrefab;
    public GameObject BasPrefab;
    public GameObject DrumPrefab;

    // Events for triggering when a note is hit
    public static event Action<float, vocals> OnVocalNoteHit;   // Event for when a vocal note is hit
    public static event Action<float, bass> OnBassNoteHit;     // Event for when a bass note is hit
    public static event Action<float, drums> OnDrumNoteHit;    // Event for when a drum note is hit

    // Lists to track positions and objects spawned
    private List<float> accumulatedPositions = new List<float>();
    private List<GameObject> accumulatedObjects = new List<GameObject>();
    private List<int> accumulatedEntries = new List<int>();

    private List<float> accumulatedBassPositions = new List<float>();
    private List<GameObject> accumulatedBassObjects = new List<GameObject>();
    private List<int> accumulatedBassEntries = new List<int>();

    private int accumulatedEntry = 0;

    // Controls automatic chunking
    public bool autoChunk = true;
    public OfflineMusicDataAsset offlineMusicDataAsset;

    // Constants for positioning multipliers (replacing hardcoded numbers)
    private const float XOffsetMultiplier = 5f;  // Controls spacing on X-axis
    private const float YOffsetMultiplier = 2f;  // Controls spacing on Y-axis
    private const float YBasePosition = -5f;     // Base Y position for items like drums
    private const float YPitchAdjustment = 4f;   // Adjustment for Y position based on pitch

    /// <summary>
    /// Unity's Start method to initialize and set up events.
    /// </summary>
    void Start()
    {
        // Subscribe the instantiation methods to the events
        OnVocalNoteHit += InstantiateVocalPrefab;
        OnBassNoteHit += InstantiateBassPrefab;
        OnDrumNoteHit += InstantiateDrumPrefab;

        if (autoChunk)
        {
            // Call the methods to trigger the events for each instrument
            SpawnVocals();  // Will trigger OnVocalNoteHit
            SpawnBass();    // Will trigger OnBassNoteHit
            SpawnDrums();   // Will trigger OnDrumNoteHit
            return;
        }

        // TODO: This might be better suited in another script handling chunk management
        foreach (var chunk in songChunker.chunkAssets)
        {
            SpawnChunk(chunk);  // TODO: Consider moving to chunk management logic
        }
    }

    /// <summary>
    /// Unsubscribe from the events when the object is destroyed to avoid memory leaks.
    /// </summary>
    void OnDestroy()
    {
        // Unsubscribe from the events when the object is destroyed
        OnVocalNoteHit -= InstantiateVocalPrefab;
        OnBassNoteHit -= InstantiateBassPrefab;
        OnDrumNoteHit -= InstantiateDrumPrefab;
    }
    
  
    

    /// <summary>
    /// Method to spawn vocals and trigger the OnVocalNoteHit event.
    /// </summary>
    void SpawnVocals()
    {
        float prev_offset = 0;
        float prev_pitch = 0;
        float prev_end = 0;

        foreach (var vocal in offlineMusicDataAsset.vocals)
        {
            float offset = Mathf.Round(vocal.offset * 4) / 4f;  // Round offset to nearest 0.25

            // Skipping logic based on previous vocal's pitch and offset
            if (Mathf.Round(vocal.note) % 12 == prev_pitch % 12 && vocal.offset_seconds + 0.3f < prev_end)
            {
                continue;
            }

            if ((offset % 1 != 0f || offset % 1 != 0.5f) && vocal.duration_seconds < 0.1f)
            {
                continue;  // Skip very short vocals
            }

            if (offset <= prev_offset + 0.25f || offset == prev_offset)
            {
                continue;  // Skip if the offset is too close to the previous one
            }

            prev_offset = offset;
            VocalPrefab.GetComponent<pitchdata>().pitch = vocal.note;

            // Trigger the event for when a vocal note is hit
            OnVocalNoteHit?.Invoke(offset, vocal);
        }
    }

    /// <summary>
    /// Instantiates the vocal prefab when the OnVocalNoteHit event is triggered.
    /// </summary>
    /// <param name="offset">The time offset for when the vocal note is hit.</param>
    /// <param name="vocal">The vocals data associated with the note.</param>
    private void InstantiateVocalPrefab(float offset, vocals vocal)
    {
        Vector3 position = new Vector3(offset * XOffsetMultiplier, GetYPosition(vocal.note), 0);
        var obj = Instantiate(VocalPrefab, position, Quaternion.identity, gameObject.transform);
        accumulatedObjects.Add(obj);  // Track the spawned object
        accumulatedPositions.Add(position.x);  // Track its X position
        accumulatedEntries.Add(accumulatedEntry);  // Track its entry count
    }

    /// <summary>
    /// Method to spawn bass and trigger the OnBassNoteHit event.
    /// </summary>
    void SpawnBass()
    {
        float prev_offset = 0;
        float prev_pitch = 0;
        float prev_end = 0;

        foreach (var bass in offlineMusicDataAsset.bass)
        {
            float offset = Mathf.Round(bass.offset * 4) / 4f;  // Round offset to nearest 0.25

            // Skipping logic based on previous bass's pitch and offset
            if (Mathf.Round(bass.note) % 12 == prev_pitch % 12 && bass.offset_seconds + 0.3f < prev_end)
            {
                continue;
            }

            if ((offset % 1 != 0f || offset % 1 != 0.5f) && bass.duration_seconds < 0.15f)
            {
                continue;  // Skip very short bass notes
            }

            if (offset <= prev_offset + 0.25f || offset == prev_offset)
            {
                continue;  // Skip if the offset is too close to the previous one
            }

            prev_offset = offset;
            prev_pitch = Mathf.Round(bass.note);
            prev_end = bass.offset_seconds + bass.duration_seconds;
            BasPrefab.GetComponent<pitchdata>().pitch = bass.note;

            // Trigger the event for when a bass note is hit
            OnBassNoteHit?.Invoke(offset, bass);
        }
    }

    /// <summary>
    /// Instantiates the bass prefab when the OnBassNoteHit event is triggered.
    /// </summary>
    /// <param name="offset">The time offset for when the bass note is hit.</param>
    /// <param name="bass">The bass data associated with the note.</param>
    private void InstantiateBassPrefab(float offset, bass bass)
    {
        Vector3 position = new Vector3(offset * XOffsetMultiplier, GetYPosition(bass.note), 0);
        var obj = Instantiate(BasPrefab, position, Quaternion.identity, gameObject.transform);
        accumulatedBassObjects.Add(obj);  // Track the spawned bass object
        accumulatedBassPositions.Add(position.x);  // Track its X position
        accumulatedBassEntries.Add(accumulatedEntry);  // Track its entry count 
    }

    /// <summary>
    /// Method to spawn drums and trigger the OnDrumNoteHit event.
    /// </summary>
    void SpawnDrums()
    {
        float prev_offset = 0;

        foreach (var drum in offlineMusicDataAsset.drums)
        {
            float offset = Mathf.Round(drum.offset * 4) / 4f;  // Round offset to nearest 0.25

            if (offset <= prev_offset + 0.25f || offset == prev_offset)
            {
                continue;  // Skip if the offset is too close to the previous one
            }

            prev_offset = offset;
            DrumPrefab.GetComponent<pitchdata>().pitch = 128;  // Example pitch for drums (fixed value)

            // Trigger the event for when a drum note is hit
            OnDrumNoteHit?.Invoke(offset, drum);
        }
    }

    /// <summary>
    /// Instantiates the drum prefab when the OnDrumNoteHit event is triggered.
    /// </summary>
    /// <param name="offset">The time offset for when the drum note is hit.</param>
    /// <param name="drums">The drums data associated with the note.</param>
    private void InstantiateDrumPrefab(float offset, drums drum)
    {
        Vector3 position = new Vector3(offset * XOffsetMultiplier, YBasePosition, 0);  // Fixed Y position for drums
        var obj = Instantiate(DrumPrefab, position, Quaternion.identity, gameObject.transform);
        accumulatedObjects.Add(obj);  // Track the spawned object
        accumulatedPositions.Add(position.x);  // Track its X position
        accumulatedEntries.Add(accumulatedEntry);  // Track its entry count
    }

    // TODO: Consider moving chunk management logic to another script
    void SpawnChunk(ChunkAsset chunk)
    {
        // Loop through the chunk's intList to spawn corresponding objects
        for (int i = 0; i < chunk.intList.Count; i++)
        {
            int value = chunk.intList[i];  // Get the value for this beat in the chunk

            // Check if the value indicates a prefab should be spawned
            if (value > 0)
            {
                GameObject prefabToSpawn = VocalPrefab;  // Default to vocal prefab

                // Select the correct prefab based on the index
                if (i == 0)
                {
                    prefabToSpawn = DrumPrefab;  // Use drum prefab for index 0
                }
                else if (i == 8)
                {
                    prefabToSpawn = BasPrefab;  // Use bass prefab for index 8
                }

                // Calculate position using the X and Y multipliers and base Y position
                Vector3 position = new Vector3(accumulatedEntry * XOffsetMultiplier, (value * YOffsetMultiplier) + YBasePosition, 0);
                
                // Spawn the prefab at the calculated position
                var obj = Instantiate(prefabToSpawn, position, Quaternion.identity, gameObject.transform);
                accumulatedObjects.Add(obj);  // Track the spawned object
                accumulatedPositions.Add(position.x);  // Track its X position
                accumulatedEntries.Add(accumulatedEntry);  // Track its entry count
            }

            accumulatedEntry++;  // Increment the entry count
        }
    }

    /// <summary>
    /// Moves all the spawned chunks based on the current beat (only moves on the X-axis for now).
    /// </summary>
    void MoveChunks()
    {
        float beatPosition = ReactionalEngine.Instance.CurrentBeat * XOffsetMultiplier;

        // Move all objects (vocals, bass, drums) on the X-axis
        for (int i = 0; i < accumulatedObjects.Count; i++)
        {
            float position = accumulatedPositions[i];
            accumulatedObjects[i].transform.position = new Vector3(position - beatPosition, accumulatedObjects[i].transform.position.y, accumulatedObjects[i].transform.position.z);
        }

        // Move bass objects (similar to vocals)
        for (int i = 0; i < accumulatedBassObjects.Count; i++)
        {
            float position = accumulatedBassPositions[i];
            accumulatedBassObjects[i].transform.position = new Vector3(position - beatPosition, accumulatedBassObjects[i].transform.position.y, accumulatedBassObjects[i].transform.position.z);
        }
    }

    /// <summary>
    /// Calculates the Y-position based on the note value.
    /// </summary>
    /// <param name="note">The note value to calculate Y position for.</param>
    /// <returns>Returns the Y position adjusted for pitch.</returns>
    float GetYPosition(float note)
    {
        return (note % 12) / YPitchAdjustment - YPitchAdjustment;  // Return Y position adjusted for pitch
    }
}
