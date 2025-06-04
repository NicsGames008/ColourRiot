using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 3f;

    [HideInInspector] public float ladderBottomY;
    [HideInInspector] public float ladderTopY;

    private void Start()
    {
        float halfHeight = transform.localScale.y * 0.5f;
        ladderBottomY = transform.position.y - halfHeight;
        ladderTopY = transform.position.y + halfHeight;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerState playerState = other.GetComponent<PlayerState>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (playerState != null && rb != null)
            {
                playerState.ChangePlayerState(EPlayerState.Climbing);
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerState playerState = other.GetComponent<PlayerState>();
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (playerState != null && rb != null)
            {
                Vector3 playerPos = other.transform.position;
               
                if (IsAtTop(playerPos))
                {
                    other.transform.position += Vector3.up * 0.5f + other.transform.forward * 0.3f;
                    rb.AddForce((Vector3.up + other.transform.forward) * 2f, ForceMode.Impulse);
                }
                else if (IsAtBottom(playerPos))
                {
                    rb.AddForce(Vector3.down + other.transform.forward * 0.5f, ForceMode.Impulse);
                }
                playerState.ChangePlayerState(EPlayerState.Moving);
                rb.useGravity = true;
            }
        }
    }
    public bool IsAtBottom(Vector3 playerPosition)
    {
        return playerPosition.y <= ladderBottomY + 0.3f;
    }
    public bool IsAtTop(Vector3 playerPosition)
    {
        return playerPosition.y >= ladderTopY - 0.3f;
    }
}
