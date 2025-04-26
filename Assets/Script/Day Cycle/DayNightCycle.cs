using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Cycle Settings")]
    public float cycleDuration = 360f; 
    [Header("Lighting")]
    public Light sun;
    public Light moon;
    public AnimationCurve sunIntensityCurve;
    public AnimationCurve moonIntensityCurve;

    [Header("Visuals")]
    public GameObject stars;
    public Gradient ambientColor;
    public Gradient fogColor;

    private float time;
    private bool cycleRunning = false;



    void Update()
    {
        if (!cycleRunning)
            return; 


        time += Time.deltaTime / cycleDuration;
        time = Mathf.Clamp01(time);

        float sunAngle = Mathf.Lerp(-90f, 90f, time);
        sun.transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
        moon.transform.rotation = Quaternion.Euler(sunAngle - 180f, 0f, 0f);

        sun.intensity = sunIntensityCurve.Evaluate(time);
        moon.intensity = moonIntensityCurve.Evaluate(time);

        if (stars != null)
        {
            float starsAlpha = 1f - time * 2f;
            starsAlpha = Mathf.Clamp01(starsAlpha);
            SetStarsAlpha(starsAlpha);
        }

        RenderSettings.ambientLight = ambientColor.Evaluate(time);
        RenderSettings.fogColor = fogColor.Evaluate(time);
    }



    public void StartCycle()
    {
        cycleRunning = true;
    }

    void SetStarsAlpha(float alpha)
    {
        foreach (var renderer in stars.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
        }
    }
}
