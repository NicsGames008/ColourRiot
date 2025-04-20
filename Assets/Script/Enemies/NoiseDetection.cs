using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseDetection : MonoBehaviour
{
    private List<GameObject> enemiesThatEntered = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesThatEntered.Contains(other.gameObject))
            {
                enemiesThatEntered.Add(other.gameObject);
            }
        }
    }


    public bool HasPoliceHeardTag(GameObject enemy)
    {
        if (enemiesThatEntered.Contains(enemy))
        {
            enemiesThatEntered.Remove(enemy);
            return true;
        }
        return false;
    }


}
