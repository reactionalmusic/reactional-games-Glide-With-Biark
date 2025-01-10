using UnityEngine;

public class OnCollisionDestroy : MonoBehaviour
{
   /// This method is called when another collider enters the trigger
   private void OnTriggerEnter2D(Collider2D other)
   {
       // Destroy the game object that collides with this trigger
       Destroy(other.gameObject);
   }
}
