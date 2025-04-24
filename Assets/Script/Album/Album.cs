using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Album : MonoBehaviour
{

    public static Album Instance { get; private set; }

    public List<Tag> tags = new List<Tag>();

    private void Awake()
    {
        // Ensure that there is only one instance of Inventory
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
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
