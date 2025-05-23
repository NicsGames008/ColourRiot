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
        musicSource.clip = backgroudMusic;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
