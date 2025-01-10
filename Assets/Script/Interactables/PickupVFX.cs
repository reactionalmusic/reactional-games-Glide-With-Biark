using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class PickupVFX : MonoBehaviour
{
   
    public Vector4 position;
    
    private float _fieldSize; // Initial size value
    private float _alpha;
    
    [SerializeField] private VisualEffect visualEffect;
    [SerializeField] private float fieldSizeShrink = 0.3f; // Target value
    [SerializeField] private float lerpShrinkDuration = 0.3f; // Duration for shrink
    [SerializeField] private float fieldSizeExplode = 1.5f;
    [SerializeField] private float lerpExplodeDuration = 0.8f; // Duration for explode
    [SerializeField] private float alphaIncrease = 1.35f; // alpha when shrinking
    [SerializeField] private float alphaFade = 0; // alpha when exploding
    
    public void vfxExplode()
    {
        
        Reactional.Playback.Theme.TriggerStinger("positive, large", 16);
        transform.GetComponent<PickupVFX>().enabled = true;
        visualEffect.enabled = true;  // TODO move to when VFX changes place
        
        // Reset original values
        _fieldSize = visualEffect.GetFloat("FieldSize");
        _alpha = visualEffect.GetFloat("Alpha");
        
        //Remove Collider from parent Object
        transform.parent.GetComponent<CircleCollider2D>().enabled = false;
        

        Debug.Log("Lets Explode");
        StartCoroutine(vfxtimer());
    }

    private IEnumerator vfxtimer()
    {
        var elapsedTime = 0f;

        // Phase 1: Shrink
        while (elapsedTime < lerpShrinkDuration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / lerpShrinkDuration;
            _fieldSize = Mathf.Lerp(_fieldSize, fieldSizeShrink, t);
            visualEffect.SetFloat("FieldSize", _fieldSize);
            _alpha = Mathf.Lerp(_alpha, alphaIncrease, t);
            visualEffect.SetFloat("Alpha", _alpha);
            yield return null;
        }

        elapsedTime = 0f;

        // Phase 2: Grow
        while (elapsedTime < lerpExplodeDuration)
        {
            elapsedTime += Time.deltaTime;
            var t = elapsedTime / lerpExplodeDuration;
            _fieldSize = Mathf.Lerp(fieldSizeShrink, fieldSizeExplode, t);
            visualEffect.SetFloat("FieldSize", _fieldSize);


            _alpha = Mathf.Lerp(alphaIncrease, alphaFade, t);
            visualEffect.SetFloat("Alpha", _alpha);


            yield return null;
        }

        // Wait briefly before destroying the object
        //yield return new WaitForSeconds(lerpShrinkDuration + lerpExplodeDuration);
        
        visualEffect.enabled = false;
    }
}