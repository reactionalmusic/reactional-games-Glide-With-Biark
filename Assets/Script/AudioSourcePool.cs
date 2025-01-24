using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool : MonoBehaviour
{
    [SerializeField] private int poolSize = 20; // Adjust this based on your max concurrent sounds
    public Queue<AudioSource> availableSources = new Queue<AudioSource>();
    private int poolCounter = 0; // Tracks the current count of AudioSources in the pool
    
    public static AudioSourcePool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;  // Set the singleton instance
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
    }
    
    // Initialize the pool
    private void Start()
    {
        // Pre-fill the pool with AudioSources up to the defined poolSize
        for (poolCounter = 0; poolCounter < poolSize; poolCounter++)
        {
            AudioSource newSource = CreateNewAudioSource(); // Create a new AudioSource programmatically
            newSource.gameObject.SetActive(false); // Start disabled
            availableSources.Enqueue(newSource);
        }
    }

    // Create a new AudioSource
    private AudioSource CreateNewAudioSource()
    {
        GameObject audioSourceObject = new GameObject("PooledAudioSource"); // Create a new GameObject
        AudioSource newSource = audioSourceObject.AddComponent<AudioSource>(); // Add AudioSource component
        newSource.transform.parent = this.transform; // Parent to the pool manager for organization
        return newSource;
    }

    // Get an available AudioSource from the pool
    public AudioSource GetAvailableSource()
    {
        // Check if there is an available source in the queue
        if (availableSources.Count > 0)
        {
            AudioSource source = availableSources.Dequeue();
            source.gameObject.SetActive(true); // Activate the source
            return source;
        }

        // If no sources are available but the pool isn't full, create a new one
        if (poolCounter < poolSize)
        {
            AudioSource newSource = CreateNewAudioSource();
            poolCounter++;
            return newSource;
        }

        // If all sources are in use and the pool is full, return null
        Debug.LogWarning("No available AudioSources and pool is at max size!");
        return null;
    }

   /// <summary>
   ///  Return an AudioSource back to the pool
   /// </summary>
   /// <param name="source"></param>
    public void ReturnSource(AudioSource source)
    {
        source.Stop(); // Stop playback
        source.clip = null; // Clear the audio clip reference to avoid memory leaks
        source.gameObject.SetActive(false); // Disable the source
        availableSources.Enqueue(source); // Add back to the pool
    }
}