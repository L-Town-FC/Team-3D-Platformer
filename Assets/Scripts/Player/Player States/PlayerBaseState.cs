using UnityEngine;

public abstract class PlayerBaseState
{
    protected virtual float stateEnterTime { get; set; }

    public virtual void EnterState(PlayerV2 player, PlayerBaseState newPlayerState) {
        stateEnterTime = Time.time;
    }

    public abstract void UpdateState(PlayerV2 player);

    public abstract void ExitState(PlayerV2 player);

    public virtual void ChangeState(PlayerV2 player, PlayerBaseState newPlayerState)
    {
        ExitState(player);
        EnterState(player, newPlayerState);
    }
}
