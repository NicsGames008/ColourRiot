using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 3f;
    private bool isClimbing = false;
    private Rigidbody rb;
    private PlayerMovement playerMovement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("Entered ladder trigger");
            isClimbing = true;

          

            Vector3 snapPos = transform.position;
            snapPos.x = other.transform.position.x;
            snapPos.z = other.transform.position.z;
            transform.position = snapPos;



         
            Vector3 lookDir = other.transform.position - transform.position;
            lookDir.y = 0f;
            if (lookDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(lookDir);

            playerMovement.enabled = false;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
        }
        else if (other.CompareTag("LadderTop") && isClimbing)
        {
            Debug.Log("Top of ladder reached!");

            isClimbing = false;
            playerMovement.enabled = true;
            rb.useGravity = true;

           


            Vector3 exitPos = transform.position + transform.forward * 0.5f;
            exitPos.y += 0.2f; 
            transform.position = exitPos;

            rb.velocity = Vector3.zero;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("Exited ladder trigger");
            isClimbing = false;

           


            playerMovement.enabled = true;
            rb.useGravity = true;
        }
    }

    void Update()
    {
        if (isClimbing)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 climbVelocity = new Vector3(0f, vertical * climbSpeed, 0f);
            rb.velocity = climbVelocity;

          


            if (vertical == 0)
            {
                rb.velocity = Vector3.zero;
            }
        }
    }
}
