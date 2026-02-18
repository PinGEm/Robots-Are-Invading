using UnityEditor.SceneManagement;

public abstract class BaseState
{
    protected PlayerContext _ctx;
    protected StateInitialization _init;

    protected BaseState _currentSubState;
    protected BaseState _currentSuperState;

    public BaseState(PlayerContext ctx, StateInitialization init)
    {
        _ctx = ctx;
        _init = init;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

    public abstract void InitializeSubState();

    void UpdateStates()
    {

    }

    protected void SwitchState(BaseState nextState)
    {
        ExitState();

        nextState.EnterState();

        _ctx.CurrentState = nextState;
    }

    protected void SetSuperState(BaseState nextSuperState)
    {
        _currentSuperState = nextSuperState;
    }

    protected void SetSubState(BaseState nextSubState)
    {
        _currentSubState = nextSubState;
        nextSubState.SetSuperState(this);
    }
}
