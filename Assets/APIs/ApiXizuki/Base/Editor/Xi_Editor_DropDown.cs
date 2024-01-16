/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;



BASE FOR ARRAY DROPDOWN

Couldnt Make it Modular


[CustomEditor(typeof(Xi_HexaGridZone_Editor))]
public class Xi_Editor_DropDown : Editor
{
    private void OnEnable()
    {
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Xi_HexaGridZone_Editor hostScript = (Xi_HexaGridZone_Editor)target;

        GUIContent arrayLabel = new GUIContent("MyArray");

        hostScript.currentIndex = EditorGUILayout.Popup(arrayLabel, hostScript.currentIndex, hostScript.array);
    }

}

*/