using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollision : MonoBehaviour
{
    private PlayerState playerState;

    private void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        playerState.ChangePlayerState(EPlayerState.Dead);
    }
}
