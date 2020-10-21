using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyPath))]
public class EnemyPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EnemyPath MyEnemyPathScript = (EnemyPath)target;

        DrawDefaultInspector();

        //MyEnemyPathScript.m_PathPointColour = EditorGUILayout.IntField ("StayTime", MyEnemyPathScript.m_FirstPoint.m_StayTime);
        if (GUILayout.Button("Add Point"))
        {
            MyEnemyPathScript.AddPoint();
        }

        if (GUILayout.Button("Delete Point"))
        {
            MyEnemyPathScript.DeletePoint();
        }

    }
}
