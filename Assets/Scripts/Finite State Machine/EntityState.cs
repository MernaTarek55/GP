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
        //every time we need to enter to a new state, enter will be called.
        Debug.Log("I enter " + stateName);
    }

    public virtual void Update()
    {
        //Logic of the current State.
        Debug.Log("I am in " + stateName);
    }

    public virtual void Exit()
    {
        //every time we need to leave to a new state, exit will be called.
        Debug.Log("I exit " + stateName);
    }

  

}
