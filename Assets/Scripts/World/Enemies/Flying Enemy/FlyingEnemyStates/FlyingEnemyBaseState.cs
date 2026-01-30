using UnityEngine;

public abstract class FlyingEnemyBaseState
{
    protected virtual float stateEnterTime { get; set; }

    public abstract void EnterState(FlyingEnemy enemy);
    public abstract void UpdateState(FlyingEnemy enemy);
    public abstract void ExitState(FlyingEnemy enemy);

}
