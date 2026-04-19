using UnityEngine;

public class JumpingState : BaseState
{
    private bool _jumpHeld;

    public JumpingState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) {
        InitializeSubState();
    }

    public override void CheckSwitchState()
    {
        if (_ctx.GetDashInput.WasPressedThisFrame() && _ctx.EnableDash)
        {
            SwitchState(_init.Dashing());
        }

        if (_ctx.GetRigidbody.linearVelocity.y < 0)
        {
            SwitchState(_init.Airborne());
        }

        if (_ctx.GetSlideInput.WasPressedThisFrame())
        {
            SwitchState(_init.Sliding());
        }
    }

    public override void EnterState()
    {
        Debug.Log("I am jumping!");
        _jumpHeld = false;
        ApplyUpWardsForce();
    }

    public override void ExitState()
    {
        _ctx.BonusSpeed -= 1;
    }

    public override void FixedUpdateState()
    {
        if (!_jumpHeld && _ctx.GetRigidbody.linearVelocity.y > 0)
        {
            _ctx.GetRigidbody.linearVelocity += Vector3.up *
                Physics.gravity.y *
                (_ctx.GetLowJumpMultiplier - 1) *
                Time.fixedDeltaTime;
        }
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
        _jumpHeld = _ctx.GetJumpInput.IsPressed();
        CheckSwitchState();
    }

    void ApplyUpWardsForce()
    {
        // Reset current velocity, then apply force
        _ctx.GetRigidbody.linearVelocity = new Vector3(_ctx.GetRigidbody.linearVelocity.x, 0f, _ctx.GetRigidbody.linearVelocity.z);
        _ctx.GetRigidbody.AddForce(Vector3.up * _ctx.GetJumpForce, ForceMode.Impulse);
        _ctx.BonusSpeed += 1;
    }
}