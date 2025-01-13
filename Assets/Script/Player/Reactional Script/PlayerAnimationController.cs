using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerAnimationController : MonoBehaviour
{
    private static readonly int spin_trigger = Animator.StringToHash("SpinTrigger");

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] float startTimeRange = 2f;
    [SerializeField] float endTimeRange = 4f;
    
    [Header("Reactional Stingers")]
    [SerializeField] private List<String> stingerNames = new List<String>(); //Remember to fill the list with project stingers
    [SerializeField] private float quant = 0f;

    private float RandomTime;
    private float timer;
    
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        SetRandomTime();
    }
    
    private void Update()
    {
        // Count down the timer
        timer += Time.deltaTime;
        
        if (timer >= RandomTime)
        {
            // Trigger the Spin animation
            animator.SetTrigger(spin_trigger);
            //RandomizeReactionalStinger();                 Timing is off
    
            // Reset the timer and random time
            SetRandomTime();
        }
    }
    
    // Set a new random time between 2 to 5 seconds (adjust as needed)
    private void SetRandomTime()
    {
        RandomTime = Random.Range(startTimeRange, endTimeRange);
        timer = 0f;
    }

    private void RandomizeReactionalStinger()
    {
        // Get a random index from 0 to the count of the list
        int randomIndex = UnityEngine.Random.Range(0, stingerNames.Count);
    
        // Use the random index to fetch a string from the list
        string randomStinger = stingerNames[randomIndex];
        
        Reactional.Playback.Theme.TriggerStinger(randomStinger, quant);
    }
}
