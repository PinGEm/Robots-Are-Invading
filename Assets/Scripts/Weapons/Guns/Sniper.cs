using UnityEngine;
using Unity.Cinemachine;

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

        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, _data.range);

        Debug.DrawRay(ray.origin, ray.direction * _data.range, Color.white, 2f);

        float currentDamage = _data.damage;
        int penetrated = 0;

        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"Hit: {hit.transform.name} at distance {hit.distance}");

            // Draw a small line showing the surface normal
            Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 2f);

            penetrated++;

            if (penetrated >= _data.maxPenetrationTargets)
                break;

            // Damage Code Here
        }
    }
}
