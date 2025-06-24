using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections;

public class Player : MonoBehaviour
{
    public InputActionAsset InputActions;
    public Camera mainCamera;
    public Image deadEyeCooldownImage;
    public Image InvisibilityCooldownImage;
    public StateMachine stateMachine { get; private set; }
    public Player_IdleState playerIdle { get; private set; }
    public Player_MoveState playerMove { get; private set; }
    public Player_JumpState playerJump { get; private set; }
    public Player_DeadEyeStateTest1 playerDeadEye { get; private set; }
    public Death_State playerDeath { get; private set; }
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }
    public PlayerHealthComponent healthComponent { get; private set; }

    public float WalkSpeed = 5f;
    public float RotateSpeed = 10f;

    public Transform groundCheckPoint;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    public bool IsGrounded => Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    public bool hasJumped = false;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction deadEyeAction;
    private Vector2 moveInput;
    public Vector2 MoveInput => moveInput;
    public bool JumpPressed { get; private set; }
    public bool DeadEyePressed { get; private set; }
    public bool IsShooting { get; private set; }

    public float deadEyeDuration = 10f;
    public float deadEyeCooldown = 30f;//inventory.getPlayerStat(PlayerSkillsStats.DeadEyeCoolDown);
    public float lastDeadEyeTime = -Mathf.Infinity;
    public bool CanUseDeadEye => Time.time >= lastDeadEyeTime + deadEyeCooldown;
    [SerializeField]GameObject pistol;
    [Header("Invisibility Settings")]
    [SerializeField]GameObject invisibilityBtn;
    [Header("DeadEye Settings")]
    [SerializeField] GameObject DeadEyeBtn;
    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float deceleration = 15f;
    public float maxSpeed = 5f;
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float walkSpeed = 2f;
    public float runSpeed = 5f;

    [SerializeField] private SkinnedMeshRenderer renderer;
    [SerializeField] private Material dissolveMaterial;
    [SerializeField] private float dissolveSpeed = 0.5f;

    [HideInInspector]
    public Vector3 currentVelocity = Vector3.zero;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        healthComponent = gameObject.GetComponent<PlayerHealthComponent>();
        //healthComponent.setMaxHealth(inventory.getPlayerStat(PlayerSkillsStats.MaxHealth));
        moveAction = InputActions.FindActionMap("Player").FindAction("Move");
        jumpAction = InputActions.FindActionMap("Player").FindAction("Jump");
        //deadEyeAction = InputActions.FindActionMap("Player").FindAction("DeadEye");

        stateMachine = new StateMachine();
        playerIdle = new Player_IdleState(stateMachine, "Idle", this);
        playerMove = new Player_MoveState(stateMachine, "Move", this);
        playerJump = new Player_JumpState(stateMachine, "Jump", this);
       // playerDeadEye = new Player_DeadEyeStateTest1(stateMachine, "DeadEye", this);
        playerDeath = new Death_State(stateMachine, "Death", this);
        //playerDeadEye = new Player_DeadEyeStateTest1(stateMachine, "DeadEye", this);
        //Hello
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
        //DeadEyePressed = deadEyeAction.WasPressedThisFrame() && CanUseDeadEye;
        JumpPressed = jumpAction.WasPressedThisFrame() && IsGrounded && !hasJumped;
        stateMachine.UpdateActiveState();
    }
    //TODO: Put it invisibility 
    public void ActivateInvisibility()
    {
        GetComponent<InvisibilitySkill>().enabled = true; 
        invisibilityBtn.SetActive(true); 
    }

    public void ActivateDeadEye()
    {
        DeadEyeBtn.SetActive(true);
    }
    public void ActivatePistol()
    {
        pistol.SetActive(true);
    }
    public void SetShooting(bool isShooting)
    {
        IsShooting = isShooting;
    }

    public void ResetHealth()
    {
        healthComponent.RenewHealth();
    }

    public void ChangeMaterial()
    {
        if (renderer == null) return;

        renderer.material = dissolveMaterial;

        StartCoroutine(EnemyDissolve());
    }

    public IEnumerator EnemyDissolve()
    {
        float dissolve = 0f;
        dissolveMaterial.SetFloat("_Dissolve", dissolve);

        while (dissolve < 100f)
        {
            dissolve += Time.deltaTime * dissolveSpeed;
            dissolveMaterial.SetFloat("_Dissolve", dissolve);
            yield return null; // wait for the next frame
        }
        dissolveMaterial.SetFloat("_Dissolve", 0f);
    }
}
