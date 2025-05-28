using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneLoadManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private VideoClip neighborhoodLoadingScreen;
    [SerializeField] private VideoClip trainStationLoadingScreen;
    [SerializeField] private VideoClip apartamentFromNeighborhoodLoadingScreen;
    [SerializeField] private VideoClip apartamentFromTrainStationLoadingScreen;
    [SerializeField] private VideoClip apartamentFromMainScreenLoadingScreen;

    public static bool IsLoading { get; private set; }

    public IEnumerator LoadSceneAsynchronously(string sceneName, string sceneComesFrom)
    {
        IsLoading = true;
        loadingScreen.SetActive(true);

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

        Debug.Log(sceneName + " " + sceneComesFrom);

        switch (sceneName)
        {
            case "Neighborhood":
                videoPlayer.clip = neighborhoodLoadingScreen;
                break;
            case "TrainStation":
                videoPlayer.clip = trainStationLoadingScreen;
                break;
            case "Apartment":
                if (sceneComesFrom == "Neighborhood")
                {
                    videoPlayer.clip = apartamentFromNeighborhoodLoadingScreen;
                }
                else if (sceneComesFrom == "TrainStation")
                {
                    videoPlayer.clip = apartamentFromTrainStationLoadingScreen;
                }
                else if (sceneComesFrom == "MainScreen")
                {
                    videoPlayer.clip = apartamentFromMainScreenLoadingScreen;
                }
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
                IsLoading = false;

            }

            yield return null;
        }

        IsLoading = false;
    }
}