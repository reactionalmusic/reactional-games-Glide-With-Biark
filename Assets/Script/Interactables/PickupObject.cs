using System;
using Unity.VisualScripting;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [SerializeField] PickupVFX vfxObject;
    public int scoreAmount = 1;
    
    
    [HideInInspector]public AudioSource audioSource;
    public AudioClip audioClip;
    public int startNote;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Pickup"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            audioSource = AudioSourcePool.Instance.GetAvailableSource();
            audioSource.clip = audioClip;
            var manager = FindFirstObjectByType<GameManager>();
            manager.AddScore(scoreAmount);
            float pitch = GetComponent<Reactional_DeepAnalysis_PitchData>().pitch;
            pitch = Mathf.Pow(2, (pitch - startNote) / 12f);
            audioSource.pitch = pitch;
            Reactional.Playback.MusicSystem.ScheduleAudio(audioSource, 0.25f);
            audioSource.Play();
            if(vfxObject != null)
                vfxObject.vfxExplode();
        }
    }
    
    private void OnDestroy()
    {
        if(audioSource != null)
            AudioSourcePool.Instance.ReturnSource(audioSource);
    }
}
