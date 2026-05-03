using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.PostProcessing;

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
    private InputAction _meleeAction;
    #endregion

    private const float MAX_FALL_SPEED = 30;
    private const float JUMP_APEX_THRESHOLD = 0.185f; // temporary implementation if apex hanging
    private const float SPEED_BOOST_SLIDE_TIMER = 2f;
    private const float SLIDE_COOLDOWN = 0.175f;

    private const float MAX_BONUS_SPEED = 25;
    private const float MIN_BONUS_SPEED = 0;

    private const float GROUND_CHECK_RADII = 0.08f;
    private const float GROUND_CHECK_ALLOWANCE = 0.325f;

    private const float MAX_STAMINA = 100;

    [Header("Movement Variables")]
    [SerializeField] private int _playerMaxHP = 100;
    [SerializeField] private int _playerSpeed = 11;
    [SerializeField] private float _jumpForce = 10.65f;
    [SerializeField] private float _dashForce = 18f;
    [SerializeField] private float _dashTime = 0.175f;
    [SerializeField] private float _slideBoost = 2.5f;
    [SerializeField] private float _slideTime = 0.5f;
    [SerializeField] private float _fallMultiplier = 3.15f;
    [SerializeField] private float _lowJumpMultiplier = 4f;
    private int _currentPlayerHP;
    private bool _enableDash = true;
    private bool _startApexTimer;
    private float _apexCounter;
    private float _bonusSpeed;
    private bool _enableSlideCooldown = false;
    private float _slideCooldownCounter = 0;
    private bool _allowCoyoteTime = false;

    [Header("Stamina System")]
    [SerializeField] private float _dashStamina = 20;
    [SerializeField] private float _slideStamina = 10;
    private float _staminaCount = MAX_STAMINA;
    private bool _allowStaminaMovement = true;

    public float GetMaxStamina { get { return MAX_STAMINA; } }
    public float GetStaminaCount { get { return _staminaCount; } set { _staminaCount = value; } }
    public float GetDashDepletion { get { return _dashStamina; } }
    public float GetSlideDepletion { get { return _slideStamina; } }
    public bool IsStaminaMovementAllowed { get { return _allowStaminaMovement; } set { _allowStaminaMovement = value; } }

    [Header("Gun")]
    [SerializeField] private BaseWeapon _currentWeapon;
    [SerializeField] private GameObject[] _weaponList;
    public GameObject[] GetAllWeapons { get { return _weaponList; } }
    public BaseWeapon GetCurrentWeapon { get { return _currentWeapon; }  set { _currentWeapon = value; } }

    [Header("Audio Clips")]
    public AudioClip[] _dashingSFX;

    [Header("Particle Effects")]
    public GameObject _dashOverlay;
    public ParticleSystem _dashEffect;

    [Header("Sensitivity")]
    [SerializeField] private float _rotateSpeed_X = 0.4f;
    [SerializeField] private float _rotateSpeed_Y = 0.5f;
    [SerializeField] Transform _cameraPoint;
    [SerializeField] private CinemachineCamera _camera;
    public CinemachineCamera GetCamera { get { return _camera; } }
    private float _yaw;
    private float _pitch;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    [Header("Miscellaneous")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private GameObject _groundCheck;
    public GameObject GetGroundTransform() { return _groundCheck; }

    [SerializeField] private float _maxSlopeAngle = 30;
    private float _originalFieldOfView = 0;
    public float GetOriginalFOV { get { return _originalFieldOfView; } }
    private RaycastHit _slopeHit;

    [SerializeField] private Volume _postProcessingVolume;
    private UnityEngine.Rendering.Universal.MotionBlur _motionBlur;

    public UnityEngine.Rendering.Universal.MotionBlur GetMotionBlur { get { return _motionBlur; } }


    private bool _onGround()
    {
        Vector3 origin = _groundCheck.transform.position;

        Vector3[] offsets = new Vector3[]
        {
            Vector3.zero,
            new Vector3(0.4f, 0, 0.4f),
            new Vector3(-0.4f, 0, 0.4f),
            new Vector3(0.4f, 0, -0.4f),
            new Vector3(-0.4f, 0, -0.4f),
        };

        foreach (var offset in offsets)
        {
            if (Physics.Raycast(origin + offset, Vector3.down, out RaycastHit hit,
                GROUND_CHECK_ALLOWANCE, _groundLayer))
            {
                if (Vector3.Angle(hit.normal, Vector3.up) < 20f)
                    return true;
            }
        }

        return false;
    }

    private bool _onSlope()
    {
        BoxCollider col = GetComponent<BoxCollider>();

        float rayLength = col.bounds.extents.y + 0.5f; // always reaches ground
        Vector3 origin = col.bounds.center;

        //Debug.DrawRay(origin, Vector3.down * rayLength, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out _slopeHit, rayLength, _groundLayer))
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle <= _maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 _getSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(_moveDir, _slopeHit.normal).normalized;
    }

    private bool _isDead()
    {
        return _currentPlayerHP <= 0 ? true : false;
    }

    [SerializeField] private CinemachineImpulseSource _impulseSource;
    private Rigidbody _rb;
    Vector2 _moveDir = Vector2.zero;
    Vector2 _lookDir = Vector2.zero;

    private Vector2 _prevMoveDir = Vector2.zero;

    private List<Tuple<float, float>> _speedQueue = new List<Tuple<float, float>>() { };


    // ****** STATE VARIABLES ******** //
    BaseState _currentState;
    StateInitialization _states;

    #region GETTERS AND SETTERS

    // Components
    public BaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Rigidbody GetRigidbody { get { return _rb; } }
    public CinemachineImpulseSource GetImpulseSource { get { return _impulseSource; } }

    // Player Variables
    public bool IsDead { get { return _isDead(); } }
    public bool IsGrounded { get { return _onGround(); } }
    public float BonusSpeed { get { return _bonusSpeed; } set { _bonusSpeed = value; } }
    public float GetMaxFallSpeed { get { return MAX_FALL_SPEED; } }
    public float GetMaxBonusSpeed { get { return MAX_BONUS_SPEED; } }
    public float GetMinBonusSpeed { get { return MIN_BONUS_SPEED; } } // here for verbose purposes and cleaner code
    public List<Tuple<float, float>> SpeedQueue;

    // Dash Variables
    public bool EnableDash { get { return _enableDash; } set { _enableDash = value; } }
    public float GetDashTime { get { return _dashTime; } }
    public float GetDashForce { get { return _dashForce; } }
    public InputAction GetDashInput { get { return _dashAction; } }
    public Vector2 PrevMoveDir { get { return _prevMoveDir; } set { _prevMoveDir = value; } }

    // Moving Variables
    public Vector2 GetMoveDir { get { return _moveDir; } }
    public int GetPlayerSpeed { get { return _playerSpeed; } }

    // Jumping Variables
    public InputAction GetJumpInput { get { return _jumpAction; } }
    public float GetFallMultiplier { get { return _fallMultiplier; } }
    public float GetLowJumpMultiplier { get { return _lowJumpMultiplier; } }
    public float GetJumpForce { get { return _jumpForce; } }
    public bool IsCoyoteTime { get { return _allowCoyoteTime; } set { _allowCoyoteTime = value; } }

    // Sliding Variables
    public InputAction GetSlideInput { get { return _slideAction; } }
    public float GetSlideTime { get { return _slideTime; } }
    public float GetSlideBoost { get { return _slideBoost; } }
    public bool GetSlideCooldown { get { return _enableSlideCooldown; } set { _enableSlideCooldown = value; } }

    // Slope Variables
    public bool IsOnSlope { get { return _onSlope(); } }
    public Vector3 GetSlopeMoveDirection { get { return _getSlopeMoveDirection(); }  }
    public RaycastHit GetSlopeHit { get { return _slopeHit; } }

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
        // Initialize Components
        _originalFieldOfView = _camera.Lens.FieldOfView;
        _rb = GetComponent<Rigidbody>();
        _currentPlayerHP = _playerMaxHP;
        SpeedQueue = _speedQueue;

        // Configure Inputs
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _dashAction = InputSystem.actions.FindAction("Dash");
        _slideAction = InputSystem.actions.FindAction("Slide");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _meleeAction = InputSystem.actions.FindAction("Melee");

        // Setup Current State
        _states = new StateInitialization(this);
        _currentState = _states.Alive();
        _currentState.EnterState();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _postProcessingVolume.profile.TryGet(out _motionBlur);
    }

    void Update()
    {
        _moveDir = _moveAction.ReadValue<Vector2>();
        _lookDir = _lookAction.ReadValue<Vector2>();

        _currentState.UpdateStates();

        CheckTimers();

        UpdateYawPitch();

        if (_currentWeapon != null)
        {
            switch (_currentWeapon.GetFireMode)
            {
                case "Auto":
                    if (_attackAction.IsPressed()) _currentWeapon.TryFire();
                    break;
                default:
                    if (_attackAction.WasPressedThisFrame()) _currentWeapon.TryFire();
                    break;
            }
        }

        if (_meleeAction.WasPressedThisFrame()) Debug.Log("Melee!");

        if (_startApexTimer) _apexCounter += Time.deltaTime;

        transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
        _cameraPoint.localRotation = Quaternion.Euler(_pitch, 0f, 0f);

        TryClearSpeedQueue();
    }

    private void FixedUpdate()
    {
        _currentState.FixedUpdateStates();
    }


    void CheckTimers()
    {
        if (_enableSlideCooldown) _slideCooldownCounter += Time.deltaTime;

        if (_enableSlideCooldown && _slideCooldownCounter >= SLIDE_COOLDOWN)
        {   
            _enableSlideCooldown = false;
            _slideCooldownCounter = 0;
        }
    }

    void UpdateYawPitch()
    {
        // Set the yaw and pitch position
        _yaw += _lookDir.x * _rotateSpeed_X;
        _pitch -= _lookDir.y * _rotateSpeed_Y;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    void TryClearSpeedQueue()
    {
        for (int i = 0; i < _speedQueue.Count - 1; i++)
        {
            var queue = _speedQueue[i];

            // If the queue's time is up, remove it
            if (queue.Item2 < 0)
            {
                Debug.Log("Clearing Speed Queue #" +  i + ": " + queue.Item1 + ", " + queue.Item2);
                _bonusSpeed -= queue.Item1;
                _speedQueue.RemoveAt(i);
                continue;
            }

            _speedQueue[i] = new Tuple<float, float>(queue.Item1, queue.Item2 - Time.deltaTime);
        }
    }
}
