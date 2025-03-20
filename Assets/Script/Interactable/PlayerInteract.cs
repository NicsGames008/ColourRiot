using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        float distance = 2f;
        Vector3 RayOrigin = player.transform.position;
        Ray ray  = MainCamera.ScreenPointToRay(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.E))
        {
            int interactableLayerMask = 1 << LayerMask.NameToLayer("Interactable");


            if (Physics.Raycast(ray, out RaycastHit hitInfo, distance, interactableLayerMask))
            {
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, 1.0f);

                IInteractable interactableObject = hitInfo.collider.GetComponent<IInteractable>();
                Debug.Log("a");

                if (interactableObject != null)
                {
                Debug.Log("b");
                    interactableObject.Interact(gameObject);
                }
            }

            //bool HitInformation = Physics.Raycast(RayOrigin, Direction, Distance, interactableLayerMask);
            //Debug.DrawRay(RayOrigin, Direction, Color.green, 1.0f);

            //if (HitInformation)
            //{
            //    IInteractable interactableObject = HitInformation.GetComponent<IInteractable>();
            //    if (interactableObject != null)
            //    {
            //        interactableObject.Interact(gameObject);
            //    }
            //}
        }
    }
}
