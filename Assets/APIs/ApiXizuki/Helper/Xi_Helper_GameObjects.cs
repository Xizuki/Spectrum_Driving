using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XizukiMethods
{
    namespace GameObjects
    {
        public static class Xi_Helper_GameObjects
        {
            // about twice as fast as than (GetComponent<Script>()) { variable = GetComponent<Script>(); }
            public static bool ConditionalAssignment<T>(ref T variable, T getValue)
            {
                if (getValue != null)
                {
                    variable = getValue;
                    return true;
                }
                else
                    return false;

            }
            public static T ConditionalAssignment<T>(T getValue)
            {
                if (getValue != null)
                {
                    return getValue;
                }
                else
                    return default(T);

            }
            public static bool MonoInitialization<T>(ref T variable, T AssignedValue)
            {
                if (variable is null)           // < Works for Unity Defined Classes
                {
                    variable = AssignedValue;
                    return true;
                }
                else if (variable.Equals(null)) // < Works for self defined Classes but not for ( Unity GetComponent<>() )   
                {
                    variable = AssignedValue;
                    return true;
                }
                // ^ No Idea why its different
                return false;
            }



            public static GameObject[] FilterOutWithScript<T>(GameObject[] GOs)
            {
                // Filter Out Gameobjects that doesnt have component <T>
                List<int> falseIndexes = new List<int>();
                for (int i = 0; i < GOs.Length; i++)
                {
                    if (GOs[i].GetComponent<T>() == null)
                    {
                        falseIndexes.Add(i);
                    }
                }
                return XizukiMethods.Array.Xi_Array.Remove(GOs, falseIndexes);
            }


            /// <summary>
            /// Takes a reference array of GameObjects and Filter out a specific MonoBehaviour into another Array, 
            /// leaving the original array without the new array
            /// </summary>
            /// <typeparam name="T"> The MonoBehaviour Type to filter out </typeparam>
            /// <param name="GOs"> The Referenced GameObject Array to filter Classes out of </param>
            /// <returns></returns>
            public static T[] FilterOutWithScript<T>(ref GameObject[] GOs)
            {
                // Filter Out Gameobjects that doesnt have component <T>
                // and return an array of component <T>

                List<int> falseIndexes = new List<int>();
                T[] scripts = new T[GOs.Length];
                List<int> trueIndexes = new List<int>();

                for (int i = 0; i < GOs.Length; i++)
                {
                    if (GOs[i].GetComponent<T>() == null)
                    {
                        falseIndexes.Add(i);
                    }
                    else
                    {
                        scripts[i] = GOs[i].GetComponent<T>();
                        trueIndexes.Add(i);
                    }
                }
               

                scripts = XizukiMethods.Array.Xi_Array.Remove(scripts, falseIndexes);

                GOs = XizukiMethods.Array.Xi_Array.Remove(GOs, trueIndexes);
                return scripts;
            }


        }
    }
}