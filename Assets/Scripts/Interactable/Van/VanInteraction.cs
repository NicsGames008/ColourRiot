using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VanInteraction : MonoBehaviour, IInteractable
{

    [SerializeField] private GameObject visualFeedBackInteraction; // UI feedback object for interaction
    [SerializeField] private SceneLoadManager sceneLoadManager;
    private GameObject currentUI; // Instance of the feedback UI
    private Album album;

    private int maxTags;
    private string levelAt;
    private bool isLoading = false;

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
                StartCoroutine(sceneLoadManager.LoadSceneAsynchronously("Apartment", "Neighborhood"));
                isLoading = true;
                ShowUI(false);
            }
            else if (levelAt == "TrainStation" && (tagDoneOnTheLevel == 0 || tagDoneOnTheLevel == maxTags))
            {
                Debug.Log("Go to appartment from Train Station");
                AlbumManager.Instance.AddTagsFromAlbum();
                StartCoroutine(sceneLoadManager.LoadSceneAsynchronously("Apartment", "TrainStation"));
                isLoading = true;
                ShowUI(false);
            }
            else
            {
                Debug.Log("Can't Go to appartemnt");
            }
        }
    }


    public void ShowUI(bool show)
    {
        // Don't show UI if game is paused
        if (PauseMenu.gameIsPause)
        {
            show = false;
        }

        if (!show)
        {
            if (currentUI != null)
            {
                Destroy(currentUI);
                currentUI = null;
            }
            return;
        }

        if (currentUI != null || isLoading)
            return;

        int tagsInLevel = album.tags.Count(tag => tag.levelUnlocked == levelAt);

        bool isTrainStationWithTags = levelAt == "TrainStation" &&
                                    tagsInLevel > 0 &&
                                    tagsInLevel < maxTags;

        if (!isTrainStationWithTags)
        {
            currentUI = Instantiate(visualFeedBackInteraction);
        }
    }
}
