using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public InputActionAsset InputActions;
    public Camera mainCamera;
    public StateMachine stateMachine { get; private set; }
    public Player_IdleState playerIdle { get; private set; }
    public Player_MoveState playerMove { get; private set; }
    public Player_JumpState playerJump { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }

    public float WalkSpeed = 5f;
    public float RotateSpeed = 10f;

    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;
    public bool JumpPressed => jumpAction.WasPressedThisFrame();

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        moveAction = InputActions.FindActionMap("Player").FindAction("Move");
        jumpAction = InputActions.FindActionMap("Player").FindAction("Jump");

        stateMachine = new StateMachine();
        playerIdle = new Player_IdleState(stateMachine, "Idle", this);
        playerMove = new Player_MoveState(stateMachine, "Move", this);
        playerJump = new Player_JumpState(stateMachine, "Jump", this);

    }

    private void OnEnable()
    {
        InputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        InputActions.FindActionMap("Player").Disable();
    }

    private void Start()
    {
        stateMachine.Initalize(playerIdle);
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        stateMachine.UpdateActiveState();
    }
}
