using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System.Net;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(CinemachineImpulseSource))]
[RequireComponent(typeof(LineRenderer))]
public abstract class BaseWeapon : MonoBehaviour
{
    // Weapon Variables
    [Header("Weapon Data")]
    [SerializeField] protected WeaponData _data;
    [SerializeField] protected Transform _firePoint;
    protected LineRenderer _laserLine;
    [SerializeField] protected ParticleSystem _muzzleFlash;
    [SerializeField] protected Transform _muzzlePoint;
    [SerializeField] protected float _laserDuration = 0.05f;
    [SerializeField] private float _tracerWidth = 0.02f;

    [Header("Weapon Bob")]
    [SerializeField] private float _minBobSpeed = 1f;
    [SerializeField] private float _maxBobSpeed = 1.5f;
    [SerializeField] private float _minBobAmount = 0.002f;
    [SerializeField] private float _maxBobAmount = 0.004f;
    private Vector3 _bobOffset;

    public string GetFireMode { get { return _data.fireMode.ToString(); } }

    private Vector3 _originalScale;
    private Vector3 _originalLocalPosition;

    private bool _isBurstShotAllowed = false;

    protected int _currentAmmo;
    protected float _nextTimeToFire;
    protected bool _isReloading;

    private PlayerContext _player;

    protected CinemachineImpulseSource _impulseSource;

    // Laser Line
    protected bool _isLaserActive;
    protected float _laserTimer;
    protected Vector3 _laserEndPoint;
    [SerializeField] private Gradient _beamColor;

    private void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _laserLine = GetComponent<LineRenderer>();

        _laserLine.startWidth = _tracerWidth;
        _laserLine.endWidth = _tracerWidth;
    }

    protected virtual void Awake()
    {
        _originalScale = this.transform.localScale;
        _originalLocalPosition = this.transform.localPosition;

        Debug.Log(_originalScale);
        Initialize();
        GunAwake();
    }

    protected virtual void GunAwake()
    {

    }

    protected virtual void LateUpdate()
    {
        Vector3 parentScale = transform.parent.lossyScale;

        // Fix scale
        transform.localScale = new Vector3(
            _originalScale.x / parentScale.x,
            _originalScale.y / parentScale.y,
            _originalScale.z / parentScale.z
        );

        // --- 2. COMPUTE BASE POSITION (scaled correctly) ---
        Vector3 basePosition = new Vector3(
            _originalLocalPosition.x / parentScale.x,
            _originalLocalPosition.y / parentScale.y,
            _originalLocalPosition.z / parentScale.z
        );


        CheckLaserEffect();

        ApplyWeaponBob();

        transform.localPosition = basePosition + _bobOffset;
    }

    protected virtual void Initialize()
    {
        GameObject target = GameObject.FindWithTag("Hit Target");

        if (target == null)
        {
            Debug.LogWarning("Could not find fire point");
            return;
        }

        _firePoint = target.transform;

        if (_firePoint == null)
        {
            Debug.LogWarning("Could not get the transform component from the game object");
        }

        _player = GameObject.FindWithTag("Player").GetComponent<PlayerContext>();

        _currentAmmo = _data.magazineSize;
    }

    public virtual void TryFire()
    {
        if (_isReloading) return;
        if (Time.time < _nextTimeToFire) return;
        if (_currentAmmo <= 0) return;

        switch (_data.fireMode)
        {
            case FireMode.Semi:
            case FireMode.Auto:
                Fire();
                _nextTimeToFire = Time.time + 1f / _data.fireRate;
                break;

            case FireMode.Burst:
                if(!_isBurstShotAllowed) StartCoroutine(BurstFire());
                break;
        }
    }

    protected virtual void Fire()
    {
        Debug.Log("firing gun!");

        if (_data.fireMode != FireMode.Burst) SFXManager.instance.PlayRandomSoundFXClip(_data.gunSFX, transform);
        else SFXManager.instance.PlayRandomSoundFXClip(_data.gunSFX, transform, 1f / _data.burstCount);

        _currentAmmo--;

        for (int i = 0; i < _data.pellets; i++)
        {
            if (_data.kickBackForce != 0) _player.GetRigidbody.AddForce(-_player.transform.forward * _data.kickBackForce, ForceMode.Impulse);

            Vector3 direction = GetSpreadDirection();
            ExecuteShot(direction);
        }
    }

    protected Vector3 GetSpreadDirection()
    {
        Vector3 direction = _firePoint.forward;

        if (_data.spreadAngle > 0)
        {
            direction = Quaternion.Euler(
                Random.Range(-_data.spreadAngle, _data.spreadAngle),
                Random.Range(-_data.spreadAngle, _data.spreadAngle),
                0
            ) * direction;
        }

        return direction;
    }

    public abstract void ExecuteShot(Vector3 direction);

    protected virtual IEnumerator BurstFire()
    {
        _isBurstShotAllowed = true;

        for (int i = 0; i < _data.burstCount; i++)
        {
            if (_currentAmmo <= 0) break;

            Fire();
            yield return new WaitForSeconds(1f / _data.burstFireRate);
        }

        _nextTimeToFire = Time.time + 1f / _data.fireRate;
        _isBurstShotAllowed = false;
    }

    public virtual IEnumerator Reload()
    {
        Debug.Log("reloading");

        if (_isReloading) yield break;
        if (_currentAmmo == _data.magazineSize) yield break;

        _isReloading = true;

        yield return new WaitForSeconds(_data.reloadTime);

        _currentAmmo = _data.magazineSize;

        _isReloading = false;
    }

    void ApplyWeaponBob()
    {
        float directionalInfluence = 0.0008f;

        float airFactor = _player.IsGrounded ? 1f : 0.2f;
        Vector2 flatVel = new Vector2(_player.GetRigidbody.linearVelocity.x, _player.GetRigidbody.linearVelocity.z);

        float movementSpeedFactor = Mathf.Clamp01(flatVel.magnitude / _player.GetMaxBonusSpeed);

        Vector3 moveDir = _player.GetMoveDir;

        float speedT = Mathf.Pow(movementSpeedFactor, 0.6f);

        float bobSpeed = Mathf.Lerp(_minBobSpeed, _maxBobSpeed, speedT);
        float bobAmount = Mathf.Lerp(_minBobAmount, _maxBobAmount, Mathf.Pow(speedT, 1.2f));

        float time = Time.time * bobSpeed;

        // base motion
        float x = Mathf.Sin(time) * bobAmount;
        float y = Mathf.Cos(time * 0.5f) * bobAmount;

        // subtle noise (actually used now)
        float noise = (Mathf.PerlinNoise(time, 0f) - 0.5f);

        Vector3 baseBob = new Vector3(
            x + noise * bobAmount * 0.3f,
            y + noise * bobAmount * 0.5f,
            0f
        );

        Vector3 right = _player.transform.right;
        Vector3 forward = _player.transform.forward;

        Vector3 moveBob =
            right * moveDir.x * directionalInfluence +
            forward * moveDir.z * directionalInfluence;

        _bobOffset = (baseBob + moveBob) * airFactor;
    }

    void CheckLaserEffect()
    {
        if (_isLaserActive)
        {
            _laserTimer -= Time.deltaTime;

            _laserLine.SetPosition(0, _muzzlePoint.position);
            _laserLine.SetPosition(1, _laserEndPoint);

            float t = 1f - (_laserTimer / _laserDuration); // 0 -> 1 over time

            Color color = _beamColor.Evaluate(t);

            _laserLine.startColor = color;
            _laserLine.endColor = color;

            if (_laserTimer <= 0f)
            {
                _isLaserActive = false;
                _laserLine.enabled = false;
            }
        }
    }
}