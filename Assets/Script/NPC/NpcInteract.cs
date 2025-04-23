using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NPCInteract : MonoBehaviour
{


    [Header("NPC Info")]
    public string npcName = "Vinz";
    [TextArea] public string dialogueLine = "Hey there! Wanna tag something?";

    [Header("Mission")]
    public string sceneToLoad; 
    public float sceneLoadDelay = 2f;

    private bool isPlayerNear = false;
    private bool transitioning = false;

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }




    void Interact()
    {
        if (!transitioning)
        {
            Debug.Log($"[NPC: {npcName}] says: {dialogueLine}");
            transitioning = true;

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                Debug.Log("Preparing to load mission scene...");
                StartCoroutine(LoadSceneAfterDelay());
            }
        }
    }



    IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(sceneLoadDelay);
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            ShowPrompt(true);
        }
    }




    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            ShowPrompt(false);
        }
    }


    void ShowPrompt(bool show)
    {
        Debug.Log(show ? "[E] Talk" : "Out of range.");
    }
}
