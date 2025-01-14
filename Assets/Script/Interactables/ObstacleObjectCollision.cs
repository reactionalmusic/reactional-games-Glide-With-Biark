using System;
using UnityEngine;
using Random = UnityEngine.Random;


public class ObstacleObjectCollision : MonoBehaviour
{


    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private AudioSource worldAudioSource;
    [SerializeField] private AudioClip collisionSoundClip;

    private void Start()
    {
        worldAudioSource = GameObject.Find("SFXPlayer").GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraShake.TriggerShake(0.5f,1,1, 0.5f);
            
            worldAudioSource.pitch = Random.Range(0.9f, 1.1f);
            worldAudioSource.PlayOneShot(collisionSoundClip);
         
        }
    }
   

}
