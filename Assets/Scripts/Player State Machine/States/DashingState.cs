using UnityEngine;

public class DashingState : BaseState
{
    public DashingState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) { }

    private float _dashCounter;
    private float _dashAmplifier = 3f;

    public override void CheckSwitchState()
    {
        if (_dashCounter >= _ctx.GetDashTime)
        {
            if (_ctx.IsGrounded)
            {
                SetSubState(_init.Grounded());
            }

            if (!_ctx.IsGrounded)
            {
                SetSubState(_init.Airborne());
            }
        }
    }

    public override void EnterState()
    {
        ApplyDashForce();
        Debug.Log("I am dashing!");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Dash State");
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
        _dashCounter += Time.deltaTime;
    }

    private void ApplyDashForce()
    {
        _ctx.GetImpulseSource.GenerateImpulse(2);
        Vector3 forceDirection = _ctx.transform.forward.normalized * (_ctx.GetDashForce * _dashAmplifier);

        if (_ctx.GetPrevMoveDir != Vector2.zero) forceDirection = (_ctx.transform.forward.normalized * _ctx.GetPrevMoveDir.y + _ctx.transform.right.normalized * _ctx.GetPrevMoveDir.x) * (_ctx.GetDashForce * _dashAmplifier);

        Debug.Log(forceDirection);
        _ctx.GetRigidbody.AddForce(forceDirection, ForceMode.Impulse);
    }
}
