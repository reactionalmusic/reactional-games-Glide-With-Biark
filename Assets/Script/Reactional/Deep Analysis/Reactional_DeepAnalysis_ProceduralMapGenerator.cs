using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reactional.Core;
using Reactional.Experimental;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

/// <summary>
/// Using Reactional Deep Analysis to instantiate and place objects at beats
/// specified by the respective instruments. The script also utilizes the beat to
/// move the objects accordingly.
/// </summary>

public class ReactionalDeepAnalysisProceduralMapGenerator : MonoBehaviour
{
    // Public variables for prefabs
    public GameObject vocalPrefab;
    public GameObject bassPrefab;
    public GameObject drumPrefab;
    public GameObject drumDangerPrefab;

    // Lists to track positions and objects spawned
    private readonly List<float> _accumulatedPositions = new List<float>();
    private readonly List<GameObject> _accumulatedObjects = new List<GameObject>();

    private readonly List<float> _accumulatedBassPositions = new List<float>();
    private readonly List<GameObject> _accumulatedBassObjects = new List<GameObject>();
    
    //Object spawn range
    [SerializeField] private float spawnTopY = 5f; // Range for random height when spawning objects
    [SerializeField] private float spawnBottomY = -2f; // Range for random height when spawning objects
    
    //OfflineMusicDataAsset
    [SerializeField] private DeepAnalysisAssetList offlineMusicDataAssetList;
    [SerializeField] private OfflineMusicDataAsset offlineMusicDataAsset;

    // Constants for positioning multipliers
    private const float XOffsetMultiplier = 5f; // Controls spacing on X-axis
    private const float OtherXOffset = -5f; // Manual offset if needed
    private const float YPitchAdjustment = 2f; // Adjustment for Y position based on pitch

    public IEnumerator SpawnSongs()
    {
        while (!Reactional.Playback.Playlist.IsPlaying)
        {
            yield return new WaitForNextFrameUnit();
        }
        var trackName = Reactional.Playback.Playlist.GetCurrentTrackInfo().trackHash;
        foreach (var dataAsset in offlineMusicDataAssetList.songs) 
        {
            if (dataAsset.hash == trackName)
            {
                offlineMusicDataAsset = dataAsset;
                break;
            }
        }

        SpawnVocals();
        //SpawnBass();
        SpawnDrums();
    }

    void Update()
    {
        // Continuously update the position of the chunks based on the current beat
        if (ReactionalEngine.Instance.CurrentBeat > 0)
        {
            MoveChunks();
        }
    }

    /// <summary>
    /// Method to spawn vocals based on the offlineMusicDataAsset
    /// </summary>
    void SpawnVocals()
    {
        //float prevOffset = 0;
        //float prevPitch = 0;
        //float prevEnd = 0;

        foreach (var vocal in offlineMusicDataAsset.vocals)
        {
            var offset = Mathf.Round(vocal.offset * 8) / 8f; // Round offset to nearest 0.25

            // Skipping logic based on previous vocal's pitch and offset
            //if (Mathf.Approximately(Mathf.Round(vocal.note) % 12, prev_pitch % 12) && vocal.offset_seconds + 0.3f < prev_end)
            //{
            //    continue;
            //}

            //if ((offset % 1 != 0f || !Mathf.Approximately(offset % 1, 0.5f)) && vocal.duration_seconds < 0.1f)
            //{
            //    continue; // Skip very short vocals
            //}

            //if (offset <= prev_offset + 0.75f || Mathf.Approximately(offset, prev_offset))
            //{
            //    continue; // Skip if the offset is too close to the previous one
            //}
            //
            //prevOffset = offset;
            vocalPrefab.GetComponent<Reactional_DeepAnalysis_PitchData>().pitch = vocal.note; 

            InstantiateVocalPrefab(offset, vocal);
        }
    }

    private void InstantiateVocalPrefab(float offset, vocals vocal)
    {
        // Using constants for the position calculations
        Vector3 position = new Vector3(offset * XOffsetMultiplier, GetYPosition(vocal.note), 0);
        var obj = Instantiate(vocalPrefab, position, Quaternion.identity, gameObject.transform); // Spawn the vocal prefab
        _accumulatedObjects.Add(obj); // Track the spawned object
        _accumulatedPositions.Add(position.x); // Track its X position
    }

    /// <summary>
    /// Method to spawn bass objects
    /// </summary>
    void SpawnBass()
    {
        float prevOffset = 0;
        float prevPitch = 0;
        float prevEnd = 0;

        foreach (var bass in offlineMusicDataAsset.bass)
        {
            float offset = Mathf.Round(bass.offset * 4) / 4f; // Round offset to nearest 0.25

            // Skipping logic based on previous bass object's pitch and offset
            if (Mathf.Approximately(Mathf.Round(bass.note) % 12, prevPitch % 12) && bass.offset_seconds + 0.3f < prevEnd)
            {
                continue;
            }

            if ((offset % 1 != 0f || !Mathf.Approximately(offset % 1, 0.5f)) && bass.duration_seconds < 0.15f)
            {
                continue; // Skip very short bass objects
            }

            if (offset <= prevOffset + 0.25f || Mathf.Approximately(offset, prevOffset))
            {
                continue; // Skip if the offset is too close to the previous one
            }

            prevOffset = offset;
            prevPitch = Mathf.Round(bass.note);
            prevEnd = bass.offset_seconds + bass.duration_seconds;
            bassPrefab.GetComponent<Reactional_DeepAnalysis_PitchData>().pitch = bass.note;

            InstantiateBassPrefab(offset, bass);
        }
    }

    private void InstantiateBassPrefab(float offset, bass bass)
    {
        // Position calculation with constants for better clarity
        Vector3 position = new Vector3(offset * XOffsetMultiplier, GetYPosition(bass.note), 0);
        var obj = Instantiate(bassPrefab, position, Quaternion.identity, gameObject.transform); // Spawn the bass prefab
        _accumulatedBassObjects.Add(obj); // Track the spawned bass object
        _accumulatedBassPositions.Add(position.x); // Track its X position
    }

    /// <summary>
    /// Method to spawn drum objects
    /// </summary>
    void SpawnDrums()
    {
        float prevOffset = 0;

        foreach (var drums in offlineMusicDataAsset.drums)
        {
            float offset = Mathf.Round(drums.offset * 4) / 4f; // Round offset to nearest 0.25

            if (offset <= prevOffset + 1.75f)
            {
                continue; // Skip if the offset is too close to the previous one
            }

            prevOffset = offset;
            drumPrefab.GetComponent<Reactional_DeepAnalysis_PitchData>().pitch = 128; // Example pitch for drums (fixed value)

            InstantiateDrumPrefab(offset);
        }
    }

    private void InstantiateDrumPrefab(float offset)
    {
        // Calculate position for drums using constants
        float randomY = Random.Range(spawnTopY , spawnBottomY ); // Random Y position
        Vector3 position = new Vector3(offset * XOffsetMultiplier + OtherXOffset, randomY, 0); // Use random Y for the drum
        var prefab = Random.value < 0.175f ? drumDangerPrefab : drumPrefab;
        
        var obj = Instantiate(prefab, position, Quaternion.identity, gameObject.transform); // Spawn the drum prefab
        _accumulatedObjects.Add(obj); // Track the spawned object
        _accumulatedPositions.Add(position.x); // Track its X position
        
        obj.SetActive(false);   //Sets object disabled until near player
    }

    // Moves all the spawned chunks based on the current beat (only moves on the X-axis for now)
    void MoveChunks()
    {
        float beatPosition = ReactionalEngine.Instance.CurrentBeat * XOffsetMultiplier;

        // Cleanup destroyed objects from accumulatedObjects
        for (int i = _accumulatedObjects.Count - 1; i >= 0; i--)
        {
            if (!_accumulatedObjects[i])
            {
                _accumulatedObjects.RemoveAt(i);
                _accumulatedPositions.RemoveAt(i);
                continue; // Skip this iteration after removal
            }

            float position = _accumulatedPositions[i];
            _accumulatedObjects[i].transform.position = new Vector3(position - beatPosition,
                _accumulatedObjects[i].transform.position.y, _accumulatedObjects[i].transform.position.z);

            if (_accumulatedObjects[i].transform.position.x < 15f)
            {
                _accumulatedObjects[i].SetActive(true);
            }
        }

        // Cleanup for accumulatedBassObjects
        for (var i = _accumulatedBassObjects.Count - 1; i >= 0; i--)
        {
            if (!_accumulatedBassObjects[i])
            {
                _accumulatedBassObjects.RemoveAt(i);
                _accumulatedBassPositions.RemoveAt(i);
                continue;
            }

            var position = _accumulatedBassPositions[i];
            _accumulatedBassObjects[i].transform.position = new Vector3(position - beatPosition,
                _accumulatedBassObjects[i].transform.position.y, _accumulatedBassObjects[i].transform.position.z);
            
            if (_accumulatedObjects[i].transform.position.x < 15f)
            {
                _accumulatedObjects[i].SetActive(true);
            }
        }
    }


    // Calculates the Y-position based on the note value
    static float GetYPosition(float note)
    {
        // Return Y position adjusted for pitch (adjustment factor and base value)
        return (note % 12) / YPitchAdjustment - YPitchAdjustment;
    }
}
