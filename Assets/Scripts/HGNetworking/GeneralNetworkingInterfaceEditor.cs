using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneralNetworkingInterface))]
public class GeneralNetworkingInterfaceEditor : Editor
{
    SerializedProperty serverIpAddress;
    SerializedProperty username;

    private void OnEnable()
    {
        serverIpAddress = serializedObject.FindProperty("ipAddress");
        username = serializedObject.FindProperty("username");
    }

    public override void OnInspectorGUI()
    {
        GeneralNetworkingInterface myTarget = (GeneralNetworkingInterface)target;
        serializedObject.Update();
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.PropertyField(serverIpAddress);

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
            myTarget.ConnectLocally();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(username);
        if (GUILayout.Button("Send Username Event"))
        {
            Events.SEND_USERNAME e = new Events.SEND_USERNAME();
            e.Username = username.stringValue;
            HG.Utils.EventManagerUtils.SendEventToAllConnections(e);
        }



        //EditorGUILayout.LabelField("MISC");
        //if (GUILayout.Button("Create A Ghost"))
        //{
        //    myTarget.CreateGhost();
        //}
        //if (GUILayout.Button("Ball"))
        //{
        //    myTarget.Ball();
        //}
        //if (GUILayout.Button("Give Control 0 to 1"))
        //{
        //    myTarget.GiveGhost0To1();
        //}
        //if (GUILayout.Button("Give Control 1 to 2"))
        //{
        //    myTarget.GiveGhost1To2();
        //}

    }
}
