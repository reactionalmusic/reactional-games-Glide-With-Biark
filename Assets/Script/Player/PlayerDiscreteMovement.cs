using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerDiscreteMovement : MonoBehaviour
{
    private PlayerController _controller;
    
    [SerializeField] private ScriptableStats _stats;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _col;
    private Vector2 _frameVelocity;
    private bool _cachedQueryStartInColliders;
    
    [SerializeField] private bool _grounded;
    private float _time;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<CapsuleCollider2D>();
        
        _controller = new PlayerController();
    }

    private void OnEnable()
    {
        // Subscribe to the Fly action
        _controller.Player.Fly.Enable();
        _controller.Player.Fly.performed += OnJump;
    }

    private void OnDisable()
    {
        _controller.Player.Fly.performed -= OnJump;
        _controller.Player.Fly.Disable();
    }

    private void Update()
    {
        _time += Time.deltaTime;
    }

    
    private void OnJump(InputAction.CallbackContext context)
    {
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

        private void CheckCollisions()
        {
            Physics2D.queriesStartInColliders = true;
            
            // Ground and Ceiling
            var col_pos = transform.position + new Vector3(_col.offset.x, _col.offset.y, 0);
            var x = _col.size.x;
            var x_half = _col.size.x / 2;
            var y_half = (_col.size.y / 2) + 0.1f;
            _grounded = Physics2D.Raycast(
                col_pos + new Vector3(-x_half, -y_half),
                Vector2.right,
                (x / 4) * 3,
                ~_stats.PlayerLayer
            );
#if UNITY_EDITOR
            Debug.DrawLine(
                col_pos + new Vector3(-x_half, -y_half), 
                col_pos + new Vector3(x_half, -y_half),
                _grounded ? Color.green : Color.red
            );
#endif
            bool ceilingHit = Physics2D.CapsuleCast(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _stats.GrounderDistance, ~_stats.PlayerLayer);

            // Hit a Ceiling
            if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

            // Landed on the Ground
            if (_grounded)
            {
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
            }

            Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
            //Debug.Log(_grounded);
            Debug.DrawRay(_col.bounds.center, Vector2.down * _stats.GrounderDistance, Color.green);
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

        if (_grounded) 
            ExecuteJump();

        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _frameVelocity.y = _stats.JumpPower;
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
