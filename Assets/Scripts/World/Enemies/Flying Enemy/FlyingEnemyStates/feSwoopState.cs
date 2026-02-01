using UnityEngine;

public class feSwoopState : FlyingEnemyBaseState
{
    //TODO: Fix swooping

    float maxSwoopTime = 3f;
    Vector3 finalSwoopPosition;
    float initialWaitTime = 1f;
    float bufferDst = 0.2f;
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        finalSwoopPosition = GetFinalSwoopPosition(enemy);

        enemy.ChangeSpeed(enemy.swoopSpeed, enemy.swoopSpeed + 1f);

        Debug.Log("Entering Swoop");
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        enemy.ChangeSpeed(enemy.circlingSpeed, enemy.circlingSpeed);
        Debug.Log("Exiting Swoop");
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        if(Time.time > stateEnterTime + initialWaitTime)
        {
            enemy.UpdateActorInputVectors(Vector3.zero, enemy.player.position - enemy.transform.position);
            finalSwoopPosition = GetFinalSwoopPosition(enemy);
        }
        else
        {
            Swoop(enemy);
        }

        Debug.DrawLine(enemy.transform.position, finalSwoopPosition, Color.green);

        if(Time.time > stateEnterTime + maxSwoopTime + initialWaitTime)
        {
            enemy.ChangeState(enemy.circlingState);
        }

        if(Vector3.Distance(finalSwoopPosition, enemy.transform.position) < bufferDst)
        {
            enemy.ChangeState(enemy.circlingState);
        }
    }

    Vector3 GetFinalSwoopPosition(FlyingEnemy enemy)
    {
        Vector3 enemyToPlayerDir = Vector3.ProjectOnPlane(enemy.player.position - enemy.transform.position, Vector3.up);

        Vector3 finalPosition = enemy.player.position + enemyToPlayerDir.normalized * enemy.circlingRadius * 2f;

        return finalPosition;
    }

    void Swoop(FlyingEnemy enemy)
    {
        Vector3 movementInput = finalSwoopPosition - enemy.transform.position;
        Vector3 newForward = movementInput;

        enemy.UpdateActorInputVectors(movementInput, newForward);
    }


}
