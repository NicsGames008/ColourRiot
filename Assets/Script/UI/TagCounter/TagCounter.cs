using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TagCounter : MonoBehaviour
{
    [SerializeField] private int levelAt = 0;

    private TextMeshProUGUI text;
    private Album album;
    private int maxTags;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        album = GameObject.FindGameObjectWithTag("Player").GetComponent<Album>();
        maxTags = GameObject.FindGameObjectsWithTag("TaggableWall").Length;
    }

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (var tag in album.tags)
        {
            if (tag.levelUnlocked == levelAt)
            {
                i++;
            }
        }
        text.text = $"{i}/{maxTags}";
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
