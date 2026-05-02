using UnityEngine;

public class Sniper : BaseWeapon
{
    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

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
