using UnityEngine;

public class PlayerIdleAirState : PlayerBaseState
{
    public override void EnterState(PlayerV2 player)
    {
        stateEnterTime = Time.time;
        Debug.Log("Entering Air State");
    }

    public override void ExitState(PlayerV2 player)
    {
        Debug.Log("Exiting Air State");
    }

    public override void UpdateState(PlayerV2 player)
    {
        Vector3 newMovement = player.CamRelativeInputVector();
        Vector3 newForward = newMovement;

        player.UpdateActorInputVectors(newMovement, newForward);

        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        if (player.OnWallCheck().Item1)
        {
            player.ChangeState(player.pWallState);
            return;
        }
    }
}
