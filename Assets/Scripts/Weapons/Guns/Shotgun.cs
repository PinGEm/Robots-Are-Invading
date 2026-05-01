using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class Shotgun : BaseWeapon
{
    private CinemachineImpulseSource _impulseSource;

    private void Start()
    {
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        _impulseSource.GenerateImpulse(1.25f);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _data.range))
        {
            Debug.Log("Hit w/ Shotgun: " + hit.transform.name);

            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, 1f);
        }
        else
        {
            Debug.Log("Missed");

            Debug.DrawRay(ray.origin, ray.direction * _data.range, Color.yellow, 1f);
        }
    }
}
