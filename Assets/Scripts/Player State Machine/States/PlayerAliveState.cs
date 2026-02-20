using UnityEngine;

public class PlayerAliveState : BaseState
{
    public PlayerAliveState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) {
        _rootState = true;
        InitializeSubState();
    }

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
        
    }

    public override void InitializeSubState()
    {
        Debug.Log("initializing sub states");

        if (_ctx.GetDashInput.WasPressedThisFrame() && _ctx.EnableDash)
        {
            Debug.Log("Substate to dashing state");
            SetSubState(_init.Dashing());
        }

        if (_ctx.IsGrounded)
        {
            Debug.Log("Substate to grounded state");
            SetSubState(_init.Grounded());
        }

        if (!_ctx.IsGrounded)
        {
            Debug.Log("Substate to airborne state");
            SetSubState(_init.Airborne());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}