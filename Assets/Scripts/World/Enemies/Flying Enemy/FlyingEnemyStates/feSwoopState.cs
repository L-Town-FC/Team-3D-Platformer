using UnityEngine;

public class feSwoopState : FlyingEnemyBaseState
{
    float maxSwoopTime = 2.5f;
    Vector3 finalSwoopPosition;
    float initialWaitTime = 1f;
    float bufferDst = 0.2f;
    float verticalSpeedAdjustment = 1f;
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        finalSwoopPosition = GetFinalSwoopPosition(enemy);

        //speeds are different so enemy gets lower faster than it hits the player
        //this makes it easier to jump on the enemy
        enemy.ChangeSpeed(enemy.swoopSpeed, enemy.swoopSpeed + verticalSpeedAdjustment);

        Debug.Log("Entering Swoop");
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        enemy.ChangeSpeed(enemy.circlingSpeed, enemy.circlingSpeed);
        Debug.Log("Exiting Swoop");
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        //makes enemy halt for a predefined to let player know they are about to swoop
        if(Time.time < stateEnterTime + initialWaitTime)
        {
            enemy.UpdateActorInputVectors(Vector3.zero, enemy.player.position - enemy.transform.position);
            finalSwoopPosition = GetFinalSwoopPosition(enemy);
        }
        else
        {
            //starts swooping movement
            Swoop(enemy);
        }

        //Stops enemy from getting stuck swooping if the final swoop location is unreachable
        if(Time.time > stateEnterTime + maxSwoopTime + initialWaitTime)
        {
            enemy.ChangeState(enemy.circlingState);
        }

        //added buffer distance to final swoop position check to avoid floating point issues
        if(Vector3.Distance(finalSwoopPosition, enemy.transform.position) < bufferDst)
        {
            enemy.ChangeState(enemy.circlingState);
        }
    }

    Vector3 GetFinalSwoopPosition(FlyingEnemy enemy)
    {
        //Gets horizontal component of enemy to player direction
        Vector3 enemyToPlayerDir = Vector3.ProjectOnPlane(enemy.player.position - enemy.transform.position, Vector3.up);

        //creates swoop location that is on the other side of the player and slightly off the ground and past the circling radius
        Vector3 finalPosition = enemy.player.position + enemyToPlayerDir.normalized * enemy.circlingRadius * 1.5f + (Vector3.up * 0.5f);

        return finalPosition;
    }

    void Swoop(FlyingEnemy enemy)
    {
        Vector3 movementInput = finalSwoopPosition - enemy.transform.position;
        Vector3 newForward = movementInput;

        Debug.DrawLine(enemy.transform.position, finalSwoopPosition, Color.green);

        enemy.UpdateActorInputVectors(movementInput, newForward);
    }


}
