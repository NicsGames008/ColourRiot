using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerInteract : MonoBehaviour
{
    private Camera MainCamera;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        float distance = 1.5f;
        Vector3 RayOrigin = player.transform.position;
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);

        int interactableLayerMask = 1 << LayerMask.NameToLayer("Interactable");

        if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, interactableLayerMask))
        {
            IInteractable interactableObject = hitInfo.collider.GetComponent<IInteractable>();

            if (TagInteraction.Instance != null)
            {
                TagInteraction.Instance.SetInRange(true);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {   
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 1.0f);

                if (interactableObject != null) 
                {
                    interactableObject.Interact(gameObject);
                }
            }
        }
        else
        {
            if (TagInteraction.Instance != null)
            {
                TagInteraction.Instance.SetInRange(false);
            }
        }
    }
}
