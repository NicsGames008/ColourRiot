using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCaught : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;

    private PlayerState playerState;
    private float elapsedTime;

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
            deathScreen.SetActive(true);
            Countdown();
        }
    }

    // Handles the progress of the tagging
    private void Countdown()
    {
        elapsedTime += Time.deltaTime;
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        if (seconds == 5)
        {
            SceneManager.LoadScene("Apartment");
        }
    }
}
