using System;
using Reactional.Playback;
using UnityEngine;
/// <summary>
/// This Scripts moves any object on Z axis over a Animated Curve in sync to the Curretn Beat / Quant.
/// Change the Quant if you want to alter what beat in a bar that it should look for
/// </summary>
public class Reactional_PulsePosition : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] AnimationCurve curve;
   
    [Header("Reactional Quant Settings")]
    [SerializeField] private float quant = 2;
    
    private Vector3 _startPosition;
    private float _animationCurveValue;
   
    private void Start()
    {
        _startPosition.z = transform.position.z;
    }

    private void Update()
    {
        var vector3 = transform.position;
        vector3.z = (_startPosition.z + curve.Evaluate(MusicSystem.GetCurrentBeat() % quant));  // Using the curve.Evaluate on the current beat with a quantization value
        transform.position = vector3;
      
    }
}
