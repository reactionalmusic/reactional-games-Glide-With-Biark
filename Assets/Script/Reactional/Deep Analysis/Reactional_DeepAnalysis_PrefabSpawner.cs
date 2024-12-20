using System;
using System.Collections.Generic;
using Reactional.Experimental;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

/// <summary>
/// Reactional_DeepAnalysis_ObstacleSpawner is used for spawning gameobject prefabs on different beats
/// Both Props and objects that can affect the Player
/// </summary>
public class Reactional_DeepAnalysis_PrefabSpawner : MonoBehaviour
{
    [Header("References")] 
    private Reactional_DeepAnalysis_EventDispatcher eventDispatcher;
    
    [Header("Spawning Settings")]
    [SerializeField] private GameObject[] spawnableObjects; // Array to hold different objects to spawn

    [SerializeField] private GameObject ObstaclePrefab;
    [SerializeField] private GameObject PickupPrefab;
    [SerializeField] private GameObject TrailPrefab;
    [SerializeField] private float spawnTopY = 5f; // Range for random height when spawning objects
    [SerializeField] private float spawnBottomY = -2f; // Range for random height when spawning objects
    
    private bool alternatingDrumToggle = true;
    private bool vocalTrailEnabled = false;
    private float lastDrumBeatTime;
    private float drumBeatTimeout = 1f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private List<GameObject> spawnedObjects = new List<GameObject>(); // List to track spawned objects

    private void OnEnable()
    {
        eventDispatcher = FindFirstObjectByType<Reactional_DeepAnalysis_EventDispatcher>();
        
        Reactional_DeepAnalysis_EventDispatcher.OnBassNoteHit += TriggerBassNoteSpawn;
        Reactional_DeepAnalysis_EventDispatcher.OnVocalNoteHit += TriggerVocalNoteSpawn;
        Reactional_DeepAnalysis_EventDispatcher.OnDrumNoteHit += TriggerDrumNoteSpawn;
    }
    private void OnDisable()
    {
        Reactional_DeepAnalysis_EventDispatcher.OnBassNoteHit -= TriggerBassNoteSpawn;
        Reactional_DeepAnalysis_EventDispatcher.OnVocalNoteHit -= TriggerVocalNoteSpawn;
        Reactional_DeepAnalysis_EventDispatcher.OnDrumNoteHit -= TriggerDrumNoteSpawn;
    }

    void Update()
    {
        // Move all spawned objects
        MoveSpawnedObjects();
        
        // Check if drum beat hasn't occurred for a while
        vocalTrailEnabled = Time.time - lastDrumBeatTime > drumBeatTimeout;
    }
    
    private void TriggerBassNoteSpawn(float offset, bass bass)
    {
        
    }
    private void TriggerVocalNoteSpawn(float offset, vocals vocal)
    {
        if (!vocalTrailEnabled) return;
        
        TriggerVocalTrail(offset,vocal);
    }
    private void TriggerDrumNoteSpawn(float offset, drums drum)
    {
        if (alternatingDrumToggle)
        {
            SpawnPrefab(ObstaclePrefab,drum);
        }
        else
        {
            SpawnPrefab(PickupPrefab,drum);
        }
        alternatingDrumToggle = !alternatingDrumToggle;
        lastDrumBeatTime = Time.time; // Update the last drum beat time
    }

    // Method to spawn objects with a specific prefab
    public void SpawnPrefab(GameObject obstaclePrefab, drums drums)
    {
        if (obstaclePrefab == null){
            Debug.LogWarning("Obstacle object is null");
            return; // Exit if no prefab is provided
        }

        // Instantiate the prefab and set its position
        GameObject newObject = Instantiate(obstaclePrefab);
        
        newObject.transform.position = transform.position + new Vector3(0, Random.Range(spawnBottomY, spawnTopY), 0);

        // Add the newly spawned object to the list
        spawnedObjects.Add(newObject);
    }

    private void TriggerVocalTrail(float offset, vocals vocal)
    {
        // Create a trail following the vocals note
        if (TrailPrefab != null)
        {
            GameObject trail = Instantiate(TrailPrefab, transform);
            float trailYPosition = GetYPosition(vocal.note);
            trail.transform.position = transform.position + new Vector3(0, trailYPosition, 0);

            // Optionally adjust properties or behaviors of the trail object
            spawnedObjects.Add(trail);
        }
        else
        {
            Debug.LogWarning("TrailPrefab is not assigned!");
        }
    }
    
    /// <summary>
    /// Calculates the Y-position based on the note value.
    /// </summary>
    /// <param name="note">The note value to calculate Y position for.</param>
    /// <returns>Returns the Y position adjusted for pitch.</returns>
    public float GetYPosition(float note)
    {
        // Define the input (note) range and output (Y position) range
        float inputMin = 38f; // Minimum value of bass.note
        float inputMax = 52f; // Maximum value of bass.note
        float outputMin = -2f; // Minimum Y position
        float outputMax = 5f;  // Maximum Y position

        if(note < inputMin)
            inputMin = note;
        
        if(note > inputMax)
            inputMax = note;
        
        // Map the note to the output range
        float mappedValue = Mathf.Lerp(outputMin, outputMax, Mathf.InverseLerp(inputMin, inputMax, note));

        // Clamp the value to ensure it doesn't go beyond the range
        return Mathf.Clamp(mappedValue, outputMin, outputMax);
    }


    
    // TODO fix parallax and see if lists cant spawn same prefab from alinked list and make items invis
    // TODO put it back and reuse after it has passed the player
    private void MoveSpawnedObjects()
    {
        // Iterate through all spawned objects and move them
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] != null) // Check if the object still exists
            {
                spawnedObjects[i].transform.position += Vector3.left * (movementSpeed * Time.deltaTime);
            }
            else
            {
                // Remove null (destroyed) objects from the list
                spawnedObjects.RemoveAt(i);
            }
        }
    }
}
