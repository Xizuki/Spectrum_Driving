using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace XizukiMethods
{
    namespace Numbers
    {
        public static class Xi_Helper_Nums
        {
            /// <summary>
            /// it takes an input from 0-1 and scales it non linearly using a scalar parameter, and returns a value thats 0-1
            /// 
            /// Example 1-((1-0.5)^5)
            /// </summary>
            public static float ScaleNonLinearly(float input, float scalar)
            {
                float scaledValue = (float)(1 - Mathf.Pow(1 - input, scalar));
                return scaledValue;
            }

         
        }
    }
}