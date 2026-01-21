using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    //TODO: Crouching doesnt keep your legs on ground
    float gracePeriod = 0.1f;
    public override void EnterState(Player player)
    {
        stateEnterTime = Time.time;
        player.transform.localScale = new Vector3(1f, 0.5f, 1f);
        player.transform.position -= Vector3.up;
    }

    public override void ExitState(Player player)
    {
        player.transform.localScale = Vector3.one;
    }

    public override void UpdateState(Player player)
    {
        Vector3 newMovement = player.CamRelativeInputVector();
        Vector3 newForward = newMovement;

        player.UpdateActorInputVectors(newMovement, newForward);

        //include a small grace period so the player doesnt instantly change to ground state while still going to crouch
        if (!player.isPlayerGrounded && Time.time > stateEnterTime + gracePeriod)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

        if (!player.input.isCrouching)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        if (player.input.isJump && PlayerPrefs.HasKey(PowerUpList.SuperJump))
        {
            player.ChangeState(player.pSuperJumpState);
        }
    }
}
