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

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        album = GameObject.FindGameObjectWithTag("Player").GetComponent<Album>();
        maxTags = GameObject.FindGameObjectsWithTag("TaggableWall").Length;
        levelAt = SceneManager.GetActiveScene().name;

    }

    // Start is called before the first frame update
    void Start()
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

        if (tagDoneOnTheLevel == maxTags)
        {
            text.color = Color.green;
            text.text = "You've done it!";
        }
    }
}
