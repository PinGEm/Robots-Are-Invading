using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using System.Net;

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

        // Fix position
        transform.localPosition = new Vector3(
            _originalLocalPosition.x / parentScale.x,
            _originalLocalPosition.y / parentScale.y,
            _originalLocalPosition.z / parentScale.z
        );


        // Show Laser Logic
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
}