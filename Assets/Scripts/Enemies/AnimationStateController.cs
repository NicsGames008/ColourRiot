using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Public method to set the isRunning bool
    public void SetRunning(bool isRunning)
    {
        if (animator != null)
        {
            animator.SetBool("IsRunning", isRunning);
        }
    }

    public void SetBool(string paramName, bool value)
    {
        if (animator != null)
        {
            animator.SetBool(paramName, value);
        }
    }
}
