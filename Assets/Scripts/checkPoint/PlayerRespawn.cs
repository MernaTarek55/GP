using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    Player player;

    private Vector3 checkpointPosition;
    private Vector3 startPosition;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Start()
    {
        startPosition = transform.position;
        checkpointPosition = startPosition;
    }

    public void SetCheckpoint(Vector3 newCheckpoint)
    {
        checkpointPosition = newCheckpoint;
        Debug.Log("Checkpoint updated to: " + checkpointPosition);
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2.0f);
        transform.position = checkpointPosition;
        player.healthComponent.RenewHealth();
        player.stateMachine.ChangeState(player.playerIdle);
        player.animator.SetTrigger("BackToIdle");

        player.ChangeMaterial(true);
    }

    public void activateRespawn()
    {
        StartCoroutine(Respawn());
    }
}
