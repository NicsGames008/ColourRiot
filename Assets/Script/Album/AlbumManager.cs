using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager Instance { get; private set; }

    [SerializeField ]private List<Tag> storedTags = new List<Tag>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make it persist between scenes
            AddStoredTagsToAlbum(); // Push stored tags into the Album
        }
        else
        {
            Destroy(gameObject); // Destroy duplicates
        }
    }

    // Push stored tags into the Album if they are not already there
    public void AddStoredTagsToAlbum()
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }

        foreach (Tag tag in storedTags)
        {
            if (!Album.Instance.tags.Contains(tag))
            {
                Album.Instance.tags.Add(tag);
            }
        }
    }

    // Add any missing tags from Album into AlbumManager's storedTags
    public void AddTagsFromAlbum()
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }

        foreach (Tag tag in Album.Instance.tags)
        {
            if (!storedTags.Contains(tag))
            {
                storedTags.Add(tag);
            }
        }
    }


    // Get the stored list
    public List<Tag> GetStoredTags()
    {
        return storedTags;
    }
}
