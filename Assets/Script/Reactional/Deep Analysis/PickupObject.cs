using Unity.VisualScripting;
using UnityEngine;

public class PickupObject : MonoBehaviour
{
    public int scoreAmount = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Pickup"))
        {
            Destroy(gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            GameManager manager = FindFirstObjectByType<GameManager>();
            manager.AddScore(scoreAmount);
            Destroy(gameObject);
        }
    }
}
