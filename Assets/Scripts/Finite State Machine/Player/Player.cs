using StarterAssets;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerInputSet starterAssetsInputs;
    public StateMachine stateMachine { get; private set; }
    public Player_IdleState playerIdle { get; private set; }

    private void Awake()
    {
        starterAssetsInputs = new PlayerInputSet();
        stateMachine = new StateMachine();
        playerIdle = new Player_IdleState(stateMachine, "Idle", this);
    }
    private void OnEnable()
    {
        starterAssetsInputs.Enable();
        starterAssetsInputs.Player.Move.performed += ctx => Debug.Log(ctx.ReadValue<Vector2>());
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
