using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Enemy))]
public class FieldOfViewEditor : Editor
 {
    Enemy e;

     private void OnSceneGUI() {
        e = (Enemy)target;
        Handles.color = Color.cyan;
        Handles.DrawWireArc(e.transform.position, Vector3.forward, Vector2.right, 360, e.visionRange);

        Vector3 viewAngleA = DirFromAngle(-e.visionConeAngle/2);
        Vector3 viewAngleB = DirFromAngle(e.visionConeAngle/2);

        Handles.DrawLine(e.transform.position, e.transform.position+viewAngleA*e.visionRange);
        Handles.DrawLine(e.transform.position, e.transform.position+viewAngleB*e.visionRange);
    }

    private Vector2 DirFromAngle(float angleInDegrees)
    {
        angleInDegrees += e.transform.eulerAngles.z;
        return new Vector2(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad));
    }
}