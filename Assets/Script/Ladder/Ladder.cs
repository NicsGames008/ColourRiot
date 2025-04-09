using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Input = UnityEngine.Windows.Input;


public class Ladder : MonoBehaviour
{
    public Transform ladder;
    bool ladderActive = false;
    public float speedUpDown;
    public PlayerMovement playerMovement;
    
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        ladderActive = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "ladder") ;
        {
            playerMovement.enabled = false;
            ladderActive = true;
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "ladder") ;
        {
            playerMovement.enabled = true;
            ladderActive = false;
        }
    }
    
    void Update()
    {
        if (ladderActive == true && KeyCode.W == KeyCode.UpArrow)
        {
            ladder.transform.position += Vector3.up * speedUpDown * Time.deltaTime;
        }

        if (ladderActive == true && KeyCode.S == KeyCode.DownArrow)
        {
            ladder.transform.position += Vector3.down * speedUpDown * Time.deltaTime;
        }
    }
}
