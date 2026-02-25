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

        /*RaycastHit hit;

        if (Physics.Raycast(_firePoint.transform.position, _firePoint.transform.forward, out hit, _data.range))
        {
            Debug.Log("Hit: " + hit.transform.name);
        }

        Debug.DrawRay(_firePoint.transform.position, _firePoint.transform.forward * _data.range, Color.red);*/


        /*GameObject projectile = Instantiate(
            _projectilePrefab,
            _firePoint.position,
            _firePoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(_firePoint.forward * _projectileForce, ForceMode.Impulse);*/
    }
}
