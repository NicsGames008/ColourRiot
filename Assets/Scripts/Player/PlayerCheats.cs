using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCheats : MonoBehaviour
{
    [Header("Tag Assets")]
    [SerializeField] private List<Tag> neighborhoodCheatTags;
    [SerializeField] private List<Tag> trainStationCheatTags;

    private bool isInvulnerable = false;
    private bool isVisableToCops = false;
    private bool unlimitedStamina = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (neighborhoodCheatTags.Count > 0)
            {
                AlbumManager.Instance.AddCheatTags(neighborhoodCheatTags);
                Debug.Log("lets gooooooo");
            }
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (trainStationCheatTags.Count > 0)
            {
                AlbumManager.Instance.AddCheatTags(trainStationCheatTags);
                Debug.Log("trainnn");
            }
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
            Debug.Log("Invulnerability toggled: " + isInvulnerable);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            isVisableToCops = !isVisableToCops;
            Debug.Log("Visible To Cops toggled: " + isVisableToCops);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            unlimitedStamina = !unlimitedStamina;
            Debug.Log("Unlimited Stamina toggled: " + unlimitedStamina);
        }
    }

    public bool GetIsInvulnerable()
    {
        return isInvulnerable;
    }

    public bool GetIsVisableToCops()
    {
        return isVisableToCops;
    }

    public bool GetUnlimitedStamina()
    {
        return unlimitedStamina;
    }
}
