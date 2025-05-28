using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenuAnimatorFix : MonoBehaviour
{
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Animator[] buttonAnimators;

    private void OnEnable()
    {
        // Force refresh all layouts
        Canvas.ForceUpdateCanvases();

        // Reset all button animators
        foreach (Animator anim in buttonAnimators)
        {
            if (anim != null)
            {
                anim.Rebind();
                anim.Update(0f);
                anim.Play("Normal", -1, 0f);
            }
        }

        // Select a button to force highlight state
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
    }
}