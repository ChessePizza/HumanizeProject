using System.Collections;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Passability))]
public class PassabilityEditor : Editor
{
    public static bool swap;
    public override void OnInspectorGUI()
    {
        Passability o = (Passability)target;

        string button = o.passable ? "Impassable" : "Passable";

        if (GUILayout.Button(button))
        {
            o.passable = !o.passable;
            o.text = o.passable ? "O" : "X";
            // Passibility > Editor > GridMap
            GridMap grid = o.gameObject.transform.parent.transform.parent.GetComponent<GridMap>();
            // คัวอย่างเช่น passability_0_0
            string[] coord = o.gameObject.name.Split('_');

            grid.data[(int.Parse(coord[1]) * grid.size.y) + int.Parse(coord[2])] = (byte)(o.passable ? 1 : 0);
            EditorUtility.SetDirty(o);
            EditorUtility.SetDirty(o.gameObject);
        }
    }
}
