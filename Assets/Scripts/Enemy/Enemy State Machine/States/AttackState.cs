using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float moveTimer; // Value to tell the enemy AI to move slightly to make them harder to hit and add some movement variation
    private float losePlayerTimer; // How long the enemy will remain in the attack state before they start searching for player
    private float shotTimer;
    
    public override void Enter()
    {
        
    }

    public override void Perform()
    {
        // Make sure that the enemy still sees the player
        if (enemy.CanSeePlayer())
        {
            // Lock the lose player timer upon being seen, increments the move and shot timers
            losePlayerTimer = 0;
            moveTimer += Time.deltaTime;
            shotTimer += Time.deltaTime;
            enemy.transform.LookAt(enemy.Player.transform); // Locks into the player while they are visible AND in Attack State

            if(shotTimer > enemy.fireRate)
            {
                Shoot();
            }

            if(moveTimer > Random.Range(3, 7))
            {
                enemy.Agent.SetDestination(enemy.transform.position + (Random.insideUnitSphere * 5));
                moveTimer = 0;
            }
        }
        else
        {
            losePlayerTimer += Time.deltaTime;
            if(losePlayerTimer > 0.1f)
            {
                // Change to search state
                stateMachine.ChangeState(new PatrolState());
            }
        }
    }

    public override void Exit()
    {
        
    }

    public void Shoot()
    {
        Transform gunbarrel = enemy.gunBarrel;

        // Calculate direction toward player
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.position).normalized;

        // Create rotation that faces the player
        Quaternion bulletRotation = Quaternion.LookRotation(shootDirection);

        // Instantiate bullet with correct rotation
        GameObject bullet = GameObject.Instantiate(
            Resources.Load("Prefabs/Bullet") as GameObject,
            gunbarrel.position,
            bulletRotation
        );

        // Optional: add slight random spread
        Quaternion spread = Quaternion.Euler(
            Random.Range(-3f, 3f),
            Random.Range(-3f, 3f),
            0f
        );

        // Apply velocity
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = spread * shootDirection * 40f;

        Debug.Log("Shooting Player");
        shotTimer = 0;
    }
}
