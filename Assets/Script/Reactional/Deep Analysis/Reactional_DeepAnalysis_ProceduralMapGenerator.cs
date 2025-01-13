using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Reactional.Core;
using Reactional.Experimental;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
/// <summary>
/// TODO for joel to describe this 
/// </summary>
public class Reactional_DeepAnalysis_ProceduralMapGenerator : MonoBehaviour
{
    // Public variables for prefabs
    public GameObject VocalPrefab;
    public GameObject BasPrefab;
    public GameObject DrumPrefab;
    public GameObject DrumDangerPrefab;

    // Lists to track positions and objects spawned
    private List<float> accumulatedPositions = new List<float>();
    public List<GameObject> accumulatedObjects = new List<GameObject>();
    private List<int> accumulatedEntries = new List<int>();

    private List<float> accumulatedBassPositions = new List<float>();
    private List<GameObject> accumulatedBassObjects = new List<GameObject>();
    private List<int> accumulatedBassEntries = new List<int>();
    private int accumulatedEntry = 0;
    
    //Object spawn range
    [SerializeField] private float spawnTopY = 5f; // Range for random height when spawning objects
    [SerializeField] private float spawnBottomY = -2f; // Range for random height when spawning objects
    
    [SerializeField] private List<OfflineMusicDataAsset> offlineMusicDataAssetList;
    [SerializeField] private OfflineMusicDataAsset offlineMusicDataAsset;

    // Constants for positioning multipliers
    private const float XOffsetMultiplier = 5f; // Controls spacing on X-axis
    private const float otherXOffset = -5f; // other offsets if needed
    private const float YPitchAdjustment = 2f; // Adjustment for Y position based on pitch

    //TODO add delegates here for all instrument events?

    public IEnumerator SpawnSongs()
    {
        while (!Reactional.Playback.Playlist.IsPlaying)
        {
            yield return new WaitForNextFrameUnit();
        }
        var track_name = Reactional.Playback.Playlist.GetCurrentTrackInfo().trackHash;
        //print("trackName:" + track_name);
        foreach (var data_asset in offlineMusicDataAssetList) 
        {
            if (data_asset.hash == track_name)
            {
                //print("data_asset name:" + data_asset.name);
                offlineMusicDataAsset = data_asset;
                break;
            }
        }

        // TODO add the Delegates here and subscribe on them , but break out the function as separate functions that you call here. 

        SpawnVocals();
        //SpawnBass();
        SpawnDrums();   //Only drums work currently
    }

    //TODO add event for subscribe and unsubscribe , 2 methods. 

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
        float prev_offset = 0;
        float prev_pitch = 0;
        float prev_end = 0;

        foreach (var vocal in offlineMusicDataAsset.vocals)
        {
            float offset = Mathf.Round(vocal.offset * 8) / 8f; // Round offset to nearest 0.25

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

            prev_offset = offset;
            VocalPrefab.GetComponent<Reactional_DeepAnalysis_PitchData>().pitch = vocal.note; 

            InstantiateVocalPrefab(offset, vocal);
        }
    }

    private void InstantiateVocalPrefab(float offset, vocals vocal)
    {
        // Using constants for the position calculations
        Vector3 position = new Vector3(offset * XOffsetMultiplier, GetYPosition(vocal.note), 0);
        var obj = Instantiate(VocalPrefab, position, Quaternion.identity, gameObject.transform); // Spawn the vocal prefab
        accumulatedObjects.Add(obj); // Track the spawned object
        accumulatedPositions.Add(position.x); // Track its X position
        accumulatedEntries.Add(accumulatedEntry); // Track its entry count
    }

    /// <summary>
    /// Method to spawn bass objects
    /// </summary>
    void SpawnBass()
    {
        float prev_offset = 0;
        float prev_pitch = 0;
        float prev_end = 0;

        foreach (var bass in offlineMusicDataAsset.bass)
        {
            float offset = Mathf.Round(bass.offset * 4) / 4f; // Round offset to nearest 0.25

            // Skipping logic based on previous bass object's pitch and offset
            if (Mathf.Approximately(Mathf.Round(bass.note) % 12, prev_pitch % 12) && bass.offset_seconds + 0.3f < prev_end)
            {
                continue;
            }

            if ((offset % 1 != 0f || !Mathf.Approximately(offset % 1, 0.5f)) && bass.duration_seconds < 0.15f)
            {
                continue; // Skip very short bass objects
            }

            if (offset <= prev_offset + 0.25f || Mathf.Approximately(offset, prev_offset))
            {
                continue; // Skip if the offset is too close to the previous one
            }

            prev_offset = offset;
            prev_pitch = Mathf.Round(bass.note);
            prev_end = bass.offset_seconds + bass.duration_seconds;
            BasPrefab.GetComponent<Reactional_DeepAnalysis_PitchData>().pitch = bass.note;

            InstantiateBassPrefab(offset, bass);
        }
    }

    private void InstantiateBassPrefab(float offset, bass bass)
    {
        // Position calculation with constants for better clarity
        Vector3 position = new Vector3(offset * XOffsetMultiplier, GetYPosition(bass.note), 0);
        var obj = Instantiate(BasPrefab, position, Quaternion.identity, gameObject.transform); // Spawn the bass prefab
        accumulatedBassObjects.Add(obj); // Track the spawned bass object
        accumulatedBassPositions.Add(position.x); // Track its X position
        accumulatedBassEntries.Add(accumulatedEntry); // Track its entry count 
    }

    /// <summary>
    /// Method to spawn drum objects
    /// </summary>
    void SpawnDrums()
    {
        float prev_offset = 0;

        foreach (var drums in offlineMusicDataAsset.drums)
        {
            float offset = Mathf.Round(drums.offset * 4) / 4f; // Round offset to nearest 0.25

            if (offset <= prev_offset + 1.75f)
            {
                continue; // Skip if the offset is too close to the previous one
            }

            prev_offset = offset;
            DrumPrefab.GetComponent<Reactional_DeepAnalysis_PitchData>().pitch = 128; // Example pitch for drums (fixed value)

            InstantiateDrumPrefab(offset);
        }
    }

    private void InstantiateDrumPrefab(float offset)
    {
        // Calculate position for drums using constants
        float randomY = Random.Range(spawnTopY , spawnBottomY ); // Random Y position
        Vector3 position = new Vector3(offset * XOffsetMultiplier + otherXOffset, randomY, 0); // Use random Y for the drum
        GameObject prefab;
        if (Random.value < 0.175f)
        {
            prefab = DrumDangerPrefab;
        }
        else
        {
            prefab = DrumPrefab;
        }
        var obj = Instantiate(prefab, position, Quaternion.identity, gameObject.transform); // Spawn the drum prefab
        accumulatedObjects.Add(obj); // Track the spawned object
        accumulatedPositions.Add(position.x); // Track its X position
        accumulatedEntries.Add(accumulatedEntry); // Track its entry count
        
        obj.SetActive(false);   //Sets disabled until near player
    }

    // Moves all the spawned chunks based on the current beat (only moves on the X-axis for now)
    void MoveChunks()
    {
        float beatPosition = ReactionalEngine.Instance.CurrentBeat * XOffsetMultiplier;

        // Cleanup destroyed objects from accumulatedObjects
        for (int i = accumulatedObjects.Count - 1; i >= 0; i--)
        {
            if (!accumulatedObjects[i])
            {
                accumulatedObjects.RemoveAt(i);
                accumulatedPositions.RemoveAt(i);
                accumulatedEntries.RemoveAt(i);
                continue; // Skip this iteration after removal
            }

            float position = accumulatedPositions[i];
            accumulatedObjects[i].transform.position = new Vector3(position - beatPosition,
                accumulatedObjects[i].transform.position.y, accumulatedObjects[i].transform.position.z);

            if (accumulatedObjects[i].transform.position.x < 15f)
            {
                accumulatedObjects[i].SetActive(true);
            }
        }

        // Cleanup for accumulatedBassObjects
        for (int i = accumulatedBassObjects.Count - 1; i >= 0; i--)
        {
            if (!accumulatedBassObjects[i])
            {
                accumulatedBassObjects.RemoveAt(i);
                accumulatedBassPositions.RemoveAt(i);
                continue;
            }

            float position = accumulatedBassPositions[i];
            accumulatedBassObjects[i].transform.position = new Vector3(position - beatPosition,
                accumulatedBassObjects[i].transform.position.y, accumulatedBassObjects[i].transform.position.z);
            
            if (accumulatedObjects[i].transform.position.x < 15f)
            {
                accumulatedObjects[i].SetActive(true);
            }
        }
    }


    // Calculates the Y-position based on the note value
    float GetYPosition(float note)
    {
        // Return Y position adjusted for pitch (adjustment factor and base value)
        return (note % 12) / YPitchAdjustment - YPitchAdjustment;
    }
}
