using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    public EnemyBaseState activeState;

    public void Initialize()
    {
        ChangeState(new PatrolState());
    }

    private void Update()
    {
        if (activeState != null)
        {
            activeState.Perform();
        }
    }

    public void ChangeState(EnemyBaseState newState)
    {
        // Checks if activeState != null
        if(activeState != null)
        {
            // Run cleanup on activeState
            activeState.Exit();
        }
        
        // Change to a new state
        activeState = newState;

        // Another null check to make sure the new state isn't empty
        if(activeState != null)
        {
            // Set new state
            activeState.stateMachine = this;
            activeState.enemy = GetComponent<Enemy>();
            
            // Assign state
            activeState.Enter();
        }
    }
}
