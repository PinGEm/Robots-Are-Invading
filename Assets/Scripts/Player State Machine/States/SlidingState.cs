using System;
using UnityEngine;

public class SlidingState : BaseState
{
    public SlidingState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    private float _downwardsForce = 0.1f;
    private float _slideCounter;

    private float _slideSpeed = 0.7f;
    private float _slideTime = 1f;

    public override void CheckSwitchState()
    {
        Debug.Log("Checking switch states");

        if (_ctx.GetSlideInput.WasReleasedThisFrame() || _slideCounter >= _ctx.GetSlideTime)
        {
            if (_ctx.IsGrounded)
            {
                SwitchState(_init.Grounded());
            }

            if (!_ctx.IsGrounded)
            {
                SwitchState(_init.Airborne());
            }
        }
    }

    public override void EnterState()
    {
        _ctx.SpeedQueue.Add(Tuple.Create(_slideSpeed, _slideTime));

        _ctx.transform.localPosition = new Vector3(_ctx.transform.localPosition.x, _ctx.transform.localPosition.y - 0.5f, _ctx.transform.localPosition.z);
        _ctx.transform.localScale = new Vector3(1, 0.5f, 1);

        Debug.Log("currently in sliding state");

        ApplySlideForce();
    }

    public override void ExitState()
    {
        Debug.Log("exiting sliding state");
        _slideCounter = _ctx.GetSlideTime;

        _ctx.GetSlideCooldown = true;
        _ctx.transform.localScale = new Vector3(1, 1, 1);
    }

    public override void FixedUpdateState()
    {
        if (_ctx.IsOnSlope)
        {
            if (_ctx.GetRigidbody.linearVelocity.y > 0)
            {
                _ctx.GetRigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        _slideCounter += Time.deltaTime;

        CheckSwitchState();
    }

    void ApplySlideForce()
    {
        Debug.Log("Applying force");

        _ctx.GetRigidbody.AddForce((_ctx.transform.forward.normalized * _ctx.PrevMoveDir.y + _ctx.transform.right.normalized *
            _ctx.PrevMoveDir.x + (-_ctx.transform.up * _downwardsForce)) * ( (_ctx.GetPlayerSpeed + 
            Mathf.Clamp(_ctx.BonusSpeed,_ctx.GetMinBonusSpeed,_ctx.GetMaxBonusSpeed) ) + _ctx.GetSlideBoost), ForceMode.Impulse);
    }
}