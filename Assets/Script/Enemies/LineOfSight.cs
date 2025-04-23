using UnityEngine;

// This class handles checking whether the player is within the enemy's field of view and line of sight
public class LineOfSight : MonoBehaviour
{
    // The maximum distance the raycast can travel to detect the player
    [SerializeField] private float maxRaycastDistance = 20f;

    // Reference to the player's Transform component
    private Transform playerTransform = null;

    // Boolean flag to track whether the player was seen during this frame
    private bool hasSeenPlayerThisFrame = false;

    // Called when the object is first initialized (before Start)
    void Awake()
    {
        // Find the player in the scene using the "Player" tag
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Transform>();
    }

    // Called once per frame
    void Update()
    {
        float visionConeDegrees = 90.0f;
        float dotThreshold = Mathf.Cos(visionConeDegrees * 0.5f * Mathf.Deg2Rad);

        // Get this object's forward vector and the direction to the player
        Vector3 forwardVector = transform.forward;
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Use dot product to check if the player is within the vision cone
        float dotProduct = Vector3.Dot(forwardVector, directionToPlayer);

        // If the angle to the player is outside the cone, player is not visible
        if (dotProduct < dotThreshold)
        {
            hasSeenPlayerThisFrame = false;
            return;
        }

        // If the player is inside the cone, perform a raycast to check line of sight
        Vector3 directionalMagnitudeToPlayer = (playerTransform.position - transform.position);
        float distanceToHeadset = directionalMagnitudeToPlayer.magnitude;

        // Only raycast up to the maximum allowed distance or the actual distance to the player
        float raycastDistance = Mathf.Min(distanceToHeadset, maxRaycastDistance);
        Ray ray = new Ray(transform.position, directionalMagnitudeToPlayer.normalized);
        RaycastHit hitInfo;

        // Visualize the raycast in the scene view for debugging
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastDistance, Color.red, 3.0f);

        // Perform the raycast and check if it hits the player
        if (Physics.Raycast(ray, out hitInfo, raycastDistance))
        {
            if (hitInfo.collider.gameObject.CompareTag("Player"))
            {
                // Player is seen and not obstructed
                hasSeenPlayerThisFrame = true;
                return;
            }
        }

        // Player was not seen this frame
        hasSeenPlayerThisFrame = false;
    }

    // Public method to check if the player was seen during this frame
    public bool HasSeenPlayerThisFrame()
    {
        return hasSeenPlayerThisFrame;
    }
}

