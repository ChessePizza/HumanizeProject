using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManeger : MonoBehaviour
{
    public int hours;
    public int minutes;
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute { get; set; }
    public static int Hour { get; set; }

    private float minuteToRealTime = 0.5f;
    private float timer;

    private void Start()
    {
        Minute = 0;
        Hour = 15;
        timer = minuteToRealTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <=0)
        {
            Minute++;
            OnMinuteChanged?.Invoke();
            if(Minute >= 60)
            {
                Hour++;
                Minute = 0;
                OnHourChanged?.Invoke();
            }
            if (Hour >= 24)
            {
                Hour = 0;
            }
            timer = minuteToRealTime;
        }
    }
}
