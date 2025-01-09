using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerDiscreteMovement : MonoBehaviour
{
    private PlayerController controller;
    
    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private FrameInput _frameInput;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;

    #region Interface

    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    
    #endregion
    
    private float _time;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        
        controller = new PlayerController();
    }

    private void OnEnable()
    {
        // Subscribe to the Fly action
        controller.Player.Fly.Enable();
        controller.Player.Fly.performed += OnJump;
    }

    private void OnDisable()
    {
        controller.Player.Fly.performed -= OnJump;
        controller.Player.Fly.Disable();
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }

    
    private void OnJump(InputAction.CallbackContext context)
    {
        _frameInput = new FrameInput
        {
            JumpDown = true
        };
        _jumpToConsume = true;
        _timeJumpWasPressed = _time;
        
        HandleJump();
    }
    
    private void FixedUpdate()
    {
        CheckCollisions();

        HandleGravity();
            
        ApplyMovement();
    }
    
    #region Collisions

    private bool _grounded = false;
    
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer; // Layer for ground objects
    [SerializeField] private Transform groundCheckPoint; // Reference to the ground check position
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius for the overlap check
    
    void CheckCollisions()
    {
        // Use Physics2D.OverlapCircle to check if ground is detected
        _grounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        // Optional: Visualize the ground check in the Scene view
        Debug.DrawLine(groundCheckPoint.position, groundCheckPoint.position + Vector3.down * groundCheckRadius, Color.red);
    }
    
    #endregion



    #region Jumping

    private bool _jumpToConsume;
    private bool _bufferedJumpUsable;
    private bool _endedJumpEarly;
    private float _timeJumpWasPressed;

    private bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;

    private void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && _rb.linearVelocity.y > 0) _endedJumpEarly = true;
        
        if (!_jumpToConsume && !HasBufferedJump) return;

        //if (_grounded) 
            ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        Debug.Log("Executing");
        CameraShake.TriggerShake(0.5f,1,1, 0.5f);
        
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion
    
    #region Gravity

    private void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        }
        else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y = Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    private void ApplyMovement() => _rb.linearVelocity = _frameVelocity;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (_stats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
}
