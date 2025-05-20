using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(StarterAssetsInputs))]
[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
    StarterAssetsInputs _input;
    ThirdPersonController _controller;
    public StateMachine stateMachine { get; private set; }
    public Player_IdleState playerIdle { get; private set; }
    public Player_MoveState playerMove { get; private set; }
    public Player_JumpState playerJump { get; private set; }

    public CharacterController Controller { get; private set; }
    public StarterAssetsInputs Input { get; private set; }
    public Animator Animator { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 2.0f;
    public float sprintSpeed = 5.335f;
    public float rotationSmoothTime = 0.12f;
    public float jumpHeight = 1.2f;
    public float gravity = -15.0f;

    [Header("Timed Dulation Variables")]
    public float currentSpeed;
    public float verticalVelocity;
    public bool isGrounded;

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        Input = GetComponent<StarterAssetsInputs>();
        Animator = GetComponent<Animator>();

        stateMachine = new StateMachine();
        playerIdle = new Player_IdleState(stateMachine, "Idle", this);
        playerMove = new Player_MoveState(stateMachine, "Move", this);
        playerJump = new Player_JumpState(stateMachine, "Jump", this);

    }

    private void Start()
    {
        stateMachine.Initalize(playerIdle);
    }

    private void Update()
    {
        isGrounded = Controller.isGrounded;

        stateMachine.UpdateActiveState();

        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Controller.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }
}
