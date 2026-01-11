using UnityEngine;

public class PlayerV2 : ActorController
{
    public PlayerBaseState currentPlayerState;
    public bool isPlayerGrounded => isGrounded;
    public PlayerInput input;

    public PlayerGroundState pGroundState;
    public PlayerIdleAirState pIdleAirState;
    public PlayerJumpState pJumpState;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        pGroundState = new PlayerGroundState();
        pIdleAirState = new PlayerIdleAirState();
        pJumpState = new PlayerJumpState();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.inputVector = Vector3.zero;
        base.newTransformForward = transform.forward;

        currentPlayerState = pGroundState;
     
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

    public void ChangeState(PlayerBaseState _newState)
    {
        currentPlayerState.ExitState(this);
        currentPlayerState = _newState;
        currentPlayerState.EnterState(this);
    }

    public void UpdateActorInputVectors(Vector3 _inputVector, Vector3 _newForward)
    {
        base.inputVector = _inputVector;
        base.newTransformForward = _newForward;
    }
}
