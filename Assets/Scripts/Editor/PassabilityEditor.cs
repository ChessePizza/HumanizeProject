using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Passability))]
public class PassabilityEditor : Editor
{
    public static bool swap;
    public override void OnInspectorGUI()
    {
        EditorCoroutineUtility.StartCoroutine(SetPassability(), this);
    }
    IEnumerator SetPassability()
    {
        Passability o = (Passability)target;

        if (PassabilityEditor.swap == false)
        {
            Object[] objects = { o.gameObject };
            Selection.objects = objects;

            PassabilityEditor.swap = true;

            yield return null;
        }
        else
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

            Selection.objects = null;

            yield return new WaitForSeconds(1);
            
            PassabilityEditor.swap = false;
            Object[] objects = { grid.gameObject };
            Selection.objects = objects;

            yield return null;
        }
    }
}
