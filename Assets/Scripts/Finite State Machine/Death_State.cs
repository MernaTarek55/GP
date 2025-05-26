using UnityEngine;

public class Death_State : EntityState
{
    private GameObject entityGO;
    //private Animator animator;
    //private string animationTrigger = "Die";
    private float destroyDelay = 2f; // Adjust based on animation length

    public Death_State(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
        this.entityGO = player.gameObject;
        //this.animator = player.animator;
    }

    public Death_State(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
        : base(stateMachine, stateName, enemyData, enemyGO)
    {
        this.entityGO = enemyGO;
        //this.animator = enemyGO.GetComponent<Animator>();
    }

    public override void Enter()
    {
        base.Enter();

        //if (animator != null)
        //{
        //    animator.SetTrigger(animationTrigger);
        //}

        Object.Destroy(entityGO, destroyDelay);
    }
}
