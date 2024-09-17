using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Misc")]
    public PlayerController controller;
    [SerializeField] private GameManager gameManager;
    
    [Header("Flying")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float flyVelocity = 5f; // Upward force applied on click
    [SerializeField] private float maxFlyVelocity = 10f;
    [SerializeField] private float downwardForceMultiplier = 2f; // Adjusts how quickly gravity is applied
    [SerializeField] private float maxFallSpeed = -10f; // Limits how fast the player can fall
    [SerializeField] private float fallTriggerVelocity = 5f;
    
    private bool isFalling = false;
 
    private void Awake()
    {
        // Initialize 
        rb = GetComponent<Rigidbody2D>();
        gameManager = GetComponent<GameManager>();
        controller = new PlayerController();

        // Null checks for required serialized fields
        if (rb == null)
        {
            Debug.LogWarning("Rigidbody2D is not assigned as component in the PlayerMovement script.", this);
        }

        if (gameManager == null)
        {
            Debug.LogWarning("GameManager is not assigned as component in the PlayerMovement script.", this);
        }
       
    }
    
   

    private void OnEnable()
    {
        // Subscribe to the Fly action
        controller.Player.Fly.Enable();
        controller.Player.Fly.performed += OnFly;
        
        
    }

    private void OnDisable()
    {
        // Unsubscribe from the Fly action
        controller.Player.Fly.performed -= OnFly;
        controller.Player.Fly.Disable();
    }

    private void OnFly(InputAction.CallbackContext context)
    {
        
        rb.linearVelocity = Vector2.up * flyVelocity;
        isFalling = false;
        Debug.Log("Hello, I'm trying to fly");
      
        
    }
    
    
    void FixedUpdate()
    {
        // Check if the player has reached the apex and started to fall
        CheckIfShouldFall();
        
        

        // Apply downward force if the player is falling
        if (isFalling)
        {
            ApplyDownwardForce();
        }
        else if(rb.linearVelocity.y > maxFlyVelocity)
        {
            CapFlyVelocity();
        }
    }

    private void CheckIfShouldFall()
    {
        // Check if the velocity has changed from positive (upwards) to negative (downwards)
        if (rb.linearVelocity.y < fallTriggerVelocity && !isFalling)
        {
            isFalling = true;
        }
    }

    private void ApplyDownwardForce()
    {
        // Gradually add downward force to simulate gravity
        rb.linearVelocity += Vector2.down * downwardForceMultiplier * Time.fixedDeltaTime;

        // Limit the falling speed to maxFallSpeed
        if (rb.linearVelocity.y < maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallSpeed);
        }
    }

    private void CapFlyVelocity()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFlyVelocity);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Collision with " + other.gameObject.name + " Game Over");

        StartCoroutine(PlayerCollision());
    }
    
    
   // --------------- Ienumerator Coroutines -------------

   private IEnumerator WaitForSeconds(float seconds)
   {
       yield return new WaitForSeconds(seconds);
   }
   
   private IEnumerator PlayerCollision()
   {
       // Start the dissolve effect
       yield return StartCoroutine(PlayerDisolve.Instance.DisolvePlayer(true, false));
       

       // Wait for 4 seconds
       yield return new WaitForSeconds(4f);
       

       // Set GameObject inactive
       gameObject.SetActive(false);

       // Call GameOver
       gameManager.GameOver();
       
       
   }
   
   
}
