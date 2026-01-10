using UnityEngine;

public class PlayerV2 : ActorController
{
    public PlayerBaseState currentPlayerState;
    public Vector3 playerInputVector = Vector3.zero;
    public Vector3 playerTransformForward;

    public PlayerGroundState GroundState;
    public TestState testState;

    private void Awake()
    {
        GroundState = new PlayerGroundState();
        testState = new TestState();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.inputVector = playerInputVector;
        base.newTransformForward = playerTransformForward;

        GroundState.EnterState(this);

        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        currentPlayerState.UpdateState(this);

        base.inputVector = Vector3.zero;
        base.newTransformForward = playerTransformForward;

        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        currentPlayerState.ExitState(this);
        currentPlayerState = newState;
        currentPlayerState.EnterState(this);
    }
}
