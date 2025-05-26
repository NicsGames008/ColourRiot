using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAlbumPage : MonoBehaviour
{
    [SerializeField] private Tag tagOnThisPag;
    [SerializeField] private GameObject page;
    [SerializeField] private GameManager gameManager;

    private GameObject player;
    private Album album;

    void OnEnable()
    {
        player = gameManager.ReturnPlayer();
        album = player.GetComponent<Album>();

        foreach (var tag in album.tags)
        {
            if (tag == tagOnThisPag)
            {
                page.SetActive(true);
            }
        }
    }
}
