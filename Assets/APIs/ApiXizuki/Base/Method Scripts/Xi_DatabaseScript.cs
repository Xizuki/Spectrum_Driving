using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace XizukiMethods
{
    namespace Data
    {
        public static class Xi_DataBase
        {
            /*
            public static GameObject[] GetPrefabsFromPath(string folderPath)
            {
                //filter - t:prefab to get prefabs
                string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });
                GameObject[] GOs = new GameObject[GUIDs.Length];
                for (int i = 0; i < GUIDs.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                    GOs[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }

                return GOs;
            }

            public static GameObject[] GetPrefabsFromPath<T>(string folderPath)
            {
                //filter - t:prefab to get prefabs
                string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { folderPath });
                List<int> falseIndexes = new List<int>();
                for (int i = 0; i < GUIDs.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                    if (AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<T>() == null)
                    {
                        falseIndexes.Add(i);
                    }
                }
                GUIDs = XizukiMethods.Array.Xi_Array.Remove(GUIDs, falseIndexes);
                GameObject[] gridObjects = new GameObject[GUIDs.Length];
                string[] gridObjectsNames = new string[GUIDs.Length];

                for (int i = 0; i < GUIDs.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                    gridObjects[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                }
                return gridObjects;
            }

            public static Sprite[] GetSpritesFromPath(string folderPath)
            {
                //filter - t:sprite to get sprites
                string[] GUIDs = AssetDatabase.FindAssets("t:sprite", new string[] { folderPath });
                Sprite[] sprites = new Sprite[GUIDs.Length];
                for (int i = 0; i < GUIDs.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
                    sprites[i] = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }

                return sprites;
            }
            /*
             * 
             *   string[] GUIDs = AssetDatabase.FindAssets("t:prefab", new string[] { "Assets/GridObjects" });
    List<int> falseIndexes = new List<int>();
    for (int i = 0; i < GUIDs.Length; i++)
    {
        string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);
        if (AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<Xi_HexaGrid_Object>() == null) 
        {
            falseIndexes.Add(i);
        }
    }
    GUIDs = Xi_Array.Remove(GUIDs, falseIndexes);
    gridObjects = new GameObject[GUIDs.Length];
    gridObjectsNames = new string[GUIDs.Length];    

    for (int i = 0; i < GUIDs.Length; i++)
    {
        string path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);          
        gridObjects[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        gridObjectsNames[i] = gridObjects[i].GetComponent<Xi_HexaGrid_Object>().objectName;
    }
             */

        }
    }
}
