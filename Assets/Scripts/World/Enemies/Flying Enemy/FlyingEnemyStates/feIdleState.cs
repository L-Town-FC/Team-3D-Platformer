using UnityEngine;

public class feIdleState : FlyingEnemyBaseState
{
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        Debug.Log("Entering Idle");

    }

    public override void ExitState(FlyingEnemy enemy)
    {
        Debug.Log("Leaving Idle");
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        if (enemy.isAlertedToPlayer)
        {
            enemy.ChangeState(enemy.circlingState);
        }
    }
}
