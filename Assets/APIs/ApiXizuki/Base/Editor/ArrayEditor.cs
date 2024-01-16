using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// IDK IF THIS WORKS.
// I FORGOTTEN ABOUT IT AFTER FINISHING AND I DONT UNDERSTAND IT.

namespace XizukiMethods
{
    #region Array Element Naming Attribute
    public class ArrayElementNames : PropertyAttribute
    {
        public string[] names;
        public System.Type type;
        public ArrayElementNames(System.Type Enum) { names = System.Enum.GetNames(Enum); }
        public ArrayElementNames(string[] names) { this.names = names; }
    }


    [CustomPropertyDrawer(typeof(ArrayElementNames))]
    public class ArrayElementNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            Debug.Log("RUN RUN RUN RUN");
            try
            {
                int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                EditorGUI.PropertyField(rect, property, new GUIContent(((ArrayElementNames)attribute).names[pos]));
            }
            catch
            {
                EditorGUI.PropertyField(rect, property, label);
            }

        }

    }
    #endregion
}

