using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPause = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject albumUI;

    private PlayerState playerState;
    private EPlayerState currentPlayerState;
    private SceneLoadManager sceneLoadManager;

    private void Start()
    {
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
        sceneLoadManager = FindObjectOfType<SceneLoadManager>();
    }

    void Update()
    {
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
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainScreen");
    }
}