using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDiscreteMovement : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] private List<GameObject> targetBlocks; // List of game objects the player will jump to
    [SerializeField] private Transform playerModel; // Player's visual representation

    [Header("Jump Settings")]
    [SerializeField] private float jumpDuration = 0.2f; // Time it takes to complete a jump
    [SerializeField] private AnimationCurve jumpCurve; // Curve for smooth jumping animation
    [SerializeField] private float jumpHeight = 2f; // Height of the jump arc

    [Header("Misc")]
    private PlayerController controller;
    private int currentBlockIndex = 0; // Tracks the player's current block
    private bool canJump = false; // Ensures the player jumps only when allowed
    private bool isJumping = false; // Prevents multiple jumps during one animation

    private void Awake()
    {
        controller = new PlayerController();
        
        //Obtain GameObjects
        targetBlocks = FindFirstObjectByType<Reactional_DeepAnalysis_PreSpawner>().accumulatedObjects;
    }

    private void OnEnable()
    {
        //Subscribe to the Fly action
        controller.Player.Fly.Enable();
        controller.Player.Fly.performed += OnJump;
    }

    private void OnDisable()
    {
        //Unsubscribe from the Fly action
        controller.Player.Fly.performed -= OnJump;
        controller.Player.Fly.Disable();
    }

    private void Update()
    {
        // Enable jumping when the current block reaches x = 0
        if (currentBlockIndex < targetBlocks.Count && !isJumping)
        {
            if (Mathf.Abs(targetBlocks[currentBlockIndex].transform.position.x) < -1.9f)
            {
                canJump = true;
            }
        }
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (canJump && !isJumping)
        {
            StartCoroutine(JumpToNextBlock());
        }
    }

    private IEnumerator JumpToNextBlock()
    {
        if (currentBlockIndex >= targetBlocks.Count - 1) yield break; // Prevents jumping past the last block

        isJumping = true;
        canJump = false;

        Vector3 startPosition = playerModel.position;
        Vector3 endPosition = new Vector3(playerModel.position.x, targetBlocks[currentBlockIndex + 1].transform.position.y, playerModel.position.z);

        float elapsedTime = 0f;

        // Perform a parabolic jump
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;

            // Smooth animation using the jump curve
            float heightOffset = jumpCurve.Evaluate(t) * jumpHeight;
            playerModel.position = Vector3.Lerp(startPosition, endPosition, t) + new Vector3(0, heightOffset, 0);

            yield return null;
        }

        playerModel.position = endPosition; // Snap to the final position
        currentBlockIndex++;
        isJumping = false;
    }

    private void OnDrawGizmos()
    {
        // Draw lines between blocks for debugging
        if (targetBlocks != null && targetBlocks.Count > 1)
        {
            for (int i = 0; i < targetBlocks.Count - 1; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(targetBlocks[i].transform.position, targetBlocks[i + 1].transform.position);
            }
        }
    }
}
