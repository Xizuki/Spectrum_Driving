using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XizukiMethods
{
    namespace Array
    {
    
        public static class Xi_Array
        {
            public static T[] AppendArray<T>(T[] array, T appendedValue)
            {
                T[] newArray = new T[array.Length + 1];

                int i;
                for (i = 0; i < array.Length; i++)
                {
                    newArray[i] = array[i];
                }
                newArray[i] = appendedValue;
                return newArray;
            }


            public static T[] Remove<T>(T[] array, int[] removingPositions)
            {
                T[] newArray = new T[array.Length - removingPositions.Length];

                int l = 0;
                for (int i = 0; i < newArray.Length; i++)
                {
                    if (i == removingPositions[l])
                    {
                        i++;
                    }
                    else
                    { 
                        newArray[i] = array[i];
                    }
                }
                return newArray;
            }
            public static T[] Remove<T>(T[] array, List<int> removingPositions)
            {
                // removingPositiions must be in ascending orders
                T[] newArray = new T[array.Length - removingPositions.Count];

                //0, 2, 3, 4, 5

                int offset = 0;
                for(int i =0; i < array.Length; i++)
                {
                    if (offset < removingPositions.Count)
                    {
                        if (i == removingPositions[offset])
                        {
                            offset++;
                        }
                        else
                        {
                            newArray[i - offset] = array[i];
                        }
                    }
                    else
                    {
                        newArray[i - offset] = array[i];
                    }
                }
              
                return newArray;
            }
            /* Random Indexes to remove - To Do
            public static T[] Remove<T>(T[] array, List<int> removingPositions)
            {
                T[] newArray = new T[array.Length - removingPositions.Count];

                for (int i = 0; i < array.Length; i++)
                {
                    for (int l = 0; l < removingPositions.Count; l++)
                    {
                        if (i == removingPositions[l])
                        {

                        }
                    }
                }
                return newArray;
            }
            */

            public static bool OutOfArray<T>(T array, int value)
            {
                bool result = false;

                return result;
            }
        }


    }
}
