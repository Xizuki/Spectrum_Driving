using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Xi_MouseInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public RaycastHit GetOnClickHit(Camera cam)
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        return hit;
    }
}
