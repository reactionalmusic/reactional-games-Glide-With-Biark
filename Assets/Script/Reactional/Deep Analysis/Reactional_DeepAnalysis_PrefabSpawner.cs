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
    //[SerializeField] private float spawnInterval = 1f; // Time interval between spawns
    //[SerializeField] private float spawnTimer = 0f;
    [SerializeField] private float spawnTopY = 5f; // Range for random height when spawning objects
    [SerializeField] private float spawnBottomY = -2f; // Range for random height when spawning objects
    private bool alternatingDrumToggle = true;

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
    }
    
    private void TriggerBassNoteSpawn(float offset, bass bass)
    {
        return;
        //if (alternatingBassToggle)
        {
            SpawnPickups(PickupPrefab,bass);
        }
        //alternatingBassToggle = !alternatingBassToggle;
    }
    private void TriggerVocalNoteSpawn(float offset, vocals vocal)
    {
        //if (alternatingToggle)
        {
            //SpawnPickups(ObstaclePrefab,vocal);
        }
        //alternatingToggle = !alternatingToggle;
    }
    private void TriggerDrumNoteSpawn(float offset, drums drum)
    {
        if (alternatingDrumToggle)
        {
            SpawnObstacle(ObstaclePrefab,drum);
        }
        else
        {
            SpawnObstacle(PickupPrefab,drum);
        }
        alternatingDrumToggle = !alternatingDrumToggle;
    }

    // Method to spawn objects with a specific prefab
    public void SpawnObstacle(GameObject obstaclePrefab, drums drums)
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
    
    public void SpawnPickups(GameObject pickupPrefab, bass bass)
    {
        if (pickupPrefab == null){
            Debug.LogWarning("Pickup object is null");
            return; // Exit if no prefab is provided
        }

        // Instantiate the prefab and set its position
        GameObject newObject = Instantiate(pickupPrefab);
        
        newObject.transform.position = transform.position + new Vector3(0, GetYPosition(bass.note), 0);

        // Add the newly spawned object to the list
        spawnedObjects.Add(newObject);
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
