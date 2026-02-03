using UnityEngine;

public class feIdleState : FlyingEnemyBaseState
{
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
    }

    public override void ExitState(FlyingEnemy enemy)
    {
    }

    public override void UpdateState(FlyingEnemy enemy)
    {
        if (enemy.isAlertedToPlayer)
        {
            enemy.ChangeState(enemy.circlingState);
        }
    }
}
