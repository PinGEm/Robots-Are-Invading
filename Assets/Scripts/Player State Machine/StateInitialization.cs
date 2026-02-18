using UnityEngine;

public class StateInitialization : MonoBehaviour
{
    PlayerContext _context;

    public StateInitialization(PlayerContext context)
    {
        _context = context;
    }

    public BaseState Alive()
    {
        return new PlayerAliveState(_context, this);
    }

    public BaseState Death()
    {
        return new PlayerDeathState(_context, this);
    }

    public BaseState Airborne()
    {
        return new AirborneState(_context, this);
    }

    public BaseState Falling()
    {
        return new FallingState(_context, this);
    }

    public BaseState Jumping()
    {
        return new JumpingState(_context, this);
    }

    public BaseState Grounded()
    {
        return new GroundedState(_context, this);
    }

    public BaseState Moving()
    {
        return new MovingState(_context, this);
    }

    public BaseState Idle()
    {
        return new IdleState(_context, this);
    }

    public BaseState Sliding()
    {
        return new SlidingState(_context, this);
    }

    public BaseState Dashing()
    {
        return new DashingState(_context, this);
    }
}
