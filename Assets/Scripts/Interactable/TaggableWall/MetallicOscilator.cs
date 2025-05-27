using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowingOscillator : MonoBehaviour
{
    private Material targetMaterial;
    [SerializeField] private float speed = 0.25f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.5f;

    private float currentAlpha;
    private bool increasing = true;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create an instance so we don't modify the original material asset
            targetMaterial = renderer.material;
            currentAlpha = minAlpha; // Start at minimum alpha
        }
        else
        {
            Debug.LogError("No Renderer found on this GameObject!");
        }
    }

    void Update()
    {
        if (targetMaterial == null)
            return;

        if (increasing)
        {
            currentAlpha += Time.deltaTime * speed;
            if (currentAlpha >= maxAlpha)
            {
                currentAlpha = maxAlpha;
                increasing = false;
            }
        }
        else
        {
            currentAlpha -= Time.deltaTime * speed;
            if (currentAlpha <= minAlpha)
            {
                currentAlpha = minAlpha;
                increasing = true;
            }
        }

        // Set the alpha value in the material's color
        Color color = targetMaterial.color;
        color.a = currentAlpha;
        targetMaterial.color = color;
    }
}