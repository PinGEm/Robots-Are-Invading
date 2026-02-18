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
        return new PlayerAliveState();
    }

    public BaseState Death()
    {
        return new PlayerDeathState();
    }

    public BaseState Airborne()
    {
        return new AirborneState();
    }

    public BaseState Falling()
    {
        return new FallingState();
    }

    public BaseState Jumping()
    {
        return new JumpingState();
    }

    public BaseState Grounded()
    {
        return new GroundedState();
    }

    public BaseState Moving()
    {
        return new MovingState();
    }

    public BaseState Idle()
    {
        return new IdleState();
    }

    public BaseState Sliding()
    {
        return new SlidingState();
    }

    public BaseState Dashing()
    {
        return new DashingState();
    }
}
