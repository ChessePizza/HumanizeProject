using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    public Color dayLight;
    public Color sunsetLight;
    public Color nightLight;
    public Color midnightLight;

    Light2D light;
    Color currentLight;
    // Start is called before the first frame update
    void Start()
    {
        currentLight = dayLight;
        light = GetComponent<Light2D>();
        light.color = currentLight;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManeger.Hour == 17 && currentLight != sunsetLight)
        {
            currentLight = sunsetLight;
            light.color = sunsetLight;
        }
        else if(TimeManeger.Hour >= 18 && TimeManeger.Hour <= 23 && currentLight != nightLight)
        {
            currentLight = nightLight;
            light.color = nightLight;
        }
        else if (TimeManeger.Hour >= 0 && TimeManeger.Hour <= 5 && currentLight != midnightLight)
        {
            currentLight = midnightLight;
            light.color = midnightLight;
        }
        else if (TimeManeger.Hour >= 5 && TimeManeger.Hour <= 6 && currentLight != nightLight)
        {
            currentLight = nightLight;
            light.color = nightLight;
        }
    }
}
