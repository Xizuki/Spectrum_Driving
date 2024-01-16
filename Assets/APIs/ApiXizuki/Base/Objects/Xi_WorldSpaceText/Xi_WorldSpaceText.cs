using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Xi_WorldSpaceText : MonoBehaviour
{
    public GameObject anchorTarget;
    public Vector3 offset;
    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        offset = anchorTarget.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = anchorTarget.transform.position - offset;
    }
}
