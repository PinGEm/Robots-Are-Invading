using UnityEngine;

public class PlayerAliveState : BaseState
{
    public PlayerAliveState(PlayerContext context, StateInitialization stateInitializer) : base(context, stateInitializer) {
        _rootState = true;
        InitializeSubState();
    }

    private const float TIME_TO_REACH_MAX_STAMINA = 3;
    private const float TIME_TO_START_STAMINA = 5;

    private bool _isRegenerating = false;
    private float _timeSinceLastUse = 0;
    private float _previousStaminaMeter = 0;


    public override void CheckSwitchState()
    {
        if (_ctx.IsDead)
        {
            SwitchState(_init.Death());
        }
    }

    public override void EnterState()
    {
        _previousStaminaMeter = _ctx.GetStaminaCount;
        Debug.Log("I am currently alive!");
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void FixedUpdateState()
    {
        
    }

    public override void InitializeSubState()
    {
        Debug.Log("initializing sub states");

        if (_ctx.GetDashInput.WasPressedThisFrame() && _ctx.EnableDash)
        {
            Debug.Log("Substate to dashing state");
            SetSubState(_init.Dashing());
        }

        if (_ctx.IsGrounded)
        {
            Debug.Log("Substate to grounded state");
            SetSubState(_init.Grounded());
        }

        if (!_ctx.IsGrounded)
        {
            Debug.Log("Substate to airborne state");
            SetSubState(_init.Airborne());
        }
    }

    public override void UpdateState()
    {

        // --- STAMINA SYSTEM --- \\

        // 1. Detect stamina usage (ONLY when decreasing)
        if (_ctx.GetStaminaCount < _previousStaminaMeter)
        {
            _timeSinceLastUse = 0f;
            _isRegenerating = false;
        }
        else
        {
            _timeSinceLastUse += Time.deltaTime;
        }

        _previousStaminaMeter = _ctx.GetStaminaCount;


        // 2. Force regen if empty
        if (_ctx.GetStaminaCount <= 0)
        {
            _ctx.IsStaminaMovementAllowed = false;
            _isRegenerating = true;
        }

        // 3. Start regen after delay
        if (!_isRegenerating && _timeSinceLastUse >= TIME_TO_START_STAMINA)
        {
            _isRegenerating = true;
        }

        // 4. Handle regeneration
        if (_isRegenerating)
        {
            _ctx.GetStaminaCount += (_ctx.GetMaxStamina / TIME_TO_REACH_MAX_STAMINA) * Time.deltaTime;
            _ctx.GetStaminaCount = Mathf.Min(_ctx.GetStaminaCount, _ctx.GetMaxStamina);
        }

        // 5. Fully recovered
        if (_ctx.GetStaminaCount >= _ctx.GetMaxStamina)
        {
            _ctx.IsStaminaMovementAllowed = true;
            _isRegenerating = false;
        }

        // --- STAMINA SYSTEM --- \\

        CheckSwitchState();
    }
}