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
    
    
    
    //TODO WTF WAS THIS
    // [SerializeField] private Vector4 currentIntensity; 
    // [SerializeField] private float newIntensity = 0.001f; 
  


    public void vfxExplode()
    {
        // Destroy(transform.parent, 2);
       //TODO position = transform.parent.position;             INHERRIT CORRECT POS IN PARALLAX
        transform.parent = null;
        //TODO transform.position = position;
        transform.GetComponent<PickupVFX>().enabled = true;
        visualEffect.enabled = true;
        _fieldSize = visualEffect.GetFloat("FieldSize");
        _alpha = visualEffect.GetFloat("Alpha");
        // currentIntensity = visualEffect.GetVector4("Color");

        Debug.Log("However I will 'save' the children ...");
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


            // currentIntensity.x = Mathf.Lerp(currentIntensity.x, newIntensity, t);
            // currentIntensity.y = Mathf.Lerp(currentIntensity.y, newIntensity, t);
            // currentIntensity.z = Mathf.Lerp(currentIntensity.z, newIntensity, t);
            // visualEffect.SetVector4("Color", currentIntensity);
            //Debug.Log($"ElapsedTime: {elapsedTime}, t: {t}, FieldSize: {_fieldSize}" +  "alpha: " + alpha +  "intensity: " + currentIntensity.x +  currentIntensity.y +  currentIntensity.z);

            _alpha = Mathf.Lerp(alphaIncrease, alphaFade, t);
            visualEffect.SetFloat("Alpha", _alpha);


            yield return null;
        }

        // Wait briefly before destroying the object
        yield return new WaitForSeconds(lerpShrinkDuration + lerpExplodeDuration);

        visualEffect.enabled = false;
    }
}