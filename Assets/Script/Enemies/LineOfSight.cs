using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float maxRaycastDistance = 20f;
    private Transform playerTransform = null;

    private bool hasSeenPlayerThisFrame = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Find the first player as this should be the only one in the level anyways
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        // 45 degree angle from either side of the forward facing vector, we convert from Euler to Radians
        const float visionConeEulerAngles = 45.0f;
        float radVisionCone = visionConeEulerAngles * Mathf.Deg2Rad;

        // Get the object forward then the direction to the player
        Vector3 forwardVector = transform.forward;
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Use the dot product to see how close these two angles are aka in radians
        float dotProduct = Vector3.Dot(forwardVector, directionToPlayer);
        if (dotProduct < radVisionCone)
        {
            hasSeenPlayerThisFrame = false;
            return;
        }

        // If the player is technically in the vision cone then let's check through physics that he is physically visible
        // If he is we just go ahead and toggle a boolean on and save that information for later.
        Vector3 directionalMagnitudeToPlayer = (playerTransform.position - transform.position);
        float distanceToHeadset = directionalMagnitudeToPlayer.magnitude;

        // Cast ray only up to the lesser of the actual distance or maxRaycastDistance
        float raycastDistance = Mathf.Min(distanceToHeadset, maxRaycastDistance);
        Ray ray = new Ray(transform.position, directionalMagnitudeToPlayer.normalized);
        RaycastHit hitInfo;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.red, 10.0f);

        if (Physics.Raycast(ray, out hitInfo, raycastDistance))
        {
            if (hitInfo.collider.gameObject.CompareTag("Player"))
            {
                hasSeenPlayerThisFrame = true;
                return;
            }
        }

        hasSeenPlayerThisFrame = false;



    }

    // Accessor Function
    public bool HasSeenPlayerThisFrame()
    {
        return hasSeenPlayerThisFrame;
    }
}
