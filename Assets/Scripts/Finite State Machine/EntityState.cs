using UnityEngine;

public abstract class EntityState
{
    protected ExampleScript example;
    protected EnemyData enemyData;
    protected GameObject enemyGO;
    

    protected Player player;
    protected StateMachine stateMachine;
    protected string stateName;

    /*
     * Rigid Body
     * Animator
     * Player/Enemy Stats, also if we are using PlayerPrefs still searching it up
     * ETC.....
     */
    public EntityState(StateMachine stateMachine, string stateName, Player player)
    {
        this.stateMachine = stateMachine;
        this.stateName = stateName;
        this.player = player;
    }


    public EntityState(StateMachine stateMachine, string stateName, ExampleScript exp)
    {
        this.stateMachine = stateMachine;
        this.stateName = stateName;
        this.example = exp;
    }

    // public EntityState(StateMachine stateMachine, string stateName, EnemyData enemyData)
    // {
    //     this.stateMachine = stateMachine;
    //     this.stateName = stateName;
    //     this.enemyData = enemyData;
    // }

    public EntityState(StateMachine stateMachine, string stateName, EnemyData enemyData, GameObject enemyGO)
    {
        this.stateMachine = stateMachine;
        this.stateName = stateName;
        this.enemyData = enemyData;
        this.enemyGO = enemyGO;
    }

    public virtual void Enter()
    {
        Debug.Log("I enter " + stateName);
    }

    public virtual void Update()
    {
        Debug.Log("I am in " + stateName);

        if (enemyData != null)
        {
            switch (enemyData.enemyType)
            {
                case 0: // Turret
                    UpdateTurret();
                    break;

                case (EnemyData.EnemyType)1: // BallDroid
                    UpdateBallDroid();
                    break;

                case (EnemyData.EnemyType)2: // Humanoid
                    UpdateHumanoid();
                    break;
                case (EnemyData.EnemyType)3: // Humanoid
                    UpdateLavaRobot();
                    break;
            }
        }
        else
        {
            Debug.LogWarning("EnemyData is Null");
        }
    }

    public virtual void Exit()
    {
        Debug.Log("I exit " + stateName);
    }

    protected virtual void UpdateTurret() { }
    protected virtual void UpdateBallDroid() { }
    protected virtual void UpdateHumanoid() { }
    protected virtual void UpdateLavaRobot() { }
}