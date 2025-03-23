using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagInteraction : MonoBehaviour, IInteractable
{

    public static TagInteraction Instance;
    [HideInInspector] public bool isInRange = false;
    [SerializeField] private GameObject visualFeedBackInteraction;

    private GameObject lockedPlayerPosistion;   //Is it better for this to be like this, on the awake it give the vhild, or a SerializeField and putted on the editor?
    private GameObject currentUI;
    private GameObject player;


    private void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
        lockedPlayerPosistion = this.transform.GetChild(0).gameObject;
    }


    public void Interact(GameObject Instigator)
    {
        Debug.Log("Interacated with a wall");

        //Locks player position
        player.transform.position = lockedPlayerPosistion.transform.position;

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Should this be and event instead of making the Component desabled?
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Makes the player enable to move
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();      
        PlayerCam playerCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<PlayerCam>();
        playerMovement.enabled = !playerMovement.enabled;
        playerCam.enabled = !playerCam.enabled;
    }


    private void Update()
    {
        //Shows a image in case the player in in range of a Intaractable wall
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
