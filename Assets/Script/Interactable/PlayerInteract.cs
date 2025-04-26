using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerInteract : MonoBehaviour
{
    private Camera MainCamera; // Reference to the main camera
    private GameObject player; // Reference to the player object
    private IInteractable lastInteractable = null;

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

        float distance = 3f; // Maximum interaction distance
        Vector3 RayOrigin = player.transform.position; // Set ray origin to the player's position
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition); // Create a ray from the mouse positione

        int interactableLayerMask = 1 << LayerMask.NameToLayer("Interactable"); // Define the interactable layer mask

        // Cast a ray from the camera towards the mouse position to check for interactable objects
        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, interactableLayerMask))
        {
            IInteractable interactableObject = hitInfo.collider.GetComponent<IInteractable>(); // Get the interactable component

            if (interactableObject != null)
            {
                if (interactableObject is TagInteraction currentWall)
                {
                    if (!currentWall.HasDoneThisTag)
                    {
                        currentWall.ShowUI(true);
                        lastInteractable = interactableObject;
                    }

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentWall.Interact(gameObject);
                    }
                }
                else if (interactableObject is VanInteraction currentVan)
                {
                    currentVan.ShowUI(true);
                    lastInteractable = interactableObject;

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        currentVan.Interact(gameObject);
                    }
                }
            }
        }
        else
        {
            // Hide UI from the last wall we looked at
            if (lastInteractable != null && lastInteractable is TagInteraction previousWall)
            {
                previousWall.ShowUI(false);
                lastInteractable = null;
            }
            else if (lastInteractable != null && lastInteractable is VanInteraction previousVan)
            {
                previousVan.ShowUI(false);
                lastInteractable = null;
            }
        }
    }
}
