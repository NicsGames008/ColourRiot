using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private AudioClip backgroudMusic;
    private AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        if (backgroudMusic != null && musicSource != null)
        {
            musicSource.clip = backgroudMusic;
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("dumb dumb forgot audio.");
        }
    }



public void ChangeMusic(AudioClip newClip)
    {
        if (newClip == null)
        {
        Debug.LogWarning("yep you forgot the audio boy.");
        return;
        }

        if (musicSource.isPlaying)
        {
        musicSource.Stop();
        }

        musicSource.clip = newClip;
        musicSource.loop = true;
        musicSource.Play();
    } 
}
