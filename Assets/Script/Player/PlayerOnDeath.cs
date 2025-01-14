using System.Collections;
using Reactional.Playback;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.VFX;

public class PlayerOnDeath : MonoBehaviour
{
    public static PlayerOnDeath Instance { get; private set; }

    [Header("Disolve before Death")] [SerializeField]
    private float _dissolveTime = 0.75f;

    [SerializeField] private bool canDisolve = true;
    [SerializeField] private bool canDisolveVertical;
    [SerializeField] private VisualEffect _vfxTrail;
    [SerializeField] private Light2D _light;
    private SpriteRenderer _spriteRenderer;
    private Material _material;
    
    [SerializeField] private Transform respawnPoint;

    private int _dissolveAmount;


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
        
        if (!_material) Debug.LogWarning("Material null in player");

        // Set the disolve amounts to the right values
        _dissolveAmount = Shader.PropertyToID("_DistortionStrength");
    }

    // TODO check why this value behaves wierd
    public IEnumerator DisolvePlayer(bool useDissolve, bool useVerticalDisolve)
    {
        var elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            var lerpedDissolve = Mathf.Lerp(0.015f, 1.3f, elapsedTime / _dissolveTime);
            _vfxTrail.enabled = false;
            _light.enabled = false;

            if (useDissolve)
                _material.SetFloat(_dissolveAmount, lerpedDissolve);
            

            yield return null;
        }
    }

    public IEnumerator SpawnPlayer(bool useDissolve, bool useVerticalDisolve)
    {
        //Spawn at respawnPoint
        if (respawnPoint)
        {
            Theme.TriggerStinger("positive, medium", 0f);
            transform.parent.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("Respawn point is null");
        }

        var elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            var lerpedDissolve = Mathf.Lerp(1.3f, 0.015f, elapsedTime / _dissolveTime);


            _vfxTrail.enabled = true;
            _light.enabled = true;
            if (useDissolve)
                _material.SetFloat(_dissolveAmount, lerpedDissolve);
          


            yield return null;
        }
    }
}