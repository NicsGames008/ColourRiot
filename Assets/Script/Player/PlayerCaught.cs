using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCaught : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;

    private PlayerState playerState;

    // Start is called before the first frame update
    void Start()
    {
       playerState = GetComponent<PlayerState>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (playerState.GetPlayerstate() == EPlayerState.Dead)
        {
            Debug.Log("Player Died");
            Time.timeScale = 0;
            deathScreen.SetActive(true);
        }
    }
}
