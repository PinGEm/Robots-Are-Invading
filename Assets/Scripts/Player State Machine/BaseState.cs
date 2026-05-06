
public abstract class BaseState
{
    protected bool _rootState = false;

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

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    public void FixedUpdateStates()
    {
        FixedUpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.FixedUpdateStates();
        }
    }

    protected void SwitchState(BaseState nextState)
    {
        ExitState();

        nextState.EnterState();

        if (_rootState)
        {
            _ctx.CurrentState = nextState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(nextState);
        }
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
