using UnityEngine;

public class feCirclingState : FlyingEnemyBaseState
{
    public override void EnterState(FlyingEnemy enemy)
    {
        stateEnterTime = Time.time;
        Debug.Log("Entering Circling");
    }

    public override void ExitState(FlyingEnemy enemy)
    {
        Debug.Log("Leaving Circling");
    }

    public override void UpdateState(FlyingEnemy enemy)
    {

    }
}
