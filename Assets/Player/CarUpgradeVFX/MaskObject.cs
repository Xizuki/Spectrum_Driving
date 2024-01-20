using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MaskObject : MonoBehaviour
{
    public int renderQueue = 3002;
    // Start is called before the first frame update

    public MeshRenderer[] meshRenderers;

    public bool inverseMask;

    private void OnValidate()
    {
        //GetComponent<MeshRenderer>().material.renderQueue = renderQueue;
        meshRenderers = GetComponentsInChildren<MeshRenderer>();

    }
    void Start()
    {
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().material.renderQueue = renderQueue;
        if (GetComponent<MeshRenderer>() != null)  
            GetComponent<MeshRenderer>().material.renderQueue = renderQueue ;


        meshRenderers = GetComponentsInChildren<MeshRenderer>();

        for(int i = 0; i < meshRenderers.Length; i++)
        {
            for(int j = 0; j < meshRenderers[i].materials.Length; j++)
            {
                meshRenderers[i].materials[j].renderQueue = renderQueue;

                if (inverseMask)
                    meshRenderers[i].materials[j].SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            }
        }



    }


    [ContextMenu("Set")]
    public void Set()
    {
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().sharedMaterial.renderQueue = renderQueue;
        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().sharedMaterial.renderQueue = renderQueue;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            for (int j = 0; j < meshRenderers[i].sharedMaterials.Length; j++)
            {
                meshRenderers[i].sharedMaterials[j].renderQueue = renderQueue;

                if(inverseMask) 
                    meshRenderers[i].sharedMaterials[j].SetInt("_StencilComp", (int)CompareFunction.GreaterEqual);
            }
        }
    }

    public bool onUpdate;

    // Update is called once per frame
    void Update()
    {
        if (!onUpdate) return;

        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().material.renderQueue = renderQueue;
        if (GetComponent<MeshRenderer>() != null)
            GetComponent<MeshRenderer>().material.renderQueue = renderQueue;

        for (int i = 0; i < meshRenderers.Length; i++)
        {
            for (int j = 0; j < meshRenderers[i].materials.Length; j++)
            {
                meshRenderers[i].materials[j].renderQueue = renderQueue;
            }
        }
    }
}
