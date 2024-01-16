using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XizukiMethods
{
    public static class XizukiEnum
    {
        public static int GetEnumLength(System.Type Enum)
        {
            return Enum.GetEnumNames().Length;
        }
    }
}
