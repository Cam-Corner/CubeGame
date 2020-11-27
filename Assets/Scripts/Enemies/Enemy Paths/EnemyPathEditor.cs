using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

#if UNITY_EDITOR
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


    [MenuItem("Component/AI/AddEnemyPath")]
    private static void AddEnemyPathToScene()
    {
        GameObject GO = new GameObject();

        GO.transform.name = "EnemyPath";
        GO.transform.position = new Vector3(0, 0, 0);
        GO.AddComponent<EnemyPath>();
    }


    [MenuItem("GameObject/AI/AddEnemyPath", false , 0)]
    private static void AddEnemyPathToScene1()
    {
        GameObject GO = new GameObject();

        GO.transform.name = "EnemyPath";
        GO.transform.position = new Vector3(0, 0, 0);
        GO.AddComponent<EnemyPath>();
    }
}
#endif