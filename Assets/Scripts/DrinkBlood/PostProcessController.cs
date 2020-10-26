using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public struct VignetteProperties
{
    public float triggerDrinkIntensity;
    public float drinkingSmoothLowValue;
    public float drinkingSmoothHighValue;
    public float drinkingSmmothSpeed;
    public Color drinkingColor;
}

public class PostProcessController : MonoBehaviour
{
    
    public VignetteProperties vigPros;

    private Volume  volume;
    private Vignette vignette;
    private bool isDrinking = false;
    private float vignetteDefaultSmooth;
    private Color vignetteDefaultColor;
    
    // Start is called before the first frame update
    void Start()
    {
        volume = gameObject.GetComponent<Volume>();
        volume.profile.TryGet<Vignette>(out vignette);
        vignetteDefaultSmooth = vignette.smoothness.value;
        vignetteDefaultColor = vignette.color.value;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDrinking)
        {
            float range = vigPros.drinkingSmoothHighValue - vigPros.drinkingSmoothLowValue;
            vignette.smoothness.value = vigPros.drinkingSmoothLowValue + Mathf.PingPong(
                Time.time * vigPros.drinkingSmmothSpeed, range
            );
        }
    }

    public void SetDrinkTrigger(bool setting)
    {
        if (setting)
        {
            vignette.intensity.value = vigPros.triggerDrinkIntensity;
        } else
        {
            vignette.intensity.value = 0f;
        }
    }

    public void SetDrinking(bool setting)
    {
        if (setting)
        {
            isDrinking = true;
            vignette.smoothness.value = vigPros.drinkingSmoothLowValue;
            vignette.color.value = vigPros.drinkingColor;
        } else
        {
            isDrinking = false;
            vignette.smoothness.value = vignetteDefaultSmooth;
            vignette.color.value = vignetteDefaultColor;
        }
    }
    


}
