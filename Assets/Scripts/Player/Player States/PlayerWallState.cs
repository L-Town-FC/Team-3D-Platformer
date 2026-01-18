using UnityEngine;

public class PlayerWallState : PlayerBaseState
{
    //TODO: May need to add bounce check here so the player can slide down wall into a bounce

    float defaultGravityModifier;
    float wallGravityModifier = 0f;
    float defaultAirDrag;
    float wallAirDrag = 1f;
    float gravityModifierMaxChangeTime = 2f;
    Vector3 wallJumpDir = Vector3.zero;
    
    public override void EnterState(Player player)
    {
        defaultGravityModifier = player.downwardGravityModifier;
        defaultAirDrag = player.airDrag;
        player.airDrag = wallAirDrag;
        stateEnterTime = Time.time;
        wallJumpDir = player.OnWallCheck().Item2;
    }

    public override void ExitState(Player player)
    {
        player.downwardGravityModifier = defaultGravityModifier;
        player.airDrag = defaultAirDrag;
    }

    public override void UpdateState(Player player)
    {
        //Make player always face away from wall
        //Stop lateral movement and movement towards wall
        Vector3 newInput = ProjectOnWallMovement(player.input.movementInput, player);
        Vector3 newForward = wallJumpDir;

        //starts player with low gravity that increases over time
        //this gives the appearance of sticking to the wall and then slowly losing grip
        player.downwardGravityModifier = Mathf.Lerp(
            wallGravityModifier,
            defaultGravityModifier,
            Mathf.InverseLerp(stateEnterTime, stateEnterTime + gravityModifierMaxChangeTime, Time.time)
        );

        player.UpdateActorInputVectors(newInput, newForward);

        //wall check is overrided if the player is touching the ground
        //this lets player run along ground next to wall without getting stuck
        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        //check if the jump was started while the player was on the wall
        //dont want player to instantly wall jump if they are still holding jump key from a previous jump
        if (player.input.wasJumpPressedThisFrame)
        {
            player.ChangeState(player.pJumpState);
            return;
        }

        //if they are no longer on the wall, and all previous checks failed they must be in the air
        if (!player.OnWallCheck().Item1)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

    }

    Vector3 ProjectOnWallMovement(Vector3 input, Player player)
    {
        
        Vector3 horInputVector = player.CamRelativeInputVector();

        //wall jump normal is directly away from wall
        //since when on the wall we only care about vertical movement and
        //movement away from the wall, the horizontal vector is projected onto the wallJumpDir
        //so only movement away from the wall remains
        Vector3 projHorInputVector = Vector3.Project(horInputVector, wallJumpDir);

        //check if the new horizontal input vector is in the same direction as the wall normal
        //if not, the player shouldnt try to move towards the wall so cancel all horizontal input
        float dot = Vector3.Dot(projHorInputVector, wallJumpDir);
        if (dot < 0f)
        {
            projHorInputVector = Vector3.zero;
        }

        //the vertical component is then readded to the projected vector
        return new Vector3(projHorInputVector.x, input.y, projHorInputVector.z);
    }
}
