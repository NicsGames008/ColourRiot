using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomPoster : MonoBehaviour
{
    private Image imageComponent;           // The UI Image component
    [SerializeField] private Sprite[] possibleImages;        // Array of random sprites to choose from

    void Start()
    {
        imageComponent = GetComponent<Image>();

        if (possibleImages.Length == 0 || imageComponent == null)
            return;

        // Choose a random sprite
        Sprite randomSprite = possibleImages[Random.Range(0, possibleImages.Length)];

        float zRotation = Random.Range(-15f, 15f);
        Vector3 currentEuler = imageComponent.transform.localEulerAngles;
        imageComponent.transform.localEulerAngles = new Vector3(currentEuler.x, currentEuler.y, zRotation);


        // Assign and resize to native size
        imageComponent.sprite = randomSprite;
        FitImageToSize(imageComponent, new Vector2(2.5f, 2.5f)); // Max width/height

    }

    void FitImageToSize(Image image, Vector2 maxSize)
    {
        Sprite sprite = image.sprite;
        if (sprite == null) return;

        float spriteWidth = sprite.rect.width;
        float spriteHeight = sprite.rect.height;

        float widthRatio = maxSize.x / spriteWidth;
        float heightRatio = maxSize.y / spriteHeight;
        float scaleFactor = Mathf.Min(widthRatio, heightRatio);

        image.rectTransform.sizeDelta = new Vector2(spriteWidth * scaleFactor, spriteHeight * scaleFactor);
    }

}