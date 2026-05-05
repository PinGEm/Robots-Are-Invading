using UnityEngine;
using Unity.Cinemachine;
using System.Net;

public class Sniper : BaseWeapon
{
    private CinemachineImpulseSource _playerShake;

    protected override void GunAwake()
    {
        GameObject player = GameObject.FindWithTag("Player");

        PlayerContext context = player.GetComponent<PlayerContext>();

        _playerShake = context.GetImpulseSource;

        if (_playerShake != null) Debug.Log("Successfully found impulse source");
        else Debug.Log("Did not find impulse source");
    }

    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        _impulseSource.GenerateImpulse(1.5f);
        _playerShake.GenerateImpulse(1f);
        if (_muzzleFlash != null)
            _muzzleFlash.Play();

        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, _data.range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        Vector3 endPoint = ray.origin + ray.direction * _data.range;

        Debug.DrawRay(ray.origin, ray.direction * _data.range, Color.white, 2f);

        float currentDamage = _data.damage;
        int penetrated = 0;


        foreach (RaycastHit hit in hits)
        {
            if (penetrated >= _data.maxPenetrationTargets)
                break;

            Enemy enemy = hit.collider.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.Damage(_data.damage); 
            }

            endPoint = hit.point;
            penetrated++;
        }

        // Show Laser
        _laserEndPoint = endPoint;
        _laserTimer = _laserDuration;
        _isLaserActive = true;

        _laserLine.enabled = true;
    }
}
