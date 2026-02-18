using UnityEngine;

public class JumpingState : BaseState
{
    public JumpingState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    public override void CheckSwitchState()
    {
        throw new System.NotImplementedException();
    }

    public override void EnterState()
    {
        Debug.Log("I am jumping!");
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }
}