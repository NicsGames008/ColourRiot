using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenBtns : MonoBehaviour
{
    [SerializeField] private SceneLoadManager sceneLoadManager;

    public void LoadApartemnt()
    {
        //SceneManager.LoadScene("Apartment");
        StartCoroutine(sceneLoadManager.LoadSceneAsynchronously("Apartment", "MainScreen"));
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
