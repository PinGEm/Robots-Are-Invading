using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private AudioClip damageSoundClip;

    private float currentHealth;

    public bool hasTakenDamage{get; set;}

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(float damageAmount)
    {
        hasTakenDamage = true;

        currentHealth -= damageAmount;

        // Play SFX When Damaged
        SFXScript.instance.PlaySoundFXClip(damageSoundClip, transform, 1f);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player collided an Enemy!");
            
            // Play SFX When Collided
            SFXScript.instance.PlaySoundFXClip(damageSoundClip, transform, 1f);
        }

        
    }
}
