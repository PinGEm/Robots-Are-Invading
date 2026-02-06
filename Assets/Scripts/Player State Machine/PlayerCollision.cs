using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private MeshRenderer _meshRenderer;

    private void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        int targetCollisionLayer = LayerMask.NameToLayer("Pillar"); // layer 7
        Color tempColor;
        Material collidingObjectMaterial;

        if (collision.gameObject.layer == targetCollisionLayer)
        {
            MeshRenderer renderer = collision.gameObject.GetComponent<MeshRenderer>();
            collidingObjectMaterial = renderer.material;
            tempColor = collidingObjectMaterial.color;

            collidingObjectMaterial.color = _meshRenderer.material.color;
            _meshRenderer.material.color = tempColor;
        }
    }
}
