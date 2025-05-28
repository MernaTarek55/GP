using System.Collections.Generic;
using UnityEngine;

public class Player_DeadEyeStateTest1 : EntityState
{
    private float timer;
    private readonly List<GameObject> markedTargets = new();
    private float originalTimeScale;
    public Player_DeadEyeStateTest1(StateMachine stateMachine, string stateName, Player player) : base(stateMachine, stateName, player)
    {
    }

    public override void Enter()
    {
        base.Enter();

        timer = 0f;
        markedTargets.Clear();
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        player.lastDeadEyeTime = Time.unscaledTime;
        UpdateCooldownUI(0);  // Start at 0%
        Debug.Log("DeadEye activated!");
    }

    public override void Update()
    {
        base.Update();

        timer += Time.unscaledDeltaTime;
        UpdateCooldownUI(timer / player.deadEyeDuration);

        // Mark targets
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = player.mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject target = hit.collider.gameObject;
                if (!markedTargets.Contains(target) && target.CompareTag("Enemy"))
                {
                    markedTargets.Add(target);
                    Debug.Log("Target marked: " + target.name);
                }
            }
        }

        // End state
        if (timer >= player.deadEyeDuration)
        {
            Exit();
            ExecuteMarkedTargets();
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = 0.02f;
            stateMachine.ChangeState(new Player_IdleState(stateMachine, "Idle", player));
        }
    }

    public override void Exit()
    {
        base.Exit();
        UpdateCooldownUI(0);  // Reset UI for next fill
    }

    private void ExecuteMarkedTargets()
    {
        foreach (GameObject target in markedTargets)
        {
            Debug.Log("Executing target: " + target.name);
            // target.GetComponent<Target>()?.Explode();
        }
    }

    private void UpdateCooldownUI(float fillAmount)
    {
        if (player.deadEyeCooldownImage != null)
        {
            player.deadEyeCooldownImage.fillAmount = fillAmount;
        }
    }
}