using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerInteract : MonoBehaviour
{
    private Camera MainCamera; // Reference to the main camera
    private GameObject player; // Reference to the player object

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main; // Assign the main camera
        player = GameObject.FindWithTag("Player"); // Find and assign the player object by tag
    }

    // Update is called once per frame
    void Update()
    {
        // If the player reference is null, exit the function
        if (player == null)
            return;

        float distance = 1.5f; // Maximum interaction distance
        Vector3 RayOrigin = player.transform.position; // Set ray origin to the player's position
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse position

        int interactableLayerMask = 1 << LayerMask.NameToLayer("Interactable"); // Define the interactable layer mask

        // Cast a ray from the camera towards the mouse position to check for interactable objects
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, interactableLayerMask))
        {
            IInteractable interactableObject = hitInfo.collider.GetComponent<IInteractable>(); // Get the interactable component

            // Notify the TagInteraction system that the player is in range
            if (TagInteraction.Instance != null)
            {
                TagInteraction.Instance.SetInRange(true);
            }

            // Check if the player presses the interaction key (E)
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 1.0f); // Draw debug ray in the scene

                // If an interactable object was hit, call its Interact function
                if (interactableObject != null)
                {
                    interactableObject.Interact(gameObject);
                }
            }
        }
        else
        {
            // If no interactable object is detected, notify the TagInteraction system
            if (TagInteraction.Instance != null)
            {
                TagInteraction.Instance.SetInRange(false);
            }
        }
    }
}
