using UnityEngine;

public class PlayerGroundState : PlayerBaseState
{
    PlayerInput input;
    public override void EnterState(PlayerV2 player)
    {
        stateEnterTime = Time.time;
        player.currentPlayerState = this;
        input = player.GetComponent<PlayerInput>();
    }

    public override void ExitState(PlayerV2 player)
    {
        Debug.Log("Should Change state here");
    }

    public override void UpdateState(PlayerV2 player)
    {
        Debug.Log("In Ground State: " + Time.time);
        if(Time.time > 5f)
        {
            player.ChangeState(player.testState);
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
