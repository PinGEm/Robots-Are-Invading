using UnityEngine;

public class Pistol : BaseWeapon
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _projectileForce = 1000f;

    public override void ExecuteShot(Vector3 direction)
    {
        GameObject projectile = Instantiate(
            _projectilePrefab,
            _firePoint.position,
            _firePoint.rotation
        );

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(_firePoint.forward * _projectileForce, ForceMode.Impulse);
    }
}
