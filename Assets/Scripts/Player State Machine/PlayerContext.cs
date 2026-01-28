using System;
using System.Collections;
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

    private const float MAX_FALL_SPEED = 18;
    private const float JUMP_APEX_THRESHOLD = 0.185f; // temporary implementation if apex hanging
    private const float SPEED_BOOST_SLIDE_TIMER = 2f;

    [Header("Movement Variables")]
    [SerializeField] private int _playerSpeed = 11;
    [SerializeField] private float _jumpForce = 7f;
    [SerializeField] private float _slideBoost = 2.65f;
    [SerializeField] private float _fallMultiplier = 2.5f;
    [SerializeField] private float _lowJumpMultiplier = 4f;
    private bool _startApexTimer;
    private float _apexCounter;
    private float _bonusSpeed;
    private bool _grounded; // TEMPORARY

    [Header("Sensitivity")]
    [SerializeField] private float _rotateSpeed_X = 60f;
    [SerializeField] private float _rotateSpeed_Y = 65f;
    [SerializeField] Transform _cameraPoint;
    private float _pitch;
    private float minPitch = -80f;
    private float maxPitch = 80f;

    [Header("Miscellaneous")]
    [SerializeField] private GameObject _temporaryObject;
    private Rigidbody _rb;
    Vector2 _moveDir = Vector2.zero;
    Vector2 _lookDir = Vector2.zero;


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

        MouseLook();

        if (_attackAction.WasPressedThisFrame())
        {
            GameObject temp = Instantiate(_temporaryObject);

            temp.transform.position = this.transform.position;
        }

        if (_jumpAction.WasPressedThisFrame() && _grounded) Jump();

        if (_slideAction.WasPressedThisFrame()) Sliding();
        if (_slideAction.WasReleasedThisFrame()) CancelSlide();

        if (_startApexTimer) _apexCounter += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        Vector3 player_movement = (transform.forward * _moveDir.y + transform.right * _moveDir.x);


        _bonusSpeed = Math.Clamp(_bonusSpeed, 0, 25);
        player_movement *= _playerSpeed + _bonusSpeed;

        ApplyBetterGravity();

        _rb.linearVelocity = new Vector3(player_movement.x, _rb.linearVelocity.y, player_movement.z);
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

    void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        _startApexTimer = true;
        _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        _bonusSpeed += 1;
    }

    void MouseLook()
    {
        // Yaw
        float yaw = _lookDir.x * _rotateSpeed_X * Time.deltaTime;
        transform.Rotate(0f, yaw, 0f);

        // Pitch
        _pitch -= _lookDir.y * _rotateSpeed_Y * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        _cameraPoint.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    void ResetApex()
    {
        _startApexTimer = false;
        _bonusSpeed -= 1;
        _apexCounter = 0;
    }

    void Sliding()
    {
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

    // ---   TEMPORARY GROUND COLLISION CHECKS    --- \\
    private void OnCollisionStay(Collision collision)
    {
        ResetApex();
        _grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        _grounded = false;
    }
}
