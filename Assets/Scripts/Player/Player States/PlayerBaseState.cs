using UnityEngine;

public abstract class PlayerBaseState
{
    protected virtual float stateEnterTime { get; set; }

    public abstract void EnterState(PlayerV2 player);

    public abstract void UpdateState(PlayerV2 player);

    public abstract void ExitState(PlayerV2 player);
}
