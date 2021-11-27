using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class AutoLight : MonoBehaviour
{
    Light2D light;
    // Start is called before the first frame update
    void Start()
    {
        light = GetComponent<Light2D>();
        light.intensity = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeManeger.Hour >= 18 && TimeManeger.Hour <= 23)
        {
            light.intensity = 1;
        }
        else if (TimeManeger.Hour >= 0 && TimeManeger.Hour <= 6)
        {
            light.intensity = 1;
        }
        else
        {
            light.intensity = 0;
        }
    }
}
