using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private VideoClip neighborhoodLoadingScreen;
    [SerializeField] private VideoClip trainStationLoadingScreen;
    [SerializeField] private VideoClip apartamentLoadingScreen;

    // Add this to track loading state
    public static bool IsLoading { get; private set; }

    public IEnumerator LoadSceneAsynchronously(string sceneName)
    {
        IsLoading = true;
        loadingScreen.SetActive(true);

        // Get the parent UI object and disable all children except loading screen
        Transform parentUI = loadingScreen.transform.parent;
        if (parentUI != null)
        {
            foreach (Transform child in parentUI)
            {
                if (child.gameObject != loadingScreen)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        VideoPlayer videoPlayer = loadingScreen.GetComponent<VideoPlayer>();

        switch (sceneName)
        {
            case "Neighborhood":
                videoPlayer.clip = neighborhoodLoadingScreen;
                break;
            case "TrainStation":
                videoPlayer.clip = trainStationLoadingScreen;
                break;
            case "Apartment":
                videoPlayer.clip = apartamentLoadingScreen;
                break;
        }

        float elapsedTime = 0f;
        float minimumWaitTime = 5f;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            elapsedTime += Time.deltaTime;

            if (operation.progress >= 0.9f && elapsedTime >= minimumWaitTime)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        IsLoading = false;
    }
}