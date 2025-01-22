using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactional_CameraPulseOnBeat : MonoBehaviour
{
    public AnimationCurve zoomCurve; // The curve controlling the zoom
    public float zoomSpeed = 1f; // The speed of the zoom
    public float maxZoom = 10f; // The maximum zoom
    public float minZoom = 5f; // The minimum zoom
    float previousBeat = 0; // The previous beat

    private Camera cam; // The camera
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        float zoom = zoomCurve.Evaluate(Reactional.Playback.MusicSystem.GetCurrentBeat()%zoomSpeed/zoomSpeed) * (maxZoom - minZoom) + minZoom;
        cam.fieldOfView = zoom;

        previousBeat = Reactional.Playback.MusicSystem.GetCurrentBeat();
    }
}