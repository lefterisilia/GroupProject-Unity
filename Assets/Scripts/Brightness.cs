using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;

public class Brightness : MonoBehaviour
{


    public Slider brightnessSlider;


    public PostProcessProfile brightness;
    public PostProcessLayer layer;

    AutoExposure exposure;



    void Start()
    {
        brightness.TryGetSettings(out exposure);
        AdjustBrightness(brightnessSlider.value);
    }

    public void AdjustBrightness(float value)
    {
        if (value !=0 )
        {
            exposure.keyValue.value = value;
        }
        else
        {
            exposure.keyValue.value = .05f;
        }
    }
}
