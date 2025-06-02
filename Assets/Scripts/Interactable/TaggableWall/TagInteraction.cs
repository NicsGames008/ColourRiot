using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TagInteraction : MonoBehaviour, IInteractable
{
    #region Variables

    // Public property to check if tag interaction has been completed
    public bool HasDoneThisTag => hasDoneThisTag;

    [Header("UI & Visual Feedback")]
    [SerializeField] private GameObject visualFeedBackInteraction; // UI feedback object for interaction
    [SerializeField] private GameObject sprayCan; // Reference to the visual spray can
    [SerializeField] private GameObject timerUI; // UI element that represents progress during interaction
    [SerializeField] private Slider timer; // Slider to show tagging progress

    [Header("Tag Data")]
    [SerializeField] private int tagTime = 2; // Time required to finish tagging
    [SerializeField] private float movingToWalTime = 1.5f; // Time it takes to move player to wall
    [SerializeField] private Tag gratffitiTag; // Reference to the tag (art/identifier)

    [Header("FX")]
    [SerializeField] private ParticleSystem sprayParticle; // Particle effect for spray can

    // Private references and state variables
    private GameObject lockedPlayerPosistion; // Target position to lock player during tag
    private GameObject currentUI; // Instance of the feedback UI
    private GameObject player; // Reference to the player object
    private GameObject cam; // Reference to the main camera
    private GameObject noiseCollider; // Reference to gameobject that has the noiseCollider

    private bool isAtWall = false; // Is the player near the wall and ready to tag
    private bool hasDoneThisTag = false; // Has the tag already been completed

    private float elapsedTime; // Internal timer during interaction
    private Image tagImage; // Image component that visually fills as tagging progresses
    private Animator spraycanAnimator; // Animator for the spray can

    private PlayerState playerState;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        // Locate player and camera in the scene
        player = GameObject.FindWithTag("Player");
        cam = GameObject.FindWithTag("MainCamera");
        playerState = player.GetComponent<PlayerState>();

        Transform cTransform = transform.Find("NoiseDetection");

        if (cTransform != null)
        {
            noiseCollider = cTransform.gameObject;
        }

        // Get references to necessary components
        spraycanAnimator = sprayCan.GetComponent<Animator>();

        // Get the lock-in position (first child of this object)
        lockedPlayerPosistion = this.transform.GetChild(0).gameObject;

        // Get image component (child of the second child)
        GameObject secondChild = this.transform.GetChild(1).gameObject;
        GameObject childOfSecondChild = secondChild.transform.GetChild(1).gameObject;
        tagImage = childOfSecondChild.GetComponent<Image>();

        // Set slider max value to required tag time
        timer.maxValue = tagTime;
        tagImage.overrideSprite = gratffitiTag.image;
    }

    private void Update()
    {
        // Stop tagging if player presses 'E'
        if (Input.GetKeyDown(KeyCode.E) && isAtWall && playerState.GetPlayerstate() == EPlayerState.InWall)
        {
            StopInteracting();
        }

        // Continue tagging if left mouse button is held
        if (Input.GetMouseButton(0) && isAtWall && !hasDoneThisTag && playerState.GetPlayerstate() == EPlayerState.InWall)
        {
            Countdown();
        }

        // Start tagging when mouse is pressed
        if (Input.GetMouseButtonDown(0) && isAtWall && !hasDoneThisTag && playerState.GetPlayerstate() == EPlayerState.InWall)
        {
            timerUI.SetActive(true);
            noiseCollider.SetActive(true);
            spraycanAnimator.SetBool("isSpraying", true);
            sprayParticle.Play();
        }

        // Cancel tagging when mouse button is released
        if (Input.GetMouseButtonUp(0) && isAtWall && !hasDoneThisTag && playerState.GetPlayerstate() == EPlayerState.InWall)
        {
            //elapsedTime = 0;
            //tagImage.fillAmount = 0;
            timerUI.SetActive(false);
            noiseCollider.SetActive(false);
            spraycanAnimator.SetBool("isSpraying", false);
            sprayParticle.Stop();
        }
    }

    #endregion

    #region Interaction

    // Show or hide the interaction UI depending on player proximity
    public void ShowUI(bool show)
    {
        if (show && !hasDoneThisTag)
        {
            if (currentUI == null)
            {
                currentUI = Instantiate(visualFeedBackInteraction);
            }
        }
        else if (!show && currentUI != null)
        {
            Destroy(currentUI);
            currentUI = null;
        }
    }

    // Called when the player interacts with the tagging object
    public void Interact(GameObject Instigator)
    {
        // Exit if already tagged
        if (hasDoneThisTag)
        {
            return;
        }

        Debug.Log("Interacted with a wall");

        // Move and rotate player to the tagging spot
        StartCoroutine(SmoothMovePlayer());
        StartCoroutine(SmoothRotatePlayer());

        // Activate spray can visuals
        sprayCan.SetActive(true);
        sprayParticle.Stop();

        // Disable movement and camera while tagging
        playerState.ChangePlayerState(EPlayerState.InWall);
    }

    // Cancel tagging and return control to player
    private void StopInteracting()
    {
        Debug.Log("Stopped Interacting with a wall");

        playerState.ChangePlayerState(EPlayerState.Moving);
        isAtWall = false;

        sprayCan.SetActive(false);
    }

    // Handles the progress of the tagging
    private void Countdown()
    {
        elapsedTime += Time.deltaTime;

        // Update UI
        timer.value = elapsedTime;
        tagImage.fillAmount = elapsedTime / tagTime;

        // Finish tag
        if (elapsedTime >= tagTime && !hasDoneThisTag)
        {
            sprayParticle.Stop();
            timerUI.SetActive(false);
            Album.Instance.Add(gratffitiTag);
            hasDoneThisTag = true;
            noiseCollider.SetActive(false);
            StopInteracting();
        }
    }


    #endregion

    #region Smooth Transitions

    // Smoothly rotate the camera to face the tagging spot
    public IEnumerator SmoothRotatePlayer()
    {
        if (cam.transform.rotation != lockedPlayerPosistion.transform.rotation)
        {  
            float time = Time.deltaTime;
            Quaternion startRotation = cam.transform.rotation;
            Quaternion endRotation = lockedPlayerPosistion.transform.rotation;

            while (time < movingToWalTime)
            {
                cam.transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / movingToWalTime);
                time += Time.deltaTime;
                yield return null;
            }

            cam.transform.rotation = endRotation;
            isAtWall = true;
        }
    }

    // Smoothly move the player to the tagging position
    public IEnumerator SmoothMovePlayer()
    {
        if (player.transform.position != lockedPlayerPosistion.transform.position)
        {
            float time = Time.deltaTime;
            Vector3 startPosition = player.transform.position;
            Vector3 endPosition = lockedPlayerPosistion.transform.position;

            while (time < movingToWalTime)
            {
                player.GetComponent<Rigidbody>().position = Vector3.Lerp(startPosition, endPosition, time / movingToWalTime);
                time += Time.deltaTime;
                yield return null;
            }

            player.transform.position = endPosition;
        }
    }

    #endregion
}
