using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Sun : MonoBehaviour
{
    public float fadeDuration;
    [Header("Light Color")]
    public Color dayLight;
    public Color sunsetLight;
    public Color eveningLight;
    public Color latenightLight;
    public Color dawnLight;

    Light2D light;
    Color originLight;
    Color targetLight;
    float time;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        targetLight = dayLight;
        light = GetComponent<Light2D>();
        light.color = dayLight;
    }

    // Update is called once per frame
    void Update()
    {
        if (
            ((TimeManeger.Hour >= 12 && TimeManeger.Hour <= 15) || (TimeManeger.Hour == 16 && TimeManeger.Minute <= 30))
            && targetLight != dayLight)
        {
            time = 0;
            targetLight = dayLight;
            originLight = light.color;
        }
        if (
            (TimeManeger.Hour == 17 || (TimeManeger.Hour == 16 && TimeManeger.Minute > 30))
            && targetLight != sunsetLight)
        {
            time = 0;
            targetLight = sunsetLight;
            originLight = light.color;
        }
        else if (TimeManeger.Hour >= 18 && TimeManeger.Hour <= 23 && targetLight != eveningLight)
        {
            time = 0;
            targetLight = eveningLight;
            originLight = light.color;
        }
        else if (TimeManeger.Hour >= 0 && TimeManeger.Hour < 5 && targetLight != latenightLight)
        {
            time = 0;
            targetLight = latenightLight;
            originLight = light.color;
        }
        else if (TimeManeger.Hour >= 5 && TimeManeger.Hour <= 6 && targetLight != dawnLight)
        {
            time = 0;
            targetLight = dawnLight;
            originLight = light.color;
        }

        light.color = Color.Lerp(originLight, targetLight, time);
        if (time < 1)
        {
            time += Time.deltaTime / fadeDuration;
        }
    }
}
