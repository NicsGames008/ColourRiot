using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.Video;

public class NPCInteract : MonoBehaviour
{
    [Header("NPC Info")]
    public string npcName = "Vinz";
    [TextArea] public string dialogueLine = "Hey there! Wanna tag something?";
    [TextArea] public string dialogueChoice = "Start the mission? (Y/N)";

    [Header("Mission")]
    public string sceneToLoad;
    public float sceneLoadDelay = 2f;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float dialogueDuration = 3f;

    [Header("Prompt UI")]
    public CanvasGroup interactionPromptGroup;
    public float fadeSpeed = 4f;

    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private VideoClip neighborhoodLoadingScreen;
    [SerializeField] private VideoClip trainStationLoadingScreen;

    private bool isPlayerNear = false;
    private bool transitioning = false;
    private bool waitingForChoice = false;
    private Album album;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(DelayedSceneCheck());
    }

    public IEnumerator DelayedSceneCheck()
    {
        yield return null; // Wait 1 frame to let everything initialize

        sceneToLoad = "Neighborhood";
        album = GameObject.FindGameObjectWithTag("Player").GetComponent<Album>();

        int tagOnNeighborhood = 0;
        foreach (var tag in album.tags)
        {
            if (tag.levelUnlocked == "Neighborhood")
            {
                tagOnNeighborhood++;
            }
        }
        if (tagOnNeighborhood >= 10)
        {
            sceneToLoad = "TrainStation";
        }
    }


    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !transitioning)
        {
            StartDialogue();
        }

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                waitingForChoice = false;
                StartCoroutine(LoadSceneAfterDelay());
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                waitingForChoice = false;
                CloseDialogue();
            }
        }

        HandlePromptFade();
    }

    void StartDialogue()
    {
        transitioning = true;
        ShowDialogue(dialogueLine + "\n\n" + dialogueChoice);
        waitingForChoice = true;
    }

    void ShowDialogue(string text)
    {
        if (dialoguePanel != null && dialogueText != null)
        {
            dialoguePanel.SetActive(true);
            dialogueText.text = text;
        }
    }

    void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        transitioning = false;
    }

    IEnumerator LoadSceneAfterDelay()
    {
        ShowDialogue("Awesome. Let's go!");
        yield return new WaitForSeconds(sceneLoadDelay);
        //SceneManager.LoadScene(sceneToLoad);
        StartCoroutine(LoadSceneAsynchronously(sceneToLoad));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            CloseDialogue();
        }
    }

    void HandlePromptFade()
    {
        if (interactionPromptGroup == null) return;

        float targetAlpha = isPlayerNear && !transitioning ? 1f : 0f;
        interactionPromptGroup.alpha = Mathf.Lerp(interactionPromptGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        interactionPromptGroup.blocksRaycasts = targetAlpha > 0.1f;
    }

    IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        loadingScreen.SetActive(true);
        dialoguePanel.SetActive(false);

        VideoPlayer videoPlayer = loadingScreen.GetComponent<VideoPlayer>();

        switch (sceneName)
        {
            case "Neighborhood":
                videoPlayer.clip = neighborhoodLoadingScreen;
                break;
            case "TrainStation":
                videoPlayer.clip = trainStationLoadingScreen;
                break;
        }

        // Start tracking time
        float elapsedTime = 0f;
        float minimumWaitTime = 5f;

        // Start loading the scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent automatic scene activation

        while (!operation.isDone)
        {
            elapsedTime += Time.deltaTime;

            // Check if the loading has reached 90% (0.9) and waited at least 5 seconds
            if (operation.progress >= 0.9f && elapsedTime >= minimumWaitTime)
            {
                operation.allowSceneActivation = true; // Allow the scene to activate
            }

            yield return null;
        }
    }
}
