using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DangerObject : MonoBehaviour
{
    [SerializeField] private int scorePenalty = -5; 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraShake.TriggerShake(0.5f,1,1, 0.5f);
            Reactional.Playback.Theme.TriggerStinger("negative, large");
            //Destroy(other.gameObject);
            StartCoroutine(PlayerCollision(other));
            var manager = FindFirstObjectByType<GameManager>();
            manager.AddScore(scorePenalty);
        }
    }
    private IEnumerator PlayerCollision(Collider2D other)
    {
        // Start the dissolve effect
        yield return StartCoroutine(PlayerOnDeath.Instance.DisolvePlayer(true, false));
       
        // Wait for 4 seconds
        yield return new WaitForSeconds(0.5f);
       
        // Set GameObject inactive
        //other.gameObject.SetActive(false);
        
        // Start the spawn effect
        yield return StartCoroutine(PlayerOnDeath.Instance.SpawnPlayer(true, false));

        //Debug.Log("Player respawn");
        // Call GameOver
        //gameManager.GameOver();
    }
}
