using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


[DisallowMultipleComponent]
[RequireComponent(typeof(CinemachineVirtualCamera))]
// To use this script, simply call `CameraShake.TriggerShake` with the desired parameters. 
// Remember to set a Noise Profile in the Virtual Camera in cinemachine, and set the amplitude and freq to zero. 
// Ensure that an instance of `CameraShake` is present in the scene on a GameObject with a Cinemachine Virtual Camera component.
public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;
    public CinemachineVirtualCamera VirtualCamera;
    private CinemachineBasicMultiChannelPerlin channelPerlinNoise;

    private bool isShaking = false; // FLag to sheck if shake is already happening
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject); // Ensure only one instance exists.
        }
        else
        {
            instance = this;
        }
        channelPerlinNoise = VirtualCamera.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    /// Triggers a camera shake effect using the Cinemachine Virtual Camera. This method initiates a camera shake
    /// that interpolates both amplitude and frequency from their start values to 0, creating a diminishing shake effect.
    /// This method ensures that only one shake effect occurs at a time by checking the `isShaking` flag before starting a new shake.
    /// </summary>
    /// <param name="duration">The total duration of the shake effect in seconds. This controls how long the shake effect will last.</param>
    /// <param name="startAmplitude">The starting amplitude of the shake. This controls the initial intensity of the camera's shake effect.</param>
    /// <param name="startFrequency">The starting frequency of the shake. This controls the initial speed of the shake oscillations.</param>
    /// <param name="decreeasSpeed">A multiplier for the rate at which the shake's magnitude and frequency decrease. A higher value makes the shake taper off more quickly, whereas a lower value makes it decrease more slowly. This does not affect the total duration of the shake, but rather how quickly it diminishes.</param>
    /// <remarks>
    public static void TriggerShake(float duration, float startAmplitude, float startFrequency, float decreeasSpeed)
    {
        if (instance != null && !instance.isShaking)
        {
            instance.isShaking = true;
            instance.StartCoroutine(instance.ShakeDuration(duration, startAmplitude, startFrequency, decreeasSpeed));
            // ControllerRumbleManager.Instance.SetRumble(0.25f, 1f, duration); If we later add rubmle to controls
        }
    }

    private IEnumerator ShakeDuration(float duration, float startAmplitude, float startFrequency, float decreaseSpeed)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float lerpFactor = elapsed / duration * decreaseSpeed;
            channelPerlinNoise.m_AmplitudeGain = Mathf.Lerp(startAmplitude, 0f, lerpFactor);
            channelPerlinNoise.m_FrequencyGain = Mathf.Lerp(startFrequency, 0f, lerpFactor);

            yield return null;
        }
        ResetNoise();
        isShaking = false;
    }

    private void ResetNoise()
    {
        channelPerlinNoise.m_AmplitudeGain = 0f;
        channelPerlinNoise.m_FrequencyGain = 0f;
    }
}
