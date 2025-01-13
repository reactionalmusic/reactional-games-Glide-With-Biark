using System.Collections;
using UnityEngine;

public class PlayerOnDeath : MonoBehaviour
{
    public static PlayerOnDeath Instance { get; private set; }
    
    [Header("Disolve before Death")]
    [SerializeField] private float _dissolveTime = 0.75f;
    [SerializeField] private bool canDisolve = true;
    [SerializeField] private bool canDisolveVertical = false;
    
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
        if(!_material) { Debug.LogWarning("Material null in player");}
        
        // Set the disolve amounts to the right values
        _dissolveAmount = Shader.PropertyToID("_DistortionStrength");
       
    }

    // TODO check why this value behaves wierd
    public IEnumerator DisolvePlayer(bool useDissolve, bool useVerticalDisolve)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(0.015f, 1.3f, (elapsedTime / _dissolveTime));
       

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
            Reactional.Playback.Theme.TriggerStinger("positive, medium", 0f);
            transform.parent.position = respawnPoint.position;
        }
        else
        {
            Debug.LogWarning("Respawn point is null");
        }
        
        float elapsedTime = 0f;
        while (elapsedTime < _dissolveTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedDissolve = Mathf.Lerp(1.3f, 0.015f, (elapsedTime / _dissolveTime));
     

            //if(!_material) { yield return null; }
            
            if (useDissolve)
                _material.SetFloat(_dissolveAmount, lerpedDissolve);

            
            yield return null;
        }
    }
}
