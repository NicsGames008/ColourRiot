using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagInteraction : MonoBehaviour, IInteractable
{
    public void Interact(GameObject Instigator)
    {
        Debug.Log("Interacated with a wall");
    }
}
