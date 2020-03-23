using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GrappleCreator))]
public class GrappleCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GrappleCreator myTarget = (GrappleCreator)target;
        myTarget.grappleLink = EditorGUILayout.ObjectField("Link Obj", myTarget.grappleLink, typeof(GameObject), false) as GameObject;
        myTarget.startPos = EditorGUILayout.Vector3Field("Start", myTarget.startPos);
        myTarget.endPos = EditorGUILayout.Vector3Field("End", myTarget.endPos);
        myTarget.desiredLinkDist = EditorGUILayout.FloatField("Gap Dist", myTarget.desiredLinkDist);
        if (GUILayout.Button("Create Tether"))
        {
            myTarget.createTether();
        }


    }
}
