using System;
using Reactional.Playback;
using UnityEngine;

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
        vector3.z = (_startPosition.z + curve.Evaluate(MusicSystem.GetCurrentBeat() % quant));
        transform.position = vector3;
      
    }
}
