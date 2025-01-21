using System;
using Unity.VisualScripting;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [SerializeField] PickupVFX vfxObject;
    public int scoreAmount = 1;
    
    public AudioSource audioSource;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Pickup"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            var manager = FindFirstObjectByType<GameManager>();
            manager.AddScore(scoreAmount);
            float pitch = GetComponent<NoteData>().note;
            pitch = Mathf.Pow(2, (pitch - 60) / 12f);
            audioSource.pitch = pitch;
            Reactional.Playback.MusicSystem.ScheduleAudio(audioSource, 0.25f);
            vfxObject.vfxExplode();
        }
    }
}
