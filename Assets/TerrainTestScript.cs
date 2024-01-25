using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.TerrainUtils;
using Unity.Mathematics;
using System;
using Unity;
using System.Linq;

public class TerrainTestScript : MonoBehaviour
{
    public Terrain terrain; // Reference to your terrain object

    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain reference not set. Please assign a terrain in the inspector.");
            return;
        }

        // Modify the terrain
        ModifyTerrain();
    }

    // Get terrain heightmap dimensions
    public int heightmapWidth;
    public int heightmapHeight;

    // Get current heightmap data
    public float[,] heights;

    // Modify heights (for example, raise the center of the terrain)
    public int centerX;
    public int centerY;
    public int size;


    public bool onUpdate;

    public float thresholdTest;
    public void Update()
    {
        if (onUpdate)
        {
            ModifyTerrain();
        }
    }

    [ContextMenu("DetailMapCutoff()")]
    public void TriggerMap()
    {
        DetailMapCutoff(terrain, thresholdTest);
    }
    void DetailMapCutoff(Terrain t, float threshold)
    {
        // Get all of layer zero.
        var map = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth, t.terrainData.detailHeight, 0);

        //print("terrainData.terrainLayers).Count = " + t.terrainData.terrainLayers.Count());
        ////print(t.terrainData.terrainLayers[0].);
        //print("map[0, 0] = " + map[0, 0]);
        //print("map[513, 0] = " + map[513, 0]);


        // For each pixel in the detail map...
        for (int y = 0; y < t.terrainData.detailHeight; y++)
        {
            for (int x = 0; x < t.terrainData.detailWidth; x++)
            {
                map[x, y] = 1;
                // If the pixel value is below the threshold then
                // set it to zero.
                //if (map[x, y] < threshold)
                //{
                //    map[x, y] = 0;
                //}
            }
        }
        SetRandomAlphaMap();
        // Assign the modified map back.
        //t.terrainData.SetDetailLayer(0, 0, 0, map);
    }

    void SetRandomAlphaMap()
    {
        TerrainData terrainData = terrain.terrainData;
        int alphaMapResolution = terrainData.alphamapResolution;
        float[,,] alphaMap = new float[alphaMapResolution, alphaMapResolution, terrainData.terrainLayers.Length];

        //print(alphaMap.GetLength(0));

        // Iterate through each point on the alpha map
        for (int x = 0; x < alphaMapResolution; x++)
        {
            for (int z = 0; z < alphaMapResolution; z++)
            {
                // Randomly choose between terrainLayer[0] and terrainLayer[1]
                float randomValue = UnityEngine.Random.Range(0f, 1f);
                alphaMap[x, z, 0] = randomValue; // Value for terrainLayer[0]
                alphaMap[x, z, 1] = 1- randomValue; // Value for terrainLayer[1]
            }
        }

        // Apply the alpha map to the terrain
        terrainData.SetAlphamaps(0, 0, alphaMap);
    }


    [ContextMenu("TerrainNoiseTexturing()")]
    void TerrainNoiseTexturing()
    {
        TerrainData terrainData = terrain.terrainData;

        //print("terrainData.terrainLayers).Count = " + terrainData.terrainLayers.Count());

        //terrainData.terrainLayers[0].

        ////print("terrainData.GetDetailLayer(0, 0, 0, 0, 0)[0,0]).Count = " + terrainData.GetDetailLayer(0, 0, 0, 0, 0)[0,0]);
        //print("terrainData.GetDetailLayer(0, 0, 0, 0, 0)[1,1]).Count = " + terrainData.GetDetailLayer(0, 0, 0, 0, 0)[1,1]);

        terrainData.SetDetailLayer(0, 0, 0, new int[0, 0]);
        //terrainData.GetDetailLayer();
    }
    void ModifyTerrain()
    {
        //// Get terrain data
        //TerrainData terrainData = terrain.terrainData;

        //// Get terrain heightmap dimensions
        //heightmapWidth = terrainData.heightmapResolution;
        //heightmapHeight = terrainData.heightmapResolution;

        //// Get current heightmap data
        //heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);




        //for (int i = 0; i < heightmapWidth; i++)
        //{
        //    for (int j = 0; j < heightmapHeight; j++)
        //    {
        //        // Ensure we're within bounds
        //        if (!(i >= 0 && i < heightmapWidth && j >= 0 && j < heightmapHeight))
        //        {
        //            //print("OOB i = " + i + ", j = " + j);
        //            continue;
        //        }
        //        // Modify the height (you can adjust this value)
        //        heights[i, j] = 0
        //            + UnityEngine.Random.Range(-0.01f, 0.01f);
        //    }
        //}





        //// Apply modified heights to the terrain
        //terrainData.SetHeights(0, 0, heights);
    }
}
