using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    public override void EnterState(PlayerV2 player, PlayerBaseState newPlayerState)
    {
        stateEnterTime = Time.time;
    }

    public override void ExitState(PlayerV2 player)
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState(PlayerV2 player)
    {
        throw new System.NotImplementedException();
    }
}
