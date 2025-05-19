using StarterAssets;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerInputSet starterAssetsInputs;
    public StateMachine stateMachine { get; private set; }
    public Player_IdleState playerIdle { get; private set; }
    public Player_MoveState playerMove { get; private set; }
    public Vector2 moveInput { get; private set; }

    private void Awake()
    {
        starterAssetsInputs = new PlayerInputSet();
        stateMachine = new StateMachine();
        playerIdle = new Player_IdleState(stateMachine, "Idle", this);
        playerMove = new Player_MoveState(stateMachine, "Move", this);
    }
    private void OnEnable()
    {
        starterAssetsInputs.Enable();
        starterAssetsInputs.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        starterAssetsInputs.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        starterAssetsInputs.Disable();
    }

    private void Start()
    {
        stateMachine.Initalize(playerIdle);
    }

    private void Update()
    {
        stateMachine.currentState.Update();
    }
}
