using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerController))]
public class DistanceAnchorUpdate : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerController playerController = (PlayerController)target;
        if(GUILayout.Button("update distance joint connected anchor"))
        {
            playerController.UpdateDistanceJointConnectedAnchor();
        }
    }
}
