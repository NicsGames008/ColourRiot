using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class NPCInteract : MonoBehaviour
{
    [Header("Dialogue Sets")]
    [TextArea] public string[] firstMissionDialogue;
    [TextArea] public string[] incompleteNeighborhoodDialogue;
    [TextArea] public string[] secondMissionIntroDialogue;
    [TextArea] public string[] postSecondMissionDialogue;
    [TextArea] public string dialogueChoice = "Start the mission? (Y/N)";

    [Header("Mission")]
    public string defaultScene = "Neighborhood";
    public string secondScene = "TrainStation";
    public float sceneLoadDelay = 2f;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public float typewriterSpeed = 0.03f;

    [Header("Prompt UI")]
    public CanvasGroup interactionPromptGroup;
    public float fadeSpeed = 4f;

    [Header("Loading Screen")]
    [SerializeField] private SceneLoadManager sceneLoadManager;

    [Header("NPC Info")]
    public string npcName = "";
    [TextArea] public string dialogueLine = "sup boy?";

    private bool isPlayerNear = false;
    private bool transitioning = false;
    private bool waitingForChoice = false;
    private bool showingDialogue = false;
    private int currentLine = 0;
    private string[] currentDialogueSet;
    private Coroutine typewriterCoroutine;

    private GameManager gameManager;
    private PlayerState playerState;

    private bool loadTrainStation = false;

    private void Start()
    {
        gameManager = GameManager.Instance;
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E) && !transitioning)
        {
            StartDialogue();
        }

        if (showingDialogue && Input.GetKeyDown(KeyCode.Space))
        {
            if (typewriterCoroutine != null)
            {
                StopCoroutine(typewriterCoroutine);
                dialogueText.text = currentDialogueSet[currentLine];
                typewriterCoroutine = null;
                return;
            }

            currentLine++;
            if (currentLine < currentDialogueSet.Length)
            {
                typewriterCoroutine = StartCoroutine(TypeText(currentDialogueSet[currentLine]));
            }
            else
            {
                showingDialogue = false;
                waitingForChoice = true;
                typewriterCoroutine = StartCoroutine(TypeText(dialogueChoice));
            }
        }

        if (waitingForChoice)
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                waitingForChoice = false;

                if (npcName != "Said")
                {
                    StartCoroutine(LoadSceneAfterDelay());
                }
                else
                {
                    CloseDialogue(); // Said doesn't load scenes
                }
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
        Debug.Log("Current Mission Progress: " + gameManager.MissionProgress);

        transitioning = true;
        currentLine = 0;
        showingDialogue = true;
        SelectDialogueSet();

        playerState.ChangePlayerState(EPlayerState.Dialogue);

        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
            typewriterCoroutine = StartCoroutine(TypeText(currentDialogueSet[currentLine]));
        }
    }

    void SelectDialogueSet()
    {
        List<Tag> tags = gameManager.ReturnPlayer().GetComponent<Album>().tags;
        int tagCount = tags.Count;

        loadTrainStation = false;

        if (npcName == "Said")
        {
            currentDialogueSet = firstMissionDialogue;
            return;
        }

        if (tagCount == 0)
        {
            currentDialogueSet = firstMissionDialogue;
        }
        else if (tagCount >= 1 && tagCount <= 9)
        {
            currentDialogueSet = incompleteNeighborhoodDialogue;
        }
        else if (tagCount == 10)
        {
            currentDialogueSet = secondMissionIntroDialogue;
            gameManager.HasSeenSecondMissionIntro = true;
            loadTrainStation = true;
        }
        else if (tagCount >= 14)
        {
            currentDialogueSet = postSecondMissionDialogue;
        }
        else
        {
            currentDialogueSet = firstMissionDialogue;
        }
    }

    IEnumerator LoadSceneAfterDelay()
    {
        dialogueText.text = "Good Luck";
        yield return new WaitForSeconds(sceneLoadDelay);

        string targetScene = "Apartment";

        if (!loadTrainStation)
        {
            targetScene = defaultScene;
        }
        else
        {
            targetScene = secondScene;
        }

        dialoguePanel.SetActive(false);
        playerState.ChangePlayerState(EPlayerState.Moving);
        StartCoroutine(sceneLoadManager.LoadSceneAsynchronously(targetScene, null));
    }

    void CloseDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        playerState.ChangePlayerState(EPlayerState.Moving);
        transitioning = false;
        showingDialogue = false;
        waitingForChoice = false;
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        typewriterCoroutine = null;
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
}
