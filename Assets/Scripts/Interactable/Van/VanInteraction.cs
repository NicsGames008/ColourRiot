using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VanInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject visualFeedBackInteraction; // UI feedback object for interaction
    private GameObject currentUI; // Instance of the feedback UI
    private Album album;

    private int maxTags;
    private string levelAt;

    // Start is called before the first frame update
    void Start()
    {
        album = GameObject.FindGameObjectWithTag("Player").GetComponent<Album>();
        levelAt = SceneManager.GetActiveScene().name;
        maxTags = GameObject.FindGameObjectsWithTag("TaggableWall").Length;
    }

    // Update is called once per frame
    void Update()
    {
        int tagDoneOnTheLevel = 0;
        foreach (var tag in album.tags)
        {
            if (tag.levelUnlocked == levelAt)
            {
                tagDoneOnTheLevel++;
            }
        }
    }

    // Called when the player interacts with the tagging object
    public void Interact(GameObject Instigator)
    {
        int tagDoneOnTheLevel = 0;
        foreach (var tag in album.tags)
        {
            if (tag.levelUnlocked == levelAt)
            {
                tagDoneOnTheLevel++;
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (levelAt == "Neighborhood")
            {
                Debug.Log("Go to appartment from Neighborhood");
                AlbumManager.Instance.AddTagsFromAlbum();
                SceneManager.LoadScene("Apartment");
            }
            else if (levelAt == "TrainStation" && (tagDoneOnTheLevel == 0 || tagDoneOnTheLevel == maxTags))
            {
                Debug.Log("Go to appartment from Train Station");
                SceneManager.LoadScene("Apartment");
            }
            else
            {
                Debug.Log("Can't Go to appartemnt");
            }
        }
    }


    // Show or hide the interaction UI depending on player proximity
    public void ShowUI(bool show)
    {
        int tagDoneOnTheLevel = 0;
        foreach (var tag in album.tags)
        {
            if (tag.levelUnlocked == levelAt)
            {
                tagDoneOnTheLevel++;
            }
        }

        if (show)
        {
            if (currentUI == null && !(levelAt == "TrainStation" && tagDoneOnTheLevel > 0 && tagDoneOnTheLevel < maxTags))
            {
                currentUI = Instantiate(visualFeedBackInteraction);
            }
        }
        else if (!show && currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
    }
}
