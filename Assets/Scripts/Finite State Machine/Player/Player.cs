using StarterAssets;
using UnityEngine;

public class Player : MonoBehaviour
{
    StarterAssetsInputs _input;
    ThirdPersonController _controller;
    public StateMachine stateMachine { get; private set; }
    public Player_IdleState playerIdle { get; private set; }
    public Player_MoveState playerMove { get; private set; }
    public Player_JumpState playerJump { get; private set; }
    public Vector2 MoveInput => _input.move;
    public bool IsGrounded => _controller.Grounded;
    public bool JumpTriggered => _input.jump;

    private void Awake()
    {
        stateMachine = new StateMachine();
        playerIdle = new Player_IdleState(stateMachine, "Idle", this);
        playerMove = new Player_MoveState(stateMachine, "Move", this);
        playerJump = new Player_JumpState(stateMachine, "Jump", this);

        _input = GetComponent<StarterAssetsInputs>();
        _controller = GetComponent<ThirdPersonController>();
    }

    private void Start()
    {
        stateMachine.Initalize(playerIdle);
    }

    private void Update()
    {
        stateMachine.UpdateActiveState();
    }
}
