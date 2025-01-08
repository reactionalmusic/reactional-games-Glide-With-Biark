using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDiscreteMovement : MonoBehaviour
{
    [Header("Game Objects")]
    private List<GameObject> targetBlocks; // List of game objects the player will jump to
    [SerializeField] private Transform playerModel; // Player's visual representation

    [Header("Jump Settings")]
    [SerializeField] private float jumpDuration = 0.5f; // Time it takes to complete a jump
    [SerializeField] private AnimationCurve jumpCurve; // Curve for smooth jumping animation
    [SerializeField] private float baseJumpHeight = 2f; // Base height of the jump arc

    [Header("Misc")]
    private PlayerController controller;
    private bool isJumping = false; // Prevents multiple jumps during one animation

    private void Awake()
    {
        controller = new PlayerController();
    }

    private void OnEnable()
    {
        var spawner = FindFirstObjectByType<Reactional_DeepAnalysis_PreSpawner>();
        if (spawner != null && spawner.accumulatedObjects != null)
        {
            targetBlocks = spawner.accumulatedObjects;
        }
        else
        {
            Debug.LogError("Failed to find Reactional_DeepAnalysis_PreSpawner or its accumulatedObjects.");
        }

        // Subscribe to the Fly action
        controller.Player.Fly.Enable();
        controller.Player.Fly.performed += OnJump;
    }

    private void OnDisable()
    {
        controller.Player.Fly.performed -= OnJump;
        controller.Player.Fly.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!isJumping)
        {
            Debug.Log("Jumping");
            StartCoroutine(JumpInArc());
        }
    }

    private IEnumerator JumpInArc()
    {
        isJumping = true;

        // Find the next target block
        GameObject nextBlock = GetNextBlock();
        if (nextBlock == null)
        {
            Debug.LogWarning("No valid block found for jumping!");
            isJumping = false;
            yield break;
        }

        Vector3 startPosition = playerModel.position;
        Vector3 endPosition;
        if (nextBlock.transform.position.x - playerModel.position.x < 4f)
        { 
            Debug.Log("Jumping" + (nextBlock.transform.position.x - playerModel.position.x).ToString());
                endPosition = new Vector3(
                nextBlock.transform.position.x, 
                nextBlock.transform.position.y + 0.5f, // Slight offset for safe landing
                playerModel.position.z
            );
        }
        else
        {
            endPosition = new Vector3(
                playerModel.transform.position.x, 
                playerModel.transform.position.y, // Slight offset for safe landing
                playerModel.position.z
            );
        }

        float jumpDistance = Mathf.Abs(endPosition.x - startPosition.x);
        float jumpHeight = baseJumpHeight + (jumpDistance * 0.5f); // Adjust arc height based on distance

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

        //playerModel.position = endPosition; // Snap to the final position
        isJumping = false;
    }

    /// <summary>
    /// Finds the next valid block to jump to.
    /// </summary>
    /// <returns>Next block GameObject</returns>
    private GameObject GetNextBlock()
    {
        foreach (var block in targetBlocks)
        {
            if (block.transform.position.x > playerModel.position.x)
            {
                return block;
            }
        }
        return null; // No valid block found
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
