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
    [SerializeField] private int tagTime = 2; // Reference time that it take to make the Tag

    private GameObject lockedPlayerPosistion; // Position where player gets locked during interaction
    private GameObject currentUI; // Holds the current UI feedback instance
    private GameObject player; // Reference to the player
    private GameObject cam; // Reference to the main camera
    private bool isInRange = false; // Whether the player is in interaction range
    private bool isAtWall = false; // Whether the player is interacting with a wall
    private PlayerMovement playerMovement; // Reference to player's movement script
    private PlayerCam playerCam; // Reference to player's camera script
    private float elapsedTime; // Timer for interaction duration
    private Image tagImage; //Referece for the image

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
        
        // Get the child of the second child and saves it on the tagImage
        GameObject secondChild = this.transform.GetChild(1).gameObject;
        GameObject childOfSecondChild = secondChild.transform.GetChild(0).gameObject;
        tagImage = childOfSecondChild.GetComponent<Image>();

        // The slider to show the time set to the max time of the tag
        slider.maxValue = tagTime;
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
            tagImage.fillAmount = 0;
            sliderUI.SetActive(false);
        }
    }

    // Handles player interaction with an object
    public void Interact(GameObject Instigator)
    {
        Debug.Log("Interacted with a wall");

        // Lock player position to the predefined locked position
        //player.transform.position = lockedPlayerPosistion.transform.position;
        StartCoroutine(SmoothMovePlayer());
        StartCoroutine(SmoothRotatePlayer());

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

        // Make the tag appear
        tagImage.fillAmount = elapsedTime / tagTime;

        if (seconds == tagTime)
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

    public IEnumerator SmoothRotatePlayer()
    {
        // Check if the camera's current rotation is different from the target locked position rotation
        if (cam.transform.rotation != lockedPlayerPosistion.transform.rotation)
        {
            float time = 0f; // Timer to track the interpolation progress

            Quaternion startRotation = cam.transform.rotation; // Store the camera's initial rotation
            Quaternion endRotation = lockedPlayerPosistion.transform.rotation; // Store the target rotation

            // Gradually rotate the camera towards the target over 3 seconds
            while (time < 3)
            {
                cam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / 3);
                time += Time.deltaTime; // Increment the timer with delta time
                yield return null; // Wait for the next frame before continuing
            }

            cam.transform.rotation = endRotation; // Ensure the final rotation is exactly the target rotation
            isAtWall = true; // After the player end the animation of going to the place is able to make the tag
        }
    }
    
    public IEnumerator SmoothMovePlayer()
    {
        // Check if the players's current position is different from the target locked position position
        if (player.transform.position != lockedPlayerPosistion.transform.position)
        {
            float time = 0f; // Timer to track the interpolation progress

            Vector3 startPosition = player.transform.position; // Store the players's initial position
            Vector3 endPosition = lockedPlayerPosistion.transform.position; // Store the target position

            // Gradually rotate the camera towards the target over 3 seconds
            while (time < 3)
            {
                player.transform.position = Vector3.Lerp(startPosition, endPosition, time / 3);
                time += Time.deltaTime; // Increment the timer with delta time
                yield return null; // Wait for the next frame before continuing
            }

            player.transform.position = endPosition; // Ensure the final position is exactly the target position
        }
    }
}
