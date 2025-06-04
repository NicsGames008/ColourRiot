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
            DontDestroyOnLoad(gameObject); // Persist across scenes
            AddStoredTagsToAlbum(); // Load saved tags into Album
        }
        else
        {
            Destroy(gameObject); // Prevent duplicates
        }
    }
    public List<Tag> Tags()
    {
        return storedTags;
    }

    public void AddStoredTagsToAlbum()
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }
        Album.Instance.tags.Clear();
        Album.Instance.tags.AddRange(storedTags);
    }
    public void AddTagsFromAlbum()
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }

        storedTags.Clear();
        storedTags.AddRange(Album.Instance.tags);
    }
    public List<Tag> GetStoredTags()
    {
        return storedTags;
    }
    public void AddCheatTags(List<Tag> cheatTags)
    {
        if (Album.Instance == null)
        {
            Debug.LogWarning("Album Instance not found!");
            return;
        }

        foreach (Tag tag in cheatTags)
        {
            if (!Album.Instance.tags.Contains(tag))
            {
                Album.Instance.Add(tag);
            }

            if (!storedTags.Contains(tag))
            {
                storedTags.Add(tag);
            }
        }
    }
}
