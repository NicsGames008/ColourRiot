using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCheats : MonoBehaviour
{
    [SerializeField] private Tag tagToCheatIntoAlbumNeighbotHood;
    [SerializeField] private Tag tagToCheatIntoAlbumTrainStation;

    private Album album;

    private bool isInvulnerable = false;

    // Start is called before the first frame update
    void Start()
    {
        album = GetComponent<Album>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Album.Instance.Add(tagToCheatIntoAlbumNeighbotHood);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            Album.Instance.Add(tagToCheatIntoAlbumTrainStation);
        }

        else if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("Appartment");
        }
        else if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("Neighborhood");
        }
        else if (Input.GetKeyDown(KeyCode.F3))
        {
            SceneManager.LoadScene("TrainStation");
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            isInvulnerable = !isInvulnerable;
        }
    }

    public bool GetIsInvulnerable()
    {
        return isInvulnerable;
    }
}
