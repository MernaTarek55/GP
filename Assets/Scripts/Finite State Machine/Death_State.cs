using UnityEngine;
using static EnemyData;

public class Death_State : EntityState
{
    private GameObject entityGO;
    private Animator animator;
    private string animationTrigger = "Die";
    private float destroyDelay = 2f; // Adjust based on animation length
    private HealthComponent healthComponent;

    //private Vector3[] dropForce;
    //private GameObject drop;
    //private float dropChance;
    //private int minDrops;
    //private int maxDrops;

    public Death_State(StateMachine stateMachine, string stateName, Player player)
        : base(stateMachine, stateName, player)
    {
        this.entityGO = player.gameObject;
        //this.animator = player.animator;
    }



    public Death_State(StateMachine stateMachine, string stateName, EnemyData enemyData, Enemy enemy)
       : base(stateMachine, stateName, enemyData, enemy)
    {
        this.entityGO = enemy.gameObject;
        this.enemy = enemy;
        TryGetComponents(entityGO);


        //enemy.getDropProperties(drop, dropChance, minDrops, maxDrops, dropForce);

        //this.animator = enemyGO.GetComponent<Animator>();
    }

    public override void Enter()
    {
        base.Enter();

        if (animator != null)
        {
            animator.SetTrigger(animationTrigger);
        }

        SpawnDrops();
        enemy?.ChangeEnemyMaterial(); // to make the enemy dissolve after death
        enemy?.PlayDeathEffect(); // if you want both effects uncomment
        player?.ChangeMaterial(); // to make the enemy dissolve after death
       // player?.PlayDeathEffect(); // if you want both effects uncomment

        //this line


        if (entityGO.CompareTag("Player"))
        {
           // player.GetComponent<PlayerRespawn>().Respawn();
        }
        else
        {
            Object.Destroy(entityGO, destroyDelay);
        }
    }

    #region Enemy Drops 
    //TODO: make the drop properties private and make a getter
    private void SpawnSingleDrop(GameObject itemPrefab)
    {
        if (PoolManager.Instance == null)
        {
            Debug.LogWarning("PoolManager instance not found!");
            _ = GameObject.Instantiate(itemPrefab, entityGO.transform.position + Vector3.up, Quaternion.identity);
            return;
        }

        _ = entityGO.transform.position + Vector3.up;
        GameObject drop = PoolManager.Instance.SpawnFromPool(PoolType.DropItem,
            entityGO.transform.position + Vector3.up,
            Quaternion.identity
        );

        if (drop.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(new Vector3(
                Random.Range(enemy.dropForce[0].x, enemy.dropForce[1].x),
                Random.Range(enemy.dropForce[0].y, enemy.dropForce[1].y),
                Random.Range(enemy.dropForce[0].z, enemy.dropForce[1].z)
            ), ForceMode.Impulse);
        }
    }

    public void SpawnDrops()
    {


        if (entityGO.CompareTag("Player"))
        {
            Debug.LogWarning("Entity GameObject is player.");
            return;
        }
        if (enemy.drop == null)
            return;


        if (Random.value > enemy.dropChance)
            return;

        int amount = Random.Range(enemy.minDrops, enemy.maxDrops + 1);

        for (int i = 0; i < amount; i++)
        {
            if (enemy.drop != null || enemy != null)
            {
                SpawnSingleDrop(enemy.drop);
            }
        }
    }

    #endregion

    private void TryGetComponents(GameObject entityGO)
    {
        if (entityGO.CompareTag("Player"))
        {
            return;
        }
        else
        {
            if (entityGO.TryGetComponent(out Enemy enemy)) this.enemy = enemy;
            else Debug.LogWarning("enemy not found");
        }

        if (entityGO.TryGetComponent(out HealthComponent healthComponent)) this.healthComponent = healthComponent;
        else Debug.LogWarning("healthComponent not found");

        animator = entityGO.GetComponentInChildren<Animator>();

    }
}