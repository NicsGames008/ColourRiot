using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    private EPlayerState playerState;

    public void ChangePlayerState(EPlayerState newstate)
    {
        playerState = newstate;
    }

    public EPlayerState GetPlayerstate()
    {
        return playerState;
    }
}
