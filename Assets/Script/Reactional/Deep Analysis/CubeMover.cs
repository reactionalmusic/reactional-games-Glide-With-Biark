using System.Collections;
using System.Collections.Generic;
using Reactional.Core;
using UnityEngine;

public class CubeMover : MonoBehaviour
{
    public int score = 0;
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10; // Set this to be the distance you want the object to be placed in front of the camera.
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        this.transform.position = new Vector3(worldPosition.x, this.transform.position.y, this.transform.position.z);

    }

    void OnTriggerEnter(Collider other)
    {
        float midiAudioTarget = other.transform.parent.GetComponent<pitchdata>().pitch;
        audioSource.pitch = Mathf.Pow(2, (midiAudioTarget - 60) / 12f);

        if (other.name.StartsWith("BigSphere"))
        {
           // Reactional.Playback.Theme.SetControl("big", 0.25f);
        }
        else if (other.name.StartsWith("MediumSphere"))
        {
          //  Reactional.Playback.Theme.SetControl("medium", 0.25f);
        }
        else
        {
            //Reactional.Playback.Theme.SetControl("small", 0.25f);
        }
        score++;
        Debug.Log(other.name);
        Destroy(other.gameObject);

        Reactional.Playback.MusicSystem.ScheduleAudio(audioSource, 0.5f);


    }
}
