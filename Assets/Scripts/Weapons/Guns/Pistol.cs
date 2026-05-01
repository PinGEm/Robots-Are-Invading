using UnityEngine;

public class Pistol : BaseWeapon
{
    public override void ExecuteShot(Vector3 direction)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _data.range))
        {
            Debug.Log("Hit w/ Pistol: " + hit.transform.name);
        }
    }
}
