using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class TimerManager : MonoBehaviour
{
    private EPlayerState currentPlayerState;
    private PlayerState playerState;
    public TextMeshProUGUI timerText;
    public DayNightCycle dayNightCycle;
    public GameObject deathScreen;

    public float startingTime = 5f;
    private float timeRemaining;
    private bool timerRunning = false;

    [SerializeField] private AudioClip timestarted;
    private AudioManager audioManager;


    void Start()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (deathScreen != null)
            deathScreen.SetActive(false);

        timeRemaining = startingTime;
        playerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerState>();

        audioManager = FindObjectOfType<AudioManager>();

    }

    void Update()
    {
        if (timerRunning)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0f)
            {
                timeRemaining = 0f;
                timerRunning = false;
                UpdateTimerDisplay(); // Make sure it shows 00:00
                LoseGame(); // Trigger lose behavior
                return; // Prevent further execution
            }

            UpdateTimerDisplay(); // Only call if time > 0
        }
    }

    public void StartTimer()
    {
        if (!timerRunning)
        {
            timerRunning = true;

            if (timerText != null)
                timerText.gameObject.SetActive(true);

            if (dayNightCycle != null)
                dayNightCycle.StartCycle();
        }

        if (audioManager != null)
        {
            audioManager.ChangeMusic(timestarted);
        }

    }

    private void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void LoseGame()
    {
        Debug.Log("Player lost! Showing Death Screen.");

        if (deathScreen != null)
        {
            deathScreen.SetActive(true);

            if (playerState != null)
            {
                playerState.ChangePlayerState(EPlayerState.Dead);
            }

            Time.timeScale = 0f; 
            StartCoroutine(DelayedSceneLoad()); 
        }
    }

    private IEnumerator DelayedSceneLoad()
    {
        
        Time.timeScale = 1f;

        yield return new WaitForSeconds(3f); 

        SceneManager.LoadScene("Apartment");
    }

}
