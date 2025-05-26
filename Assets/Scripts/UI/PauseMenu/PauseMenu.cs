using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPause = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject albumUI;

    private PlayerState playerState;
    private EPlayerState currentPlayerState;
    private SceneLoadManager sceneLoadManager; // Add this reference

    private void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
        // Get reference to SceneLoadManager
        sceneLoadManager = FindObjectOfType<SceneLoadManager>();
    }

    void Update()
    {
        // Check if scene is loading before processing pause input
        if (sceneLoadManager != null && SceneLoadManager.IsLoading) return;

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
        albumUI.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPause = false;
        if (currentPlayerState == EPlayerState.Moving)
        {
            playerState.ChangePlayerState(EPlayerState.Moving);
        }
        else if (currentPlayerState == EPlayerState.InWall)
        {
            playerState.ChangePlayerState(EPlayerState.InWall);
        }
    }

    void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPause = true;
        currentPlayerState = playerState.GetPlayerstate();
        playerState.ChangePlayerState(EPlayerState.Paused);
    }

    public void OpenAlbum()
    {
        Debug.Log("Open Album....");
        pauseMenuUI.SetActive(false);
        albumUI.SetActive(true);
    }

    public void OpenMainMenu()
    {
        Debug.Log("Open Main Menu....");
    }
}