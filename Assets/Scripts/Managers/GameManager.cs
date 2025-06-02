using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int MissionProgress { get; set; } = 0;

    public bool HasSeenSecondMissionIntro = false;


    private GameObject player;

    private void Awake()
    {
        // Ensure there's only one GameManager instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }

        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Update the reference to the player after every scene load
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public GameObject ReturnPlayer()
    {
        Debug.Log(player);
        return player;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            MissionProgress = 1;
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            MissionProgress = 0;
        }
    }
}