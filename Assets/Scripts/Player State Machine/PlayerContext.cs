using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerContext : MonoBehaviour
{

    #region Inputs
    [SerializeField] private InputActionAsset _inputActions;
    
    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _slideAction;
    private InputAction _attackAction;
    #endregion


    enum MovementStates 
    {
        Moving,
        Dashing
    }

    private MovementStates _movementState;

    private const float MAX_FALL_SPEED = 30;
    private const float JUMP_APEX_THRESHOLD = 0.185f; // temporary implementation if apex hanging
    private const float SPEED_BOOST_SLIDE_TIMER = 2f;

    private const float GROUND_CHECK_RADII = 0.08f;
    private const float GROUND_CHECK_ALLOWANCE = 0.325f;

    #region Variables
    [Header("Movement Variables")]
    [SerializeField] private int _playerSpeed = 11;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _dashForce = 18f;
    [SerializeField] private float _dashTime = 0.175f;
    [SerializeField] private float _slideBoost = 2.65f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 4f;
    private float _dashAmplifier = 3f;
    private bool _enableDash = true;
    private bool _startApexTimer;
    private float _apexCounter;
    private float _bonusSpeed;
    private float _dashCounter;

    [Header("Sensitivity")]
    [SerializeField] private float _rotateSpeed_X = 0.4f;
    [SerializeField] private float _rotateSpeed_Y = 0.5f;
    [SerializeField] Transform _cameraPoint;
    private float _yaw;
    private float _pitch;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    [Header("Miscellaneous")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _groundCheck;

    private bool _onGround()
    {
        if(Physics.SphereCast(_groundCheck.transform.position, GROUND_CHECK_RADII, Vector3.down,
            out RaycastHit _hit, GROUND_CHECK_ALLOWANCE, _groundLayer)) return Vector3.Angle(_hit.normal, Vector3.up) < 20f;

        return false;
    }

    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _temporaryObject;
    private Rigidbody _rb;
    Vector2 _moveDir = Vector2.zero;
    Vector2 _lookDir = Vector2.zero;

    private Vector2 _prevMoveDir = Vector2.zero;
    #endregion


    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _dashAction = InputSystem.actions.FindAction("Dash");
        _slideAction = InputSystem.actions.FindAction("Slide");
        _attackAction = InputSystem.actions.FindAction("Attack");

        _movementState = MovementStates.Moving;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        _moveDir = _moveAction.ReadValue<Vector2>();
        _lookDir = _lookAction.ReadValue<Vector2>();

        UpdateYawPitch();

        if (_attackAction.WasPressedThisFrame())
        {
            GameObject temp = Instantiate(_temporaryObject);

            temp.transform.position = this.transform.position;
        }

        if (_jumpAction.WasPressedThisFrame() && _onGround() ) Jump();

        if (_dashAction.WasPressedThisFrame() && _enableDash) _movementState = MovementStates.Dashing;

        if (_slideAction.WasPressedThisFrame()) Sliding();
        if (_slideAction.WasReleasedThisFrame()) CancelSlide();

        if (_startApexTimer) _apexCounter += Time.deltaTime;
    }

    private void FixedUpdate()
    {

        switch (_movementState)
        {
            case MovementStates.Moving:
                ApplyMovement();
                ApplyBetterGravity();
                break;
            case MovementStates.Dashing:
                Dashing();
                break;
        };
    }

    private void LateUpdate()
    {
        // Update Mouse Movement
        transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
        _cameraPoint.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void ApplyMovement()
    {
        float y = _rb.linearVelocity.y;
        Vector3 player_movement = (transform.forward * _moveDir.y + transform.right * _moveDir.x);

        _bonusSpeed = Math.Clamp(_bonusSpeed, 0, 25);
        player_movement *= (_playerSpeed + _bonusSpeed);

        Vector3 move = player_movement * Time.fixedDeltaTime;

        _rb.linearVelocity = new Vector3(player_movement.x, y, player_movement.z);
        _prevMoveDir = new Vector2(_moveDir.x,_moveDir.y);
        //_rb.AddForce(player_movement * 2.5f, ForceMode.Force);
    }

    void ApplyBetterGravity()
    {
        if (_rb.linearVelocity.y < 0 || (_startApexTimer && _apexCounter >= JUMP_APEX_THRESHOLD))
        {
            // Once falling, increase player's gravity
            ResetApex();
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (_fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (_rb.linearVelocity.y > 0 && !_jumpAction.IsPressed())
        {
            // Jump released early, fall faster
            ResetApex();
            _rb.linearVelocity += Vector3.up * Physics.gravity.y * (_lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }

        // Clamp fall speed
        if (_rb.linearVelocity.y < -MAX_FALL_SPEED)
        {
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, -MAX_FALL_SPEED, _rb.linearVelocity.z);
        }
    }

    void Dashing()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);

        if (_dashCounter >= _dashTime)
        {
            _enableDash = true;
            _dashCounter = 0;
            _movementState = MovementStates.Moving;

            return;
        }

        if (_enableDash)
        {
            _impulseSource.GenerateImpulse(2);
            Vector3 forceDirection = transform.forward.normalized * (_dashForce * _dashAmplifier);

            if(_prevMoveDir != Vector2.zero) forceDirection = (transform.forward.normalized * _prevMoveDir.y + transform.right.normalized * _prevMoveDir.x) * (_dashForce * _dashAmplifier);

            Debug.Log(forceDirection);
            _rb.AddForce(forceDirection, ForceMode.Impulse);
        }

        _dashCounter += Time.fixedDeltaTime;
        _enableDash = false;
    }

    void Jump()
    {
        if (_movementState != MovementStates.Moving) return; // lol very efficient code !!! (on god i will implement an HSM soon)

        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _startApexTimer = true;
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _bonusSpeed += 1;
    }

    void UpdateYawPitch()
    {
        // Set the yaw and pitch position
        _yaw += _lookDir.x * _rotateSpeed_X;
        _pitch -= _lookDir.y * _rotateSpeed_Y;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    void ResetApex()
    {
        _startApexTimer = false;
        _bonusSpeed -= 1;
        _apexCounter = 0;
    }

    void Sliding()
    {
        if (_movementState != MovementStates.Moving) return;

        _bonusSpeed += _slideBoost;
        StartCoroutine(SlideCooldown());
        this.transform.localScale = new Vector3(1, 0.5f, 1);
    }
    
    void CancelSlide()
    {
        this.transform.localScale = new Vector3(1, 1, 1);
    }


    private IEnumerator SlideCooldown()
    {
        // yes another temporary function lol i dont want to exert that much brainpower at 3am
        yield return new WaitForSeconds(SPEED_BOOST_SLIDE_TIMER);
        _bonusSpeed -= _slideBoost;
    }
}
