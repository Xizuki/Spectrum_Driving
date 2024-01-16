using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamTargetType { distance , offset }

[ExecuteInEditMode]
public class Xi_Camera_Targetting : MonoBehaviour
{
    [Header("Settings")]
    //public bool isInitialized;
    public GameObject target;
    public GameObject centerPoint;
    public CamTargetType type;

    [Header("Distance Values")]
    public float distance;
    public Vector2 angle;
    public Vector3 targetToCenterPoint;

    [Header("Offset Values")]
    public Vector3 offset;

    public void Awake()
    {
        Initailize(target, centerPoint);
        UpdatePosition();
    }

    public void Initailize(GameObject targetGO, GameObject centerPointGO)
    {
        //isInitialized = true;

        target = targetGO;
        centerPoint = centerPointGO;
        targetToCenterPoint = (centerPoint.transform.position - target.transform.position);
    }

    public void UpdatePosition()
    {
        if (type == CamTargetType.distance)
        {
            transform.eulerAngles = angle;
            target.transform.position = centerPoint.transform.position - (new Vector3(
                                                                        transform.forward.x * targetToCenterPoint.magnitude, 
                                                                        targetToCenterPoint.y,
                                                                        transform.forward.z * targetToCenterPoint.magnitude));

            transform.position = centerPoint.transform.position + (transform.forward * distance);
        }
        else if (type == CamTargetType.offset)
        {
            transform.position = target.transform.position + offset;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //InitializePosition();
        // if (!isInitialized) { return; }
        transform.LookAt(target.transform);
    }
}
