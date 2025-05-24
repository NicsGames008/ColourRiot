using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class book : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;
    bool rotate = false;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject forwardButton;

    private void Start()
    {
        InitialState();
    }

    private void OnEnable()
    {
        ResetPageOrder();
        InitialState();
    }

    public void ResetPageOrder()
    {
        // Reverse the hierarchy order (so pages[0] is at the bottom, pages[10] at the top)
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetSiblingIndex(i); // First, reset to default order
        }
        // Now reverse them by setting each page's sibling index in reverse
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetSiblingIndex(pages.Count - 1 - i);
        }
        // Ensure the first page is active (since it's the "cover")
        pages[0].SetAsLastSibling();
    }

    public void InitialState()
    {
        // Reset rotations
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
        }

        // Make the first page visible (cover)
        pages[0].SetAsLastSibling();

        // Reset index and button states
        index = -1;
        backButton.SetActive(false);
        forwardButton.SetActive(true); // Enable forward button (unless on last page)
    }

    public void RotateForward()
    {
        if (rotate == true) { return; }
        index++;
        float angle = 180; //in order to rotate the page forward, you need to set the rotation by 180 degrees around the y axis
        ForwardButtonActions();
        pages[index].SetAsLastSibling();
        StartCoroutine(Rotate(angle, true));

    }

    public void ForwardButtonActions()
    {
        if (backButton.activeInHierarchy == false)
        {
            backButton.SetActive(true); //every time we turn the page forward, the back button should be activated
        }
        if (index == pages.Count - 1)
        {
            forwardButton.SetActive(false); //if the page is last then we turn off the forward button
        }
    }

    public void RotateBack()
    {
        if (rotate == true) { return; }
        float angle = 0; //in order to rotate the page back, you need to set the rotation to 0 degrees around the y axis
        pages[index].SetAsLastSibling();
        BackButtonActions();
        StartCoroutine(Rotate(angle, false));
    }

    public void BackButtonActions()
    {
        if (forwardButton.activeInHierarchy == false)
        {
            forwardButton.SetActive(true); //every time we turn the page back, the forward button should be activated
        }
        if (index - 1 == -1)
        {
            backButton.SetActive(false); //if the page is first then we turn off the back button
        }
    }

    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            rotate = true;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.unscaledDeltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value); //smoothly turn the page
            float angle1 = Quaternion.Angle(pages[index].rotation, targetRotation); //calculate the angle between the given angle of rotation and the current angle of rotation
            if (angle1 < 0.1f)
            {
                if (forward == false)
                {
                    index--;
                }
                rotate = false;
                break;

            }
            yield return null;

        }
    }



}
