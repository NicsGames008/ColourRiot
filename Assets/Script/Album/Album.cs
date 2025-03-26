using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Album : MonoBehaviour
{

    public static Album Instance { get; private set; }

    public List<Tag> tags = new List<Tag>();


    // Define an event for inventory changes
    public event Action OnInventoryChanged;

    private void Awake()
    {
        // Ensure that there is only one instance of Inventory
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keep it alive across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    public void Add(Tag item)
    {
        tags.Add(item);
        OnInventoryChanged?.Invoke(); // Trigger the event
    }

    public void Remove(Tag item)
    {
        tags.Remove(item);
        OnInventoryChanged?.Invoke(); // Trigger the event
    }
}
