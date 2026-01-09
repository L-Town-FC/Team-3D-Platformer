using UnityEngine;

public class PlayerV2 : ActorController
{
    PlayerBaseState currentPlayerState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        currentPlayerState.UpdateState(this);

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
