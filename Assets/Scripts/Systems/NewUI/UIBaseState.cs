using UnityEngine;

public abstract class UIBaseState
{
    protected virtual float stateEnterTime { get; set; }
    protected abstract BaseMenu UIMenu { get; set; }

    public abstract void EnterState(UIStateManager ui);

    public abstract void UpdateState(UIStateManager ui);

    public abstract void ExitState(UIStateManager ui);
}
