using System;
using Unity.VisualScripting;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    [SerializeField] PickupVFX vfxObject;
    public int scoreAmount = 1;
   
 
    
    private void OnDestroy()
    {
        Debug.Log("I am now self-destructing ...");
        if(vfxObject != null)
            vfxObject.vfxExplode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Pickup"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            var manager = FindFirstObjectByType<GameManager>();
            manager.AddScore(scoreAmount);
            Destroy(gameObject);
        }
    }
}
