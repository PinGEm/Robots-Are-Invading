using UnityEngine;

public class Pistol : BaseWeapon
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileForce = 1000f;

    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _data.range))
        {
            Debug.Log("Hit: " + hit.transform.name);
        }
    }
}
