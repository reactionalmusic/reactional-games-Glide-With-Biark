using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PickupVFX : MonoBehaviour
{

    // // "Shader Properties"
    // private MaterialPropertyBlock materialPropertyBlock;
    // private static readonly int _dissolveAmount = Shader.PropertyToID("_DissolveAmount");
    
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float fieldSize = 1; // Initial size value
    [SerializeField] private float fieldSizeEndValue = 0.3f; // Target value
    [SerializeField] private float lerpDuration = 0.5f; // Duration for lerp

    public void vfxExplode()
    {
        StartCoroutine(vfxtimer());
    }
    
    private IEnumerator vfxtimer()
    {
        float elapsedTime = 0f;

        // While lerp isn't complete, update the explosion value
        while (elapsedTime < lerpDuration)
        {
            elapsedTime += Time.deltaTime;
            fieldSize = Mathf.Lerp(fieldSize, fieldSizeEndValue, elapsedTime / lerpDuration);
            visualEffect.SetFloat("FieldSize", fieldSize);
            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        visualEffect.SetFloat("FieldSize", fieldSizeEndValue);
        
        // Wait briefly before destroying the object
        yield return new WaitForSeconds(lerpDuration);
        Destroy(gameObject);
    }
    
    
}
