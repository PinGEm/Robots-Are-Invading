public abstract class BaseState
{
    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void FixedUpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchState();

    public abstract void InitializeSubState();

    void UpdateStates()
    {

    }

    void SwitchState()
    {

    }

    void SetSuperState()
    {

    }

    void SetSubState()
    {

    }
}
