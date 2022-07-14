using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneralNetworkingManager))]
public class GeneralNetworkingManagerEditor : Editor
{
    SerializedProperty serverIpAddress;

    private void OnEnable()
    {
        serverIpAddress = serializedObject.FindProperty("ipAddress");
    }

    public override void OnInspectorGUI()
    {
        GeneralNetworkingManager myTarget = (GeneralNetworkingManager)target;
        serializedObject.Update();
        EditorGUILayout.PropertyField(serverIpAddress);
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.LabelField("Starting Server/Client");
        if (GUILayout.Button("Start Server"))
        {
            myTarget.StartServer();
        }
        if (GUILayout.Button("Start Client and Connect To Server"))
        {
            myTarget.StartClient();
            myTarget.ConnectClientToServer();
        }
        if (GUILayout.Button("Start Client and Connect To Local Server"))
        {
            myTarget.StartClient();
            myTarget.ConnectClientToServer();
        }
        EditorGUILayout.LabelField("MISC");
        if (GUILayout.Button("Give Control Of Ghost"))
        {
            myTarget.GiveControlOfGhost();
        }
    }
}
