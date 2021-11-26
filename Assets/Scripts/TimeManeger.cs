using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TimeManeger : MonoBehaviour
{
    public static Action OnMinuteChanged;
    public static Action OnHourChanged;

    public static int Minute { get; private set; }
    public static int Hour { get; private set; }

    private float minuteToRealTime = 0.5f;
    private float timer;

    public Vector2Int time; 

    private void Start()
    {
        Minute = time.y;
        Hour = time.x;
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
