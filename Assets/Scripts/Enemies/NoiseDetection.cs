using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDetection : MonoBehaviour
{
    // List of enemy GameObjects that have entered this noise detection trigger
    private List<GameObject> enemiesThatEntered = new List<GameObject>();

    // Called automatically by Unity when a collider enters this trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is tagged as an enemy
        if (other.CompareTag("Enemy"))
        {
            // Add the enemy to the list if it's not already tracked
            if (!enemiesThatEntered.Contains(other.gameObject))
            {
                enemiesThatEntered.Add(other.gameObject);
            }
        }
    }

    // Checks if a specific enemy has entered the noise zone, and clears the entry if it has
    public bool HasPoliceHeardTag(GameObject enemy)
    {
        // If the enemy was tracked, return true and remove it from the list
        if (enemiesThatEntered.Contains(enemy))
        {
            enemiesThatEntered.Remove(enemy);
            return true;
        }

        // Otherwise, the enemy hasn't heard the noise
        return false;
    }
}