using System;
using UnityEngine;

public class MovingState : BaseState
{
    public MovingState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    public override void CheckSwitchState()
    {
        if (_ctx.GetMoveDir == Vector2.zero)
        {
            SwitchState(_init.Idle());
        }
        else if (_ctx.GetSlideInput.WasPressedThisFrame())
        {
            SwitchState(_init.Sliding());
        }
    }

    public override void EnterState()
    {
        Debug.Log("I am moving!");
    }

    public override void ExitState()
    {
        // Note: We can actually add deceleration and acceleration!
        _ctx.GetRigidbody.linearVelocity = new Vector3(0, _ctx.GetRigidbody.linearVelocity.y, 0);
    }

    public override void FixedUpdateState()
    {
        // movement logic here
        ApplyMovement();
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    void ApplyMovement()
    {
        float y = _ctx.GetRigidbody.linearVelocity.y;
        Vector3 player_movement = (_ctx.transform.forward * _ctx.GetMoveDir.y + _ctx.transform.right * _ctx.GetMoveDir.x);

        _ctx.BonusSpeed = Math.Clamp(_ctx.BonusSpeed, _ctx.GetMinBonusSpeed, _ctx.GetMaxBonusSpeed);
        player_movement *= (_ctx.GetPlayerSpeed + _ctx.BonusSpeed);

        Vector3 move = player_movement * Time.fixedDeltaTime;

        _ctx.GetRigidbody.linearVelocity = new Vector3(player_movement.x, y, player_movement.z);
        _ctx.PrevMoveDir = new Vector2(_ctx.GetMoveDir.x, _ctx.GetMoveDir.y);
        //_rb.AddForce(player_movement * 2.5f, ForceMode.Force);
    }
}
