using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPause = false;

    [SerializeField] private GameObject pauseMenuUI;

    private PlayerState playerState;

    private void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerState.GetPlayerstate() != EPlayerState.Dead)
        {
            if (gameIsPause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPause = false;
        playerState.ChangePlayerState(EPlayerState.Moving);

    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPause = true;
        playerState.ChangePlayerState(EPlayerState.Paused);
    }


    public void OpenAlbum()
    {
        Debug.Log("Open Album....");
    }

    public void OpenMainMenu()
    {
        Debug.Log("Open Main Menu....");
    }
}
