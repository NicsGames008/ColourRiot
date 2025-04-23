using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    
    public float sensX;
    public float sensY;
    
    public Transform orientation;

    private float xRotation;
    private float yRotation;

    private PlayerState playerState;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerState = GameObject.FindWithTag("Player").GetComponent<PlayerState>();
    }
    
    private void Update()
    {
        if (playerState.GetPlayerstate() == EPlayerState.Moving)
        {
            // get mouse input 
            float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;

            yRotation += mouseX;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -50f, 50f);

            // rotate cam and orientation
            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0); 
        }
    }
}