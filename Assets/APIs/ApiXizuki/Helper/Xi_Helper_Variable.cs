using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XizukiMethods
{
    namespace Variable
    {
        public static class Xi_Helper_Variable
        {
            public static bool Clamp<T>(ref T variable, T getValue)
            {
                if (getValue != null)
                {
                    variable = getValue;
                    return true;
                }
                else
                    return false;

            }
        }

        public static class Xi_Helper_ArrayVariable
        {
            public static int Flow(int index, int increment, int length)
            {
                int incrementalValue = (index + increment);

                int result = incrementalValue;

                if (incrementalValue > length)
                {
                    result = incrementalValue - length;
                }

                if (incrementalValue < 0)
                {
                    result = -1+length-incrementalValue;
                }

                return result;
            }
            public static bool EdgeFlow(ref int index, int increment, int length)
            {
                int incrementalValue = (index + increment);
                bool result = false;

                if (incrementalValue > length)
                {
                    index = incrementalValue - length;

                    result = true;
                }

                if (incrementalValue < 0)
                {
                    index = -1 + length - incrementalValue;

                    result = true;
                }

                return result;
            }

            public static bool EdgeCheck(int index, int increment, int length)
            {
                int incrementalValue = (index + increment);

                bool result = false;

                if (incrementalValue > length)
                {
                    result = true;
                }

                if (incrementalValue < 0)
                {
                    result = true;
                }

                return result;
            }
        }
    }
}