using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class DebugMapValues : MonoBehaviour
{
    public MapManager mapManager;
    public Terrain terrain;
    // Start is called before the first frame update
    void Start()
    {
        DrawDistanceSlider.value = terrain.detailObjectDistance;
        Grass1DensitySlider.value = terrain.terrainData.detailPrototypes[0].density;
        Grass2DensitySlider.value = terrain.terrainData.detailPrototypes[1].density;

        detailResolutionPerPatchSlider.value = terrain.terrainData.detailResolutionPerPatch;
        detailResolutionSlider.value = 1024;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Slider DrawDistanceSlider;
    public Slider Grass1DensitySlider;
    public Slider Grass2DensitySlider;

    public void OnChangeDrawDistance()
    {
        terrain.detailObjectDistance = DrawDistanceSlider.value;
        terrain.treeDistance = DrawDistanceSlider.value;
    }

    public void OnChangeGrass1Densitiy()
    {
        terrain.terrainData.detailPrototypes[0].density = Grass1DensitySlider.value;
    }

    public void OnChangeGrass2Densitiy()
    {
        terrain.terrainData.detailPrototypes[1].density = Grass2DensitySlider.value;
    }

    public void ChangeGrassCastShadows()
    {
        if (terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().shadowCastingMode == UnityEngine.Rendering.ShadowCastingMode.On)
            terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        else
            terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
    }
    public void ChangeGrassRecieveShadows()
    {
        if (terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().receiveShadows == true)
            terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().receiveShadows = false;
        else
            terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().receiveShadows = true;

    }

    public Material originalMat, transMat;
    public Material originalMat2, transMat2;

    public void ChangeGrassSetTransparent()
    {
        Material targetMaterial = terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().sharedMaterial;

        if (targetMaterial == originalMat)
        {
            terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().sharedMaterial = transMat;
            terrain.terrainData.detailPrototypes[1].prototype.GetComponent<MeshRenderer>().sharedMaterial = transMat2;
        }
        else
        {
            terrain.terrainData.detailPrototypes[0].prototype.GetComponent<MeshRenderer>().sharedMaterial = originalMat;
            terrain.terrainData.detailPrototypes[1].prototype.GetComponent<MeshRenderer>().sharedMaterial = originalMat2;
        }


    }


    public Slider detailResolutionSlider;
    public Slider detailResolutionPerPatchSlider;

    public void OnChangeDetailResolution()
    {
        terrain.terrainData.SetDetailResolution((int)detailResolutionSlider.value, (int)detailResolutionPerPatchSlider.value);

    }
}
