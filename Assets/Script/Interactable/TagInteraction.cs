using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TagInteraction : MonoBehaviour, IInteractable
{
    // Singleton instance of the TagInteraction class
    public static TagInteraction Instance;

    // Event triggered when interaction range changes
    public static event Action<bool> OnRangeChanged;

    [SerializeField] private GameObject visualFeedBackInteraction; // UI feedback object for interaction
    [SerializeField] private GameObject sliderUI; // UI element that represents progress during interaction
    [SerializeField] private Slider slider; // Reference to the slider component
    [SerializeField] private int xOffset = 15; // X Offset for the camera when its locked on the wall 

    private GameObject lockedPlayerPosistion; // Position where player gets locked during interaction
    private GameObject currentUI; // Holds the current UI feedback instance
    private GameObject player; // Reference to the player
    private GameObject cam; // Reference to the main camera
    private bool isInRange = false; // Whether the player is in interaction range
    private bool isAtWall = false; // Whether the player is interacting with a wall
    private PlayerMovement playerMovement; // Reference to player's movement script
    private PlayerCam playerCam; // Reference to player's camera script
    private float elapsedTime; // Timer for interaction duration

    private void Awake()
    {
        // Set the singleton instance
        Instance = this;

        // Find player and camera by tag
        player = GameObject.FindWithTag("Player");
        cam = GameObject.FindWithTag("MainCamera");

        // Get player movement and camera components
        playerMovement = player.GetComponent<PlayerMovement>();
        playerCam = cam.GetComponent<PlayerCam>();

        // Get the first child of this object as the locked position for the player
        lockedPlayerPosistion = this.transform.GetChild(0).gameObject;
    }

    // Updates the player's interaction range status
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
        // If the player presses 'E' while interacting with a wall, stop interacting
        if (Input.GetKeyDown(KeyCode.E) && isAtWall)
        {
            StopInteracting();
        }

        // If left mouse button is held down while interacting, start countdown
        if (Input.GetMouseButton(0) && isAtWall)
        {
            Countdown();
        }

        // Show slider UI when left mouse button is first pressed
        if (Input.GetMouseButtonDown(0) && isAtWall)
        {
            sliderUI.SetActive(true);
        }

        // Hide slider UI and reset timer when mouse button is released
        if (Input.GetMouseButtonUp(0) && isAtWall)
        {
            elapsedTime = 0;
            sliderUI.SetActive(false);
        }
    }

    // Handles player interaction with an object
    public void Interact(GameObject Instigator)
    {
        Debug.Log("Interacted with a wall");

        // Lock player position to the predefined locked position
        player.transform.position = lockedPlayerPosistion.transform.position;
        StartCoroutine(SmoothLookAtMe());
        isAtWall = true;

        // Disable player movement and camera controls
        playerMovement.enabled = false;
        playerCam.enabled = false;
    }

    // Stops the interaction and restores player movement
    private void StopInteracting()
    {
        Debug.Log("Stopped Interacting with a wall");

        // Re-enable player movement and camera controls
        playerMovement.enabled = true;
        playerCam.enabled = true;
        isAtWall = false;
    }

    // Counts down interaction time, stopping interaction after 2 seconds
    private void Countdown()
    {
        elapsedTime += Time.deltaTime;
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        // Update the slider value to reflect the elapsed time
        slider.value = elapsedTime;

        if (seconds == 2)
        {
            sliderUI.SetActive(false);
            StopInteracting();
        }
    }

    private void OnEnable()
    {
        // Subscribe to the range change event
        OnRangeChanged += HandleRangeChange;
    }

    private void OnDisable()
    {
        // Unsubscribe from the range change event
        OnRangeChanged -= HandleRangeChange;
    }

    // Handles UI feedback when player enters or exits interaction range
    private void HandleRangeChange(bool inRange)
    {
        if (inRange && currentUI == null)
        {
            // Instantiate visual feedback UI if in range
            currentUI = Instantiate(visualFeedBackInteraction);
        }
        else if (!inRange && currentUI != null)
        {
            // Destroy UI feedback when out of range
            Destroy(currentUI);
            currentUI = null;
        }
    }

    public IEnumerator SmoothLookAtMe()
    {
        if (cam.transform.rotation != lockedPlayerPosistion.transform.rotation) 
        {
            float time = 0f;

            Quaternion startRotation = cam.transform.rotation;

            Quaternion endRotation = lockedPlayerPosistion.transform.rotation;
            endRotation.x += xOffset;

            Debug.Log(endRotation);

            while (time < 3)
            {
                cam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / 3);
                time += Time.deltaTime;
                yield return null;
            }

            cam.transform.rotation = endRotation;


        }
    }
}
