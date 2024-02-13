using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerpendicularTestScript : MonoBehaviour
{
    public Transform start;
    public Transform end;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vector = (end.position - start.position);


        Vector2 pVector = new Vector2 (vector.x, vector.z);
        pVector = Vector2.Perpendicular(pVector);

        Debug.DrawRay(transform.position, pVector, Color.red, 1f);
        //Debug.DrawRay(transform.position, -pVector, Color.red, 1f);
        //Debug.DrawRay(new Vector3(0,0,0), new Vector3(3,10,5), Color.red, 1f);

        Debug.DrawRay(transform.position, Quaternion.Euler(0f, 30f, 0) * pVector, Color.green, 1f);
    }


  
}
