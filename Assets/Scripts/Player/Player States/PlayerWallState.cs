using UnityEngine;

public class PlayerWallState : PlayerBaseState
{
    float defaultGravityModifier;
    float wallGravityModifier = 0f;
    float defaultAirDrag;
    float wallAirDrag = 1;
    float gravityModifierMaxChangeTime = 2f;
    Vector3 wallJumpDir = Vector3.zero;
    
    public override void EnterState(PlayerV2 player)
    {
        defaultGravityModifier = player.downwardGravityModifier;
        defaultAirDrag = player.airDrag;
        player.airDrag = wallAirDrag;
        stateEnterTime = Time.time;
        wallJumpDir = player.OnWallCheck().Item2;
        Debug.Log("Entering Wall State");
    }

    public override void ExitState(PlayerV2 player)
    {
        player.downwardGravityModifier = defaultGravityModifier;
        player.airDrag = defaultAirDrag;
        Debug.Log("Exiting Wall State");
    }

    public override void UpdateState(PlayerV2 player)
    {
        //Make player always face away from wall

        //Stop lateral movement and movement towards wall
        Vector3 newInput = ProjectOnWallMovement(player.input.movementInput);
        Vector3 newForward = wallJumpDir;

        player.downwardGravityModifier = Mathf.Lerp(
            wallGravityModifier,
            defaultGravityModifier,
            Mathf.InverseLerp(stateEnterTime, stateEnterTime + gravityModifierMaxChangeTime, Time.time)
        );

        player.UpdateActorInputVectors(newInput, newForward);

        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }

        if (player.input.wasJumpPressedThisFrame)
        {
            player.ChangeState(player.pJumpState);
            return;
        }

        if (!player.OnWallCheck().Item1)
        {
            player.ChangeState(player.pIdleAirState);
            return;
        }

    }

    Vector3 ProjectOnWallMovement(Vector3 input)
    {
        Vector3 horInputVector = new Vector3(input.x, 0f, input.z); //remove the vertical component

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
