using UnityEngine;

public class IdleState : BaseState
{
    public IdleState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    public override void CheckSwitchState()
    {
        if (_ctx.GetMoveDir != Vector2.zero)
        {
            SwitchState(_init.Moving());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Player is currently idle!");
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}
