using UnityEngine;

public class PlayerAliveState : BaseState
{
    public PlayerAliveState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    public override void CheckSwitchState()
    {
        if (_ctx.IsDead)
        {
            SwitchState(_init.Death());
        }
    }

    public override void EnterState()
    {
        Debug.Log("I am currently alive!");
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
        if (_ctx.GetDashInput.WasPressedThisFrame() && _ctx.EnableDash)
        {
            SetSubState(_init.Dashing());
        }

        if (_ctx.IsGrounded)
        {
            SetSubState(_init.Grounded());
        }

        if (!_ctx.IsGrounded)
        {
            SetSubState(_init.Airborne());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}