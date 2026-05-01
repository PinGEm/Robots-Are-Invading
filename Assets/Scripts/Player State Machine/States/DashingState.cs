using System;
using UnityEngine;

public class DashingState : BaseState
{
    public DashingState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    private const float SLIDE_BUFFER_CUTOFF = 0.275f;

    private float _dashCounter;
    private float _dashAmplifier = 3f;

    private float _dashSpeed = 1.5f;
    private float _dashTime = 1.15f;

    private float _slideBufferCounter = 0;


    public override void CheckSwitchState()
    {
        if (_dashCounter >= _ctx.GetDashTime)
        {
            if (_ctx.IsGrounded || _ctx.IsOnSlope)
            {
                SwitchState(_init.Grounded());
            }

            if (!_ctx.IsGrounded)
            {
                SwitchState(_init.Airborne());
            }

            if (_ctx.GetSlideInput.IsPressed() && _slideBufferCounter <= SLIDE_BUFFER_CUTOFF && !_ctx.GetSlideCooldown)
            {
                SwitchState(_init.Sliding());
            }
        }
    }

    public override void EnterState()
    {
        _ctx._dashEffect.Play();
        _ctx.GetCamera.Lens.FieldOfView = _ctx.GetOriginalFOV + 10;
        _ctx.SpeedQueue.Add(Tuple.Create(_dashSpeed, _dashTime));
        ApplyDashForce();
        SFXManager.instance.PlayRandomSoundFXClip(_ctx._dashingSFX, _ctx.transform, 0.725f);
        Debug.Log("I am dashing!");
    }

    public override void ExitState()
    {
        _ctx.GetCamera.Lens.FieldOfView = _ctx.GetOriginalFOV;
        Debug.Log("Exiting Dash State");
        _ctx.GetRigidbody.linearVelocity = Vector3.zero;
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        if (_ctx.GetSlideInput.IsPressed())
        {
            _slideBufferCounter += Time.deltaTime;
        }
        else
        {
            _slideBufferCounter = 0;
        }

        CheckSwitchState();
        _dashCounter += Time.deltaTime;
    }

    private void ApplyDashForce()
    {
        _ctx.GetImpulseSource.GenerateImpulse(2);
        Vector3 forceDirection = _ctx.transform.forward.normalized * (_ctx.GetDashForce * _dashAmplifier + ((_ctx.GetPlayerSpeed + _ctx.BonusSpeed) / 2.5f));

        if (_ctx.PrevMoveDir != Vector2.zero) forceDirection = (_ctx.transform.forward.normalized * 
                _ctx.PrevMoveDir.y + _ctx.transform.right.normalized * _ctx.PrevMoveDir.x) * (_ctx.GetDashForce * _dashAmplifier);

        Debug.Log(forceDirection);
        _ctx.GetRigidbody.AddForce(forceDirection, ForceMode.Impulse);
    }
}
