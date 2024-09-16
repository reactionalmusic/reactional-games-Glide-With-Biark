using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ReactionalBeatSpawnManager is used for spawning gameobject prefabs on different beats
/// Both Props and objects that can affect the Player
/// </summary>
public class ReactionalBeatSpawnManager : MonoBehaviour
{
    
    [Header("Spawning Settings")]
    [SerializeField] private GameObject[] spawnableObjects; // Array to hold different objects to spawn

    [SerializeField] private GameObject PipePrefab;
    [SerializeField] private float spawnInterval = 1f; // Time interval between spawns
    [SerializeField] private float spawnHeightRange = 5f; // Range for random height when spawning objects
    [SerializeField] private float spawnTimer = 0f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 5f;

    private List<GameObject> spawnedObjects = new List<GameObject>(); // List to track spawned objects

    void Update()
    {
        
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            // Spawn a specific prefab 
            SpawnObject(PipePrefab); 
            spawnTimer = 0f; // Reset timer
        }

        // Move all spawned objects
        MoveSpawnedObjects();
    }

    // Method to spawn objects with a specific prefab
    public void SpawnObject(GameObject prefab)
    {
        if (prefab == null){
            Debug.LogWarning("Spawn object is null");
            return; // Exit if no prefab is provided
        }

        // Instantiate the prefab and set its position
        GameObject newObject = Instantiate(prefab);
        newObject.transform.position = transform.position + new Vector3(0, Random.Range(-spawnHeightRange, spawnHeightRange), 0);

        // Add the newly spawned object to the list
        spawnedObjects.Add(newObject);
    }

    private void MoveSpawnedObjects()
    {
        // Iterate through all spawned objects and move them
        for (int i = spawnedObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedObjects[i] != null) // Check if the object still exists
            {
                spawnedObjects[i].transform.position += Vector3.left * movementSpeed * Time.deltaTime;
            }
            else
            {
                // Remove null (destroyed) objects from the list
                spawnedObjects.RemoveAt(i);
            }
        }
    }
}
