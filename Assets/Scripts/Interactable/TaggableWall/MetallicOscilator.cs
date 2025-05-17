using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetallicOscilator : MonoBehaviour
{
    private Material targetMaterial;
    [SerializeField] private float speed = 0.25f;

    private float metallicValue = 0f;
    private bool increasing = true;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create an instance so we don't modify the original material asset
            targetMaterial = renderer.material;
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
            metallicValue += Time.deltaTime * speed;
            if (metallicValue >= 0.75f)
            {
                metallicValue = 0.75f;
                increasing = false;
            }
        }
        else
        {
            metallicValue -= Time.deltaTime * speed;
            if (metallicValue <= 0f)
            {
                metallicValue = 0f;
                increasing = true;
            }
        }

        targetMaterial.SetFloat("_Metallic", metallicValue);
    }
}
