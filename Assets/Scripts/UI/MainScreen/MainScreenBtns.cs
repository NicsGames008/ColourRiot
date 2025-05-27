using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScreenBtns : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadApartemnt()
    {
        SceneManager.LoadScene("Apartment");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
