using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlbumManager : MonoBehaviour
{
    public static AlbumManager Instance { get; private set; }

    [SerializeField] private List<Tag> storedTags = new List<Tag>();

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

    public void AddStoredTagsToAlbum()
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }

        Album.Instance.tags.Clear(); // First, clear the Album's tag list
        Album.Instance.tags.AddRange(storedTags); // Then, add all storedTags
    }


    public void AddTagsFromAlbum()
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }

        storedTags.Clear(); // First, clear the stored tags
        storedTags.AddRange(Album.Instance.tags); // Then, add all Album tags
    }



    // Get the stored list
    public List<Tag> GetStoredTags()
    {
        return storedTags;
    }
}
