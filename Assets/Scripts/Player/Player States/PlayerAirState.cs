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
        Vector3 newInput = CamRelativeInputVector(player.input);
        Vector3 newForward = newInput;

        player.UpdateActorInputVectors(newInput, newForward);

        if (player.isPlayerGrounded)
        {
            player.ChangeState(player.pGroundState);
            return;
        }
    }

    //Converts input vector from world vector to input vector based on the camera
    //makes movement feel better
    Vector3 CamRelativeInputVector(PlayerInput playerInput)
    {
        //    Your old line was:
        //      base.inputVector = Camera.main.transform.TransformDirection(input.movementInput.normalized);
        //
        //    That can introduce a NEGATIVE Y when the camera is tilted down,
        //    which fights your stomp bounce. So we project onto the XZ plane.
        Vector3 camRelative = Camera.main.transform.TransformDirection(playerInput.movementInput.normalized);
        camRelative = Vector3.ProjectOnPlane(camRelative, Vector3.up).normalized; // <-- NEW
        return camRelative;
    }
}
