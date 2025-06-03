using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private PlayerState playerState;
    private PlayerCheats playerCheats;

    private void Start()
    {
        playerState = gameManager.ReturnPlayer().GetComponent<PlayerState>();
        playerCheats = gameManager.ReturnPlayer().GetComponent<PlayerCheats>();
    }

private void OnCollisionEnter(Collision collision)
{
    if (collision.gameObject.CompareTag("Player") && !playerCheats.GetIsInvulnerable())
    {
        playerState.ChangePlayerState(EPlayerState.Dead);
    }
}

}
