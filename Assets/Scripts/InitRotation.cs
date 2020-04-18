using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InitRotation : MonoBehaviour
{
    public void Rotate()
    {
        Transform transform = GetComponent<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).localEulerAngles = new Vector3(0, Random.Range(0, 4) * 90, 0);
        }
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(InitRotation))]
public class InitRotationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Rotate"))
        {
            ((InitRotation)target).Rotate();
        }
    }
}
#endif