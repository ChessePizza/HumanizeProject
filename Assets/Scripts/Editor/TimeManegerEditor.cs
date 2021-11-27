using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimeManeger))]
public class TimeManegerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TimeManeger timer = (TimeManeger)target;
        if (GUILayout.Button("Skip Time"))
        {
            TimeManeger.Hour += timer.hours;
            TimeManeger.Minute += timer.minutes;
        }
        if (GUILayout.Button("Revert Time"))
        {
            TimeManeger.Hour -= timer.hours;
            TimeManeger.Minute -= timer.minutes;
        }
        if (GUILayout.Button("Set Time"))
        {
            TimeManeger.Hour = timer.hours;
            TimeManeger.Minute = timer.minutes;
        }
    }
}
