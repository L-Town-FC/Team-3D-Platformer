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

        if (BounceCheck(player))
        {
            player.ChangeState(player.pBounceState);
            return;
        }

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

    bool BounceCheck(PlayerV2 _player)
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        return Physics.CheckSphere(_player.transform.position - (Vector3.up * 1.05f), 0.5f, enemyMask, QueryTriggerInteraction.Ignore);
    }
}
