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
        // Store reference to the gun barrel
        Transform gunbarrel = enemy.gunBarrel;

        // Instantiate new bullet
        GameObject bullet = GameObject.Instantiate(Resources.Load("Prefabs/Bullet") as GameObject, gunbarrel.position, enemy.transform.rotation);

        // Calculate direction towards player
        Vector3 shootDirection = (enemy.Player.transform.position - gunbarrel.transform.position).normalized;

        // Add force rigidbody of the bullet
        bullet.GetComponent<Rigidbody>().linearVelocity = Quaternion.AngleAxis(Random.Range(-3f, 3f), Vector3.up) * shootDirection * 40; // NOTE: 40 is the speed of the bullet

        Debug.Log("Shooting Player");
        shotTimer = 0; // Resets shot timer
    }
}
