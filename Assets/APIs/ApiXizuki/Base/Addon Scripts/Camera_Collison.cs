using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Camera_Collison : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask layerMask;

    [Header("Values")]
    public Transform Point;
    public float midPoint;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        #region Ray Cast

        Vector3 angle = Point.position - transform.position;

        RaycastHit hit;

        Debug.DrawRay(Point.position, -angle);
        Physics.Raycast(Point.position, -angle, out hit, angle.magnitude, layerMask);

        #endregion

        #region Hit Condition

        if (hit.collider == null) { return; }

        if ((hit.point - Point.position).magnitude < midPoint) { return; }

        #endregion

        transform.position = hit.point;
    }
}
