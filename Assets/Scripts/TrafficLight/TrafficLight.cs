using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField]private GameObject redLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject greenLight;

    [SerializeField] private float redLightDuration = 5f;
    [SerializeField] private float yellowLightDuration = 2f;
    [SerializeField] private float greenLightDuration = 5f;

    private void Start()
    {
        StartCoroutine(TrafficLightCycle());
    }

    IEnumerator TrafficLightCycle()
    {
        while (true)
        {
            // Red light on
            redLight.SetActive(true);
            yellowLight.SetActive(false);
            greenLight.SetActive(false);
            yield return new WaitForSeconds(redLightDuration);

            // Green light on
            redLight.SetActive(false);
            yellowLight.SetActive(false);
            greenLight.SetActive(true);
            yield return new WaitForSeconds(greenLightDuration);

            // Yellow light on
            redLight.SetActive(false);
            yellowLight.SetActive(true);
            greenLight.SetActive(false);
            yield return new WaitForSeconds(yellowLightDuration);
        }
    }
}
