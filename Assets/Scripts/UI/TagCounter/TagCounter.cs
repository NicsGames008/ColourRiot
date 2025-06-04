using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TagCounter : MonoBehaviour
{
    private string levelAt;
    private TextMeshProUGUI text;
    private Album album;
    private int maxTags;
    private bool timerStarted = false; 
    private TimerManager timerManager;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        album = GameObject.FindGameObjectWithTag("Player").GetComponent<Album>();
        maxTags = GameObject.FindGameObjectsWithTag("TaggableWall").Length;
        levelAt = SceneManager.GetActiveScene().name;
        timerManager = FindObjectOfType<TimerManager>();
    }

    // Start is called before the first frame update
    void Start()
    {

        if (SceneManager.GetActiveScene().name == "Apartment")
        {
            gameObject.SetActive(false);
        }

        int tagDoneOnTheLevel = 0;
        foreach (var tag in album.tags)
        {
            if (tag.levelUnlocked == levelAt)
            {
                tagDoneOnTheLevel++;
            }
        }
        text.text = $"{tagDoneOnTheLevel}/{maxTags}";
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
        text.text = $"{tagDoneOnTheLevel}/{maxTags}";

        if (tagDoneOnTheLevel > 0 && !timerStarted)
        {
            timerStarted = true;
            if (timerManager != null)
            {
                timerManager.StartTimer();
            }
        }

        if (tagDoneOnTheLevel == maxTags)
        {
            text.color = Color.green;
            text.text = "You've done it!";
        }
    }
}
