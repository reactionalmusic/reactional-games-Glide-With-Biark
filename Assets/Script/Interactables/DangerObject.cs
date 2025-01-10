using System;
using Unity.VisualScripting;
using UnityEngine;

public class DangerObject : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //Destroy(other.gameObject);
            
        }
    }
}
