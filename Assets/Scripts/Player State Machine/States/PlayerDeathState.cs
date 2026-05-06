using UnityEngine;

public class PlayerDeathState : BaseState
{
    public PlayerDeathState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) {
        _rootState = true;
        InitializeSubState();
    }

    public override void CheckSwitchState()
    {
        
    }

    public override void EnterState()
    {
        Time.timeScale = 0;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        _ctx.GetCurrentWeapon.gameObject.SetActive(false);

        Debug.Log("I am dead!");
    }

    public override void ExitState()
    {
        
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}