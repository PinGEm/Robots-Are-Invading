using UnityEngine;

public class AirborneState : BaseState
{
    public AirborneState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) {
        InitializeSubState();
    }

    private const float COYOTE_TIME_CUTOFF = 0.2f;

    private float _coyoteTimeCounter = 0;

    public override void CheckSwitchState()
    {
        if(_ctx.GetJumpInput.WasPressedThisFrame() && _coyoteTimeCounter <= COYOTE_TIME_CUTOFF && _ctx.IsCoyoteTime == true)
        {
            Debug.Log("Allow Coyote Time");
            SwitchState(_init.Jumping());
        }

        if (_ctx.GetDashInput.WasPressedThisFrame() && _ctx.EnableDash)
        {
            SwitchState(_init.Dashing());
        }

        if (_ctx.IsGrounded)
        {
            SwitchState(_init.Grounded());
        }

        if (_ctx.GetSlideInput.WasPressedThisFrame() && !_ctx.GetSlideCooldown)
        {
            SwitchState(_init.Sliding());
        }
    }

    public override void EnterState()
    {
        
        Debug.Log("I am airborne!");
    }

    public override void ExitState()
    {
        Debug.Log("exiting airborne state");
    }

    public override void FixedUpdateState()
    {
        if (_ctx.GetRigidbody.linearVelocity.y < 0)
        {
            _ctx.GetRigidbody.linearVelocity += Vector3.up * Physics.gravity.y * (_ctx.GetFallMultiplier - 1) * Time.fixedDeltaTime;

            // Clamp Fall Speed
            if (_ctx.GetRigidbody.linearVelocity.y < -_ctx.GetMaxFallSpeed)
            {
                _ctx.GetRigidbody.linearVelocity = new Vector3(_ctx.GetRigidbody.linearVelocity.x, -_ctx.GetMaxFallSpeed, 
                    _ctx.GetRigidbody.linearVelocity.z);
            }
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
        if (_ctx.IsCoyoteTime == true)
        {
            _coyoteTimeCounter += Time.deltaTime; 
        }

        CheckSwitchState();
    }
}