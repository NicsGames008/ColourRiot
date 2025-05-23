using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private PlayerState playerState;
    private PlayerCheats playerCheats;

    private void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
        playerCheats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCheats>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!playerCheats.GetIsInvulnerable())
        {
            playerState.ChangePlayerState(EPlayerState.Dead);
        }
    }
}
