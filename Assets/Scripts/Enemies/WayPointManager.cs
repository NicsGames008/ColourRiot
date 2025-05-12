using System.Collections.Generic;
using UnityEngine;

public class WayPointManager : MonoBehaviour
{
    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    // Get an element from the list.
    public Transform GetwaypointsAtIndex(int index)
    {
        if (!IsIndexValid(index))
            return null;

        return waypoints[index];
    }

    // Check if the index is valid in the list
    public bool IsIndexValid(int index)
    {
        return index < waypoints.Count && index >= 0;
    }
}
