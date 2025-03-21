using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagInteraction : MonoBehaviour, IInteractable
{

    public static TagInteraction Instance;
    [HideInInspector] public bool isInRange = false;
    [SerializeField] private GameObject visualFeedBackInteraction;
    private GameObject currentUI; 


    private void Awake()
    {
        Instance = this;
    }


    public void Interact(GameObject Instigator)
    {
        Debug.Log("Interacated with a wall");
    }


    private void Update()
    {
        if (isInRange && currentUI == null) 
        {
            currentUI = Instantiate(visualFeedBackInteraction);
        }
        else if (!isInRange && currentUI != null) 
        {
            Destroy(currentUI);
            currentUI = null;
        }
    }
}
