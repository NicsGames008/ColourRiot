using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TagInteraction : MonoBehaviour, IInteractable
{

    public static TagInteraction Instance;
    public static event Action<bool> OnRangeChanged;

    [SerializeField] private GameObject visualFeedBackInteraction;
    [SerializeField] private float yOffSetLookAtWall = -10f;

    private GameObject lockedPlayerPosistion;   //Is it better for this to be like this, on the awake it give the vhild, or a SerializeField and putted on the editor?
    private GameObject currentUI;
    private GameObject player;
    private GameObject cam;
    private bool isInRange = false;
    private bool isAtWall = false;
    private PlayerMovement playerMovement;
    private PlayerCam playerCam;


    private void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
        cam = GameObject.FindWithTag("MainCamera");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCam = cam.GetComponent<PlayerCam>();
        lockedPlayerPosistion = this.transform.GetChild(0).gameObject;
    }

    public void SetInRange(bool inRange)
    {
        if (isInRange != inRange)
        {
            isInRange = inRange;
            OnRangeChanged?.Invoke(isInRange);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isAtWall)
        {
            StopInteracting();
            isAtWall = false;
        }
    }

    public void Interact(GameObject Instigator)
    {
        Debug.Log("Interacated with a wall");

        //Locks player position
        player.transform.position = lockedPlayerPosistion.transform.position;

        isAtWall = true;

        //Makes the player enable to move
        playerMovement.enabled = !playerMovement.enabled;

        //StartCoroutine(SmoothLookAtMe(cam.transform, 1f, yOffSetLookAtWall));
        playerCam.enabled = !playerCam.enabled;
    }

    private void StopInteracting()
    {
        Debug.Log("Stoped Interacting with a wall");

        //Makes the player enable to move
        playerMovement.enabled = true;
        playerCam.enabled = true;
    }

    public IEnumerator SmoothLookAtMe(Transform objectToRotate, float duration, float yOffset = 0f)
    {
        // Calculate the direction from the locked position to the wall
        Vector3 direction = transform.position - lockedPlayerPosistion.transform.position;

        if (direction != Vector3.zero) // Avoid division errors
        {
            Quaternion startRotation = objectToRotate.rotation;

            // Target rotation will look towards the direction calculated from the locked position
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Modify the Y rotation (add the offset)
            targetRotation = Quaternion.Euler(targetRotation.eulerAngles.x, targetRotation.eulerAngles.y + yOffset, targetRotation.eulerAngles.z);

            float time = 0f;

            // Smoothly rotate towards the target rotation
            while (time < duration)
            {
                objectToRotate.rotation = Quaternion.Slerp(startRotation, targetRotation, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            // Ensure final rotation is applied
            objectToRotate.rotation = targetRotation;
        }
    }





    private void OnEnable()
    {
        OnRangeChanged += HandleRangeChange;
    }

    private void OnDisable()
    {
        OnRangeChanged -= HandleRangeChange;
    }

    private void HandleRangeChange(bool inRange)
    {
        if (inRange && currentUI == null)
        {
            currentUI = Instantiate(visualFeedBackInteraction);
        }
        else if (!inRange && currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
    }
}
