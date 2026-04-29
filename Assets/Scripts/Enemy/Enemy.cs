using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private EnemyStateMachine stateMachine;
    private GameObject player;
    private NavMeshAgent agent;
    public NavMeshAgent Agent { get => agent; }
    public GameObject Player { get => player; }
    public EnemyPath path;

    [Header("Sight Values")]
    public float sightDistance = 20f; // Turn to 99999f if we are doing the AI hivemind thingy
    public float fieldOfView = 85f;
    public float eyeHeight = 0.6f;

    [Header("Weapon Values")]
    public Transform gunBarrel;
    [Range(0.1f,10f)]
    public float fireRate;

    [SerializeField] private string currentState; // Debugging thingy

    [SerializeField] private float maxHealth = 5f;
    [SerializeField] private AudioClip damageSoundClip;

    private float currentHealth;
    public bool hasTakenDamage{get; set;}

    private void Start()
    {
        stateMachine = GetComponent<EnemyStateMachine>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine.Initialize();
        player = GameObject.FindGameObjectWithTag("Player"); // Calls only once

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

    private void Update()
    {
        CanSeePlayer();
        currentState = stateMachine.activeState.ToString();
    }

    public bool CanSeePlayer()
    {
        if (player != null)
        {
            // Checks if the player is in range
            if(Vector3.Distance(transform.position, player.transform.position) < sightDistance)
            {
                Vector3 targetDirection = player.transform.position - transform.position - (Vector3.up * eyeHeight); 
                float angleToPlayer = Vector3.Angle(targetDirection, transform.forward); // Calculates angle of the player

                if(angleToPlayer >= -fieldOfView && angleToPlayer <= fieldOfView) // Checks if angle is within the field of view of the enemy
                {
                    // Checks if the enemy line of sight is blocked by an obstacle
                    Ray ray = new Ray(transform.position + (Vector3.up * eyeHeight), targetDirection);
                    RaycastHit hitInfo = new RaycastHit();

                    if(Physics.Raycast(ray, out hitInfo, sightDistance))
                    {
                        if(hitInfo.transform.gameObject == player)
                        {
                            // Draw line in scene mode to visualize enemy vision
                            Debug.DrawRay(ray.origin, ray.direction * sightDistance);
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}
