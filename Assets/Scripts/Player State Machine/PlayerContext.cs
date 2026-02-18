using System;
using System.Collections;
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
    private InputAction _meleeAction;
    #endregion

    private const float MAX_FALL_SPEED = 30;
    private const float JUMP_APEX_THRESHOLD = 0.185f; // temporary implementation if apex hanging
    private const float SPEED_BOOST_SLIDE_TIMER = 2f;

    private const float GROUND_CHECK_RADII = 0.08f;
    private const float GROUND_CHECK_ALLOWANCE = 0.325f;

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


    // ****** STATE VARIABLES ******** //
    BaseState _currentState;
    StateInitialization _states;

    private bool _onGround()
    {
        if (Physics.SphereCast(_groundCheck.transform.position, GROUND_CHECK_RADII, Vector3.down,
            out RaycastHit _hit, GROUND_CHECK_ALLOWANCE, _groundLayer)) return Vector3.Angle(_hit.normal, Vector3.up) < 20f;

        return false;
    }

    [SerializeField] private CinemachineImpulseSource _impulseSource;
    [SerializeField] private GameObject _temporaryObject;
    private Rigidbody _rb;
    Vector2 _moveDir = Vector2.zero;
    Vector2 _lookDir = Vector2.zero;

    private Vector2 _prevMoveDir = Vector2.zero;


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
        _rb = GetComponent<Rigidbody>();

        // Setup Current State
        _states = new StateInitialization(this);
        _currentState = _states.Alive();
        _currentState.EnterState();

        // Configure Inputs
        _moveAction = InputSystem.actions.FindAction("Move");
        _lookAction = InputSystem.actions.FindAction("Look");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _dashAction = InputSystem.actions.FindAction("Dash");
        _slideAction = InputSystem.actions.FindAction("Slide");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _meleeAction = InputSystem.actions.FindAction("Melee");
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

        if (_meleeAction.WasPressedThisFrame()) Debug.Log("Melee!");

        if (_jumpAction.WasPressedThisFrame() && _onGround()) Debug.Log("jump!");

        if (_dashAction.WasPressedThisFrame() && _enableDash) Debug.Log("dashing");

        if (_slideAction.WasPressedThisFrame()) Debug.Log("start sliding");
        if (_slideAction.WasReleasedThisFrame()) Debug.Log("cancel sliding");

        if (_startApexTimer) _apexCounter += Time.deltaTime;
    }

    private void LateUpdate()
    {
        // Update Mouse Movement
        transform.localRotation = Quaternion.Euler(0f, _yaw, 0f);
        _cameraPoint.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void UpdateYawPitch()
    {
        // Set the yaw and pitch position
        _yaw += _lookDir.x * _rotateSpeed_X;
        _pitch -= _lookDir.y * _rotateSpeed_Y;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }
}
