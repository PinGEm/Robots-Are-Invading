using UnityEngine;

public class GroundedState : BaseState
{
    public GroundedState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) {
        InitializeSubState();
    }

    public override void CheckSwitchState()
    {
        if (_ctx.GetDashInput.WasPressedThisFrame() && _ctx.EnableDash)
        {
            SwitchState(_init.Dashing());
        }

        if (_ctx.GetSlideInput.WasPressedThisFrame() && !_ctx.GetSlideCooldown)
        {
            SwitchState(_init.Sliding());
        }

        if (_ctx.GetJumpInput.WasPressedThisFrame())
        {
            SwitchState(_init.Jumping());
        }

        if (!_ctx.IsGrounded)
        {
            SwitchState(_init.Airborne());
        }
    }

    public override void EnterState()
    {
        _ctx.IsCoyoteTime = true;
        Debug.Log("I am grounded!");
    }

    public override void ExitState()
    {
        Debug.Log("exiting grounded state");
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        if (_ctx.GetSlideInput.WasPressedThisFrame() && _ctx.GetMoveDir != Vector2.zero)
        {
            SetSubState(_init.Sliding());
        }

        if (_ctx.GetMoveDir != Vector2.zero)
        {
            SetSubState(_init.Moving());
        }
        else
        {
            SetSubState(_init.Idle());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }
}