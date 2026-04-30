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
        else if (_ctx.GetSlideInput.WasPressedThisFrame())
        {
            SwitchState(_init.Sliding());
        }
    }

    public override void EnterState()
    {
        Debug.Log("Player is currently idle!");
    }

    public override void ExitState()
    {
        _ctx.GetRigidbody.useGravity = true;
    }

    public override void FixedUpdateState()
    {
        _ctx.GetRigidbody.linearVelocity = new Vector3(0, _ctx.GetRigidbody.linearVelocity.y, 0);
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        if (_ctx.IsOnSlope)
        {
            _ctx.GetRigidbody.linearVelocity = Vector3.zero;
            _ctx.GetRigidbody.useGravity = false; 
        }
        else
        {
            _ctx.GetRigidbody.useGravity = true;
        }

        CheckSwitchState();
    }
}
