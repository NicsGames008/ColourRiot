using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Album : MonoBehaviour
{

    public static Album Instance { get; private set; }

    public List<Tag> tags = new List<Tag>();

    private IEnumerator Start()
    {
        if(Instance == null)
            Instance = this;
        else
            Destroy(gameObject); // Destroy duplicate instances

        while (AlbumManager.Instance == null)
        {
            yield return null; // wait until next frame
        }

        AlbumManager.Instance.AddStoredTagsToAlbum();
    }

    public void Add(Tag item)
    {
        tags.Add(item);
    }

    public void Remove(Tag item)
    {
        tags.Remove(item);
    }
}
