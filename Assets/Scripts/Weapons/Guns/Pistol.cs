using UnityEngine;

public class Pistol : BaseWeapon
{
    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        _impulseSource.GenerateImpulse(1f);

        if (_muzzleFlash != null)
            _muzzleFlash.Play();

        RaycastHit hit;

        Vector3 endPoint;

        if (Physics.Raycast(ray, out hit, _data.range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                Debug.Log("Hit an Enemy!");

                enemy.Damage(_data.damage);
            }
            Debug.Log("Hit w/ Pistol: " + hit.transform.name);

            endPoint = hit.point;
        }
        else endPoint = ray.origin + ray.direction * _data.range;

        // Show Laser
        _laserEndPoint = endPoint;
        _laserTimer = _laserDuration;
        _isLaserActive = true;

        _laserLine.enabled = true;
    }
}
