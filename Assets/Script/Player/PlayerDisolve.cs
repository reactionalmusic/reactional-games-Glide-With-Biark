using System;
using System.Collections;
using UnityEngine;

public class PlayerDisolve : MonoBehaviour
{
    public static PlayerDisolve Instance { get; private set; }

    [SerializeField] private float _dissolveTime = 0.75f;
    [SerializeField] private bool canDisolve = true;
    [SerializeField] private bool canDisolveVertical = false;
    
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Material _material;
    
    private int _dissolveAmount;
    private int _verticalDissolveAmount;
    

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _material = _spriteRenderer.material;
        
        
        // Set the disolve amounts to the right values
        _dissolveAmount = Shader.PropertyToID("_Disolve");
        _verticalDissolveAmount = Shader.PropertyToID("_VerticalDisolve");
    }

  // TODO check why this value behaves wierd
    public IEnumerator DisolvePlayer(bool useDissolve, bool useVerticalDisolve)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1f, 0f, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(1.1f, 0f, (elapsedTime / _dissolveTime));

            if (useDissolve)
                _material.SetFloat(_dissolveAmount, lerpedDissolve);

            if (useVerticalDisolve)
                _material.SetFloat(_verticalDissolveAmount, lerpedVerticalDissolve);
            yield return null;
        }
        
    }
    
    public IEnumerator SpawnPlayer(bool useDissolve, bool useVerticalDisolve)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0f, 1f, (elapsedTime / _dissolveTime));
            float lerpedVerticalDissolve = Mathf.Lerp(0f, 1.1f,(elapsedTime / _dissolveTime));

            if (useDissolve)
                _material.SetFloat(_dissolveAmount, lerpedDissolve);

            if (useVerticalDisolve)
                _material.SetFloat(_verticalDissolveAmount, lerpedVerticalDissolve);
            yield return null;
        }
        
    }
}
