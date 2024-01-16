using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xi_MultiArray : MonoBehaviour 
{
    //
    [System.Serializable]
    public struct rowData
    {
        public bool[] row;
    }

    public rowData[] rows = new rowData[2];
}
