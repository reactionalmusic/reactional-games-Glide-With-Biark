using UnityEngine;
using UnityEngine.InputSystem;

//Alternate Movement
public class PlayerContinuousMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float verticalSensitivity = 10f; // Adjusts how responsive the movement is
    [SerializeField] private float minY = -2f; // Minimum Y position the player can move to
    [SerializeField] private float maxY = 5f; // Maximum Y position the player can move to
    private Vector2 screenBounds; // Screen bounds for clamping position
    private float playerXPosition = -2f; // Fixed X position

    private PlayerController controller; // Input action map
    private Vector2 lastPointerPosition; // Tracks the last pointer position for delta movement
    private bool isDragging = false; // Indicates if the user is dragging

    private void Awake()
    {
        controller = new PlayerController();
    }

    private void OnEnable()
    {
        controller.Player.Hold.Enable();
        controller.Player.Hold.started += OnDragStart;
        controller.Player.Hold.canceled += OnDragEnd;
        controller.Player.Hold.performed += OnDrag;
    }

    private void OnDisable()
    {
        controller.Player.Hold.started -= OnDragStart;
        controller.Player.Hold.canceled -= OnDragEnd;
        controller.Player.Hold.performed -= OnDrag;
        controller.Player.Hold.Disable();
    }

    private void Start()
    {
        // Initialize the player's position
        transform.position = new Vector3(playerXPosition, 0, transform.position.z);

        // Get the screen bounds for clamping
        Camera mainCamera = Camera.main;
        screenBounds = new Vector2(
            mainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, 0)).y,
            mainCamera.ScreenToWorldPoint(new Vector3(0, 0, 0)).y
        );
    }

    private void OnDragStart(InputAction.CallbackContext context)
    {
        isDragging = true;
        lastPointerPosition = context.ReadValue<Vector2>();
    }

    private void OnDrag(InputAction.CallbackContext context)
    {
        if (!isDragging) return;

        Vector2 currentPointerPosition = context.ReadValue<Vector2>();
        Vector2 pointerDelta = currentPointerPosition - lastPointerPosition;

        // Convert pointer delta to world space movement
        float worldDeltaY = (pointerDelta.y / Screen.height) * verticalSensitivity;

        // Update player's Y position with clamping
        float newY = Mathf.Clamp(transform.position.y + worldDeltaY, minY, maxY);
        transform.position = new Vector3(playerXPosition, newY, transform.position.z);

        lastPointerPosition = currentPointerPosition;
    }

    private void OnDragEnd(InputAction.CallbackContext context)
    {
        isDragging = false;
    }
}
