using UnityEngine;

public class Sniper : BaseWeapon
{
    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        // Apply spread if needed
        Vector3 finalDirection = direction; // or modify ray.direction if you want true ADS accuracy

        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, _data.range);

        // Sort hits by distance (VERY IMPORTANT)
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

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
        }
    }
}
