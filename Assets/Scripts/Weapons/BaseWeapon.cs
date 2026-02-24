using UnityEngine;
using System.Collections;

public abstract class BaseWeapon : MonoBehaviour
{
    // Weapon Variables
    [Header("Weapon Data")]
    [SerializeField] protected WeaponData _data;
    [SerializeField] protected Transform _firePoint;

    private Vector3 _originalScale;

    protected int _currentAmmo;
    protected int _reserveAmmo;
    protected float _nextTimeToFire;
    protected bool _isReloading;

    protected virtual void Awake()
    {
        _originalScale = this.transform.localScale;
        Initialize();
    }

    protected virtual void Update()
    {
        this.transform.localScale = _originalScale;
    }

    protected virtual void Initialize()
    {
        _currentAmmo = _data.magazineSize;
        _reserveAmmo = _data.maxReserveAmmo;
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
                StartCoroutine(BurstFire());
                break;
        }
    }

    protected virtual void Fire()
    {
        _currentAmmo--;

        for (int i = 0; i < _data.pellets; i++)
        {
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
        for (int i = 0; i < _data.burstCount; i++)
        {
            if (_currentAmmo <= 0) break;

            Fire();
            yield return new WaitForSeconds(1f / _data.fireRate);
        }

        _nextTimeToFire = Time.time + 1f / _data.fireRate;
    }

    public virtual IEnumerator Reload()
    {
        if (_isReloading) yield break;
        if (_currentAmmo == _data.magazineSize) yield break;
        if (_reserveAmmo <= 0) yield break;

        _isReloading = true;

        yield return new WaitForSeconds(_data.reloadTime);

        int ammoNeeded = _data.magazineSize - _currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, _reserveAmmo);

        _currentAmmo += ammoToLoad;
        _reserveAmmo -= ammoToLoad;

        _isReloading = false;
    }
}