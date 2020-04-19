using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InitMesh : MonoBehaviour
{
    [SerializeField] Mesh[] randMeshs = null;

    public void RandMesh()
    {
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < filters.Length; i++)
        {
            filters[i].sharedMesh = randMeshs[Random.Range(0, randMeshs.Length)];
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(InitMesh))]
public class InitMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Rand Mesh"))
        {
            ((InitMesh)target).RandMesh();
        }
    }
}
#endif