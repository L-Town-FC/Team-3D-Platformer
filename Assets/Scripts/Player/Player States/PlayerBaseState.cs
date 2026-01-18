using UnityEngine;

public abstract class PlayerBaseState
{
    protected virtual float stateEnterTime { get; set; }

    public abstract void EnterState(Player player);

    public abstract void UpdateState(Player player);

    public abstract void ExitState(Player player);
}
