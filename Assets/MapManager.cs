using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System;

using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.TerrainTools;
using UnityEngine.TerrainUtils;
using XizukiMethods.Variable;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;
using Newtonsoft.Json.Linq;
using JetBrains.Annotations;
using static UnityEngine.Rendering.DebugUI;
using System.Diagnostics;


#region Enums
public enum Civilization { Wild, Rural, Suburbs, Urban}
public enum WaterBody { Moving, Still, None }
public enum RoadPattern { Circular, Sharp, Random }
public enum NoiseType { Perlin, c, s, sr }
public enum TerrainDetailNoiseType { Normal, Inverse, Ignore }
#endregion

#region Struct (NoiseData)

[Serializable]
public struct NoiseData
{
    public float scale;

    public Texture2D texture;

    public float[,] heights;

    public float importance;

    public float contrast;

    //public float brightnessOffset;

    public int trimDir;

    public float trimCutOffPoint;

    public NoiseType noiseType;
}

#endregion

#region Struct ( Terrain Detail Prefab )
[Serializable]
public struct TerrainDetailPrefab
{
    public int index;
    public string id;
    public GameObject[] GameObject;
    public int maxAmount;
    public Vector2 widthRange;
    public Vector2 heightRange;
    public float detailDensity;
    public TerrainDetailNoiseType inverse;
}
#endregion

#region Struct (Texture Terrain) 
public enum TerrainTextureType { Flora, Dirt }

[Serializable]
public struct TerrainTexture
{
    public string name;
    public int index;
    public Texture2D texture;
    public TerrainTextureType type;
}
#endregion

#region Struct ( Terrain Object ) 

// Example, Trees and Bushes 

[Serializable]
public struct TerrainObject
{
    public int index;
    public int amount;
    public int valueCost;
    public string id;
    public GameObject[] GameObject;
    public Vector2 widthRange;
    public Vector2 heightRange;
    public Vector2 bufferPadding;
    public float detailDensity;
    public Texture2D texture;
}

#endregion


public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    public bool onUpdate;
    public CarScript car;

    [Header("Terrain Variables")]
    public NoiseData[] TerrainNoiseDatas;
    public NoiseData[] TextureNoiseDatas;

    public Vector3 size;

    public Mesh groundPlaneMesh;

    public RoadPattern roadPattern;

    public int landChunksMin;
    public int landChunksMax;

    [Header("Dirt Texture Variables")]
    [SerializeField]
    public TerrainTexture[] groundTerrainTextures;
    public AnimationCurve[] groundTextureCurveDistribution;

    [Header("Water Variables")]
    [TooltipAttribute("Type of Water Body")]
    public WaterBody waterBody;
    [TooltipAttribute("What Height Value means underwater")]
    public float waterLevel;
    [TooltipAttribute("% Amount of Total Area Underwater")]
    public float waterAmount;











    [Header("Rain Variables")]

    [TooltipAttribute("")]
    public float rainAmount;


    [TooltipAttribute("")]
    public float cloudAmount;
    public float cloudClumping;


    public float windLevel;
    public Vector2 windDirection;


    public Civilization civilization;
    public float civilizationRange;
    public float civilizationAmount;

    public float rainLevel;

    // Height and Water Map
    public Texture2D terrainMap;
    public Texture2D rainMap;
    public Texture2D structureMap;

    public SplineContainer roadSplineContainer;
    //public Spline roadSpline;
    public SplineExtrude splineExtrude;
    // Start is called before the first frame update



    [Header("GameObjects")]
    public Terrain terrain;

    [Header("Prefabs")]
    public GameObject WaterGO;
    public GameObject WaterPrefab1;
    public GameObject WaterPrefab2;
    public GameObject WaterPrefab3;

    public GameObject tunnelEntranceGO;
    public GameObject tunnelExitGO;
    public GameObject tunnelEntrancePrefab;
    public GameObject tunnelExitPrefab;


    public void OnValidate()
    {
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<MapManager>(ref Instance, this);
    }

    void Start()
    {
        car.splineAnimate.Container = roadSplineContainer;

        roadSplineContainer.gameObject.GetComponent<SplineInstantiate>().Randomize();
        GameManager.instance.RemoveObstaclesNearPlayer();
    }

    private void Awake()
    {
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<MapManager>(ref Instance, this);

        // Measure the time taken by GenerateMap method
      

        GenerateMap();

    }

   


    [Header("Terrain Values")]

    // Get terrain heightmap dimensions
    public int heightmapWidth;
    public int heightmapHeight;

    // Get current heightmap data
    public float[,] heights;
    public int[,] heightsChunks;

    public int terrainsize;




    int ran = 0;
    float timer = 0;
    public void Update()
    {
        if (onUpdate && timer %4 <= Time.deltaTime)
            GenerateGroundTerrain_PerlinNoise();

        timer += Time.deltaTime;
        //if(ran < 2)
        //    roadSplineContainer.gameObject.GetComponent<SplineInstantiate>().Randomize();
        //else
        //    GameManager.instance.RemoveObstaclesNearPlayer();

        ran++;
    }

    public int[] entrancePosition;
    public int[] exitPosition;



    [ContextMenu("GenerateMapAsync")]
    public void GenerateMapAsync2()
    {
        StartCoroutine(GenerateMapAsync());
    }

    IEnumerator GenerateMapAsync()
    {
        yield return StartCoroutine(test());
    }

    IEnumerator test()
    {
        return null;
    }


    [ContextMenu("Generate Map")]
    public void GenerateMap()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();


        UnityEngine.Random.InitState(System.DateTime.Now.Millisecond);

        terrain.detailObjectDistance = floraDrawDistance;


        InitalizeTerrain();
        ResetTerrain();



        //GenerateRoadPoints();

        GenerateGroundTerrain_PerlinNoise();

        //GenerateWater();

        ScaleRoadSplineToTerrain();
        ProjectRoadOntoTerrain();


        GenerateFloraMap();
        GenerateCivilizationNoise();
        //////CivilizationHotSpots();
        AddFloraDetailsToTerrain();
        GenerateGrass();
        AddFloraObjectToTerrain();
        GenerateFloraObjects();


        AddCivilizationObjectsToTerrain();
        CivilizationTest();

        AddTerrianTextureLayer();
        TerrainTexturing();

        //GenerateEntranceExit();
        //GenerateRoad();
        //GenerateFlora();
        //GenerateStructures();
        //GenerateClouds();
        //GenerateRain();






        stopwatch.Stop();
        print($"GenerateMap() took {stopwatch.Elapsed.TotalSeconds} seconds to run.");
    }




    [ContextMenu("RoadTerrainTest")]
    public void RoadTerrainTest()
    {
        TerrainData t = terrain.terrainData;


        float scale = (float)t.detailResolution /(float)roadOverlayScale/ (float)t.heightmapResolution;

        for (int x = 0; x < roadOverlayingHeightsArray.GetLength(0); x++)
        {
            for (int y = 0; y < roadOverlayingHeightsArray.GetLength(1); y++)
            {
                int scaledX = (int)(x * scale);
                int scaledY = (int)(y * scale);



                // Average Out Nearby Heights

                float roadHeight = roadOverlayingHeightsArray[scaledX, scaledY];

                roadOverlayingHeightsArray[scaledX, scaledY] = roadHeight;

            }
        }

        t.SetHeights(0, 0, heights);
    }








    #region Contrast Script

    [ContextMenu("Adjust Contrast")]
    public void AdjustContrast()
    {
        for (int i = 0; i < TerrainNoiseDatas.Length; i++)
        {
            TerrainNoiseDatas[i].texture = XizukiMethods.Textures.Xi_Helper_Texture.AdjustContrast(TerrainNoiseDatas[i].texture, TerrainNoiseDatas[i].contrast);
            TerrainNoiseDatas[i].heights = ConvertTextureToFloatArray(TerrainNoiseDatas[i].texture);
        }

        heights = new float[heightmapWidth, heightmapHeight];


        for (int i = 0; i < TerrainNoiseDatas.Length; i++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                for (int x = 0; x < heights.GetLength(0); x++)
                {
                    heights[x, y] += TerrainNoiseDatas[i].heights[x, y] * TerrainNoiseDatas[i].importance;
                }
            }
        }

        terrain.terrainData.SetHeights(0, 0, heights);
    }

    #endregion


    public void InitalizeTerrain()
    {
        terrain.terrainData.size = size;
    }



    #region Road Projection To Terrain Scripts
    [Header("Road Variables")]
    public GameObject roadSplinePrefabGO;
    public float roadSplineTerrainProjectionResolution;
    public float roadPadding;
    public float roadPrefabOriginalScale;
    public Spline projectedRoadSpline;
    public Spline referenceRoadSpline;

    public List<List<Vector2>> roadOverlayingHeights;
    public float roadOverlayTerrainPadding;

    public int[,] roadOverlayingHeightsArray;
    public float roadOverlayScale;
    public Texture2D roadOverlayTexture;

    public Vector3[] splineResolutionPositions;



    public void SetRoadOverlayHeights()
    {
        roadOverlayingHeightsArray = new int[(int)(terrain.terrainData.detailResolution/ roadOverlayScale), (int)(terrain.terrainData.detailResolution / roadOverlayScale)];
    }

    public void ScaleRoadSplineToTerrain()
    {
        //if(roadSplineContainer.Splines.Count>=1)
        //    roadSplineContainer.RemoveSplineAt(0);

        //roadSplineContainer.Spline = null;
        //roadSplineContainer.AddSpline(new Spline());

        referenceRoadSpline = new Spline();
        referenceRoadSpline.Copy(roadSplinePrefabGO.GetComponent<SplineContainer>().Spline);

        List<BezierKnot> knots = referenceRoadSpline.Knots.ToList();


        for (int i = 0; i < referenceRoadSpline.Count; i++)
        {
            BezierKnot scaledKnot = knots[i];
            scaledKnot.Position *= size.x / roadPrefabOriginalScale;
            knots[i] = scaledKnot;

            referenceRoadSpline.SetKnot(i, scaledKnot, BezierTangent.In);
        }
        //referenceRoadSpline.SetTangentMode(TangentMode.AutoSmooth);

        //referenceRoadSpline. = knots;
    }


    [ContextMenu("GetCurvature")]
    public void GetCurvature()
    {
        //BezierKnot test = roadSplineContainer.Spline.Evaluate(1,Vector3.zero,Vector3.zero,Vector3.zero);
        //for (int i = 0; i < roadSplineContainer.Spline.Count; i++)
        //{
        //    print(roadSplineContainer.Spline.Evaluate(i / roadSplineContainer.Spline.Count)); ;
        //}
    }

    public float roadOverlayResolution;

    public void ProjectRoadOntoTerrain()
    {
        roadSplineContainer.Spline = new Spline();

        //roadSplineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);

        projectedRoadSpline = new Spline();

        roadOverlayingHeightsArray = new int[(int)(terrain.terrainData.detailResolution / roadOverlayScale), (int)(terrain.terrainData.detailResolution / roadOverlayScale)];



        for (int i = 0; i < roadSplineTerrainProjectionResolution-1; i++)
        {
            float3 splinePos = Vector3.zero;
            float3 splineTanget = Vector3.zero;
            float3 splineUpVector = Vector3.zero;


            float t1 = (float)i / roadSplineTerrainProjectionResolution;
            float t2 =((float)i+1) / roadSplineTerrainProjectionResolution;


            for (int j = 0; j < roadOverlayResolution; j++)
            {
                float tDiff = t2 - t1;
                float division = j/roadOverlayResolution;
                referenceRoadSpline.Evaluate((t1+ (tDiff* division)), out splinePos, out splineTanget, out splineUpVector);

                // >>>>>>>> Set roadOverlayingHeightsArray Values as 1 or 0 if its overlaying

                #region Assign Values to Array

                float detailToSizeScale = (float)terrain.terrainData.detailResolution / (float)terrain.terrainData.size.x / roadOverlayScale;


                int xValue = Mathf.RoundToInt(splinePos.x * detailToSizeScale);
                int yValue = Mathf.RoundToInt(splinePos.z * detailToSizeScale);



                if (xValue - 1 >= 0)
                {
                    if (yValue - 1 >= 0)
                        roadOverlayingHeightsArray[xValue - 1, yValue - 1] = 1;

                    roadOverlayingHeightsArray[xValue - 1, yValue] = 1;

                    if (yValue - 1 >= roadOverlayingHeightsArray.GetLength(1))
                        roadOverlayingHeightsArray[xValue - 1, yValue + 1] = 1;
                }

                {
                    if (yValue - 1 >= 0)
                        roadOverlayingHeightsArray[xValue, yValue - 1] = 1;

                    roadOverlayingHeightsArray[xValue, yValue] = 1;

                    if (yValue - 1 >= roadOverlayingHeightsArray.GetLength(1))
                        roadOverlayingHeightsArray[xValue, yValue + 1] = 1;
                }

                if (xValue + 1 < roadOverlayingHeightsArray.GetLength(0))
                {
                    if (yValue - 1 >= 0)
                        roadOverlayingHeightsArray[xValue + 1, yValue - 1] = 1;

                    roadOverlayingHeightsArray[xValue + 1, yValue] = 1;

                    if (yValue - 1 >= roadOverlayingHeightsArray.GetLength(1))
                        roadOverlayingHeightsArray[xValue + 1, yValue + 1] = 1;
                }
            }



            #endregion


            Vector3 pos = new Vector3(splinePos.x, terrain.SampleHeight(splinePos), splinePos.z);
            BezierKnot bezierKnot = new BezierKnot(pos, Vector3.zero, Vector3.zero, quaternion.Euler(splineUpVector));


            projectedRoadSpline.Add(bezierKnot);
            projectedRoadSpline.SetKnot(i, bezierKnot, BezierTangent.Out);

            //roadSplineContainer.GetComponent<SplineInstantiate>().InstantiateMethod 
        }

        //projectedRoadSpline.Knots = ;

        roadSplineContainer.Spline = projectedRoadSpline;
        roadSplineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);



        roadOverlayTexture = ConvertIntArrayToTexture(roadOverlayingHeightsArray);



        #region Debug Method for Texture Visual
        Texture2D ConvertIntArrayToTexture(int[,] intArray)
        {
            int width = intArray.GetLength(0);
            int height = intArray.GetLength(1);

            // Create a new Texture2D
            Texture2D outputTexture = new Texture2D(width, height);

            // Iterate through each element in the array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    // Get the color value from the integer array
                    int colorValue = intArray[x, y];

                    // Convert the color value to a Color object (you can customize this based on your color representation)
                    Color color = new Color(colorValue, colorValue, colorValue); 

                    // Set the pixel color in the Texture2D
                    outputTexture.SetPixel(x, y, color);
                }
            }

            // Apply changes to the texture
            outputTexture.Apply();

            return outputTexture;
            // Assign the texture to a material or UI image as needed
            // Example: GetComponent<Renderer>().material.mainTexture = outputTexture;
        }
        #endregion
    }

    #endregion


    #region Reset Terrain

    public void ResetTerrain()
    {
        // Get terrain data
        TerrainData terrainData = terrain.terrainData;

        // Get terrain heightmap dimensions
        heightmapWidth = terrainData.heightmapResolution;
        heightmapHeight = terrainData.heightmapResolution;

        // Get current heightmap data
        heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);


        heightsChunks = new int[heights.GetLength(0), heights.GetLength(1)];

        for (int i = 0; i < heightmapWidth; i++)
        {
            for (int j = 0; j < heightmapHeight; j++)
            {

                // Modify the height (you can adjust this value)
                heights[i, j] = 0;
                heightsChunks[i, j] = -1;
            }
        }


        // Apply modified heights to the terrain
        terrainData.SetHeights(0, 0, heights);
    }

    #endregion


    #region Terrain PerlinNoise


    //SCALE Scale properly in accordance to heightmap 16x16 to 4096x4096 2d array
    // USE DOTS or GPU Compute to make run faster


    public void GenerateGroundTerrain_PerlinNoise()
    {
        // Use a random seed each time the script runs

        //UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
        int randX = UnityEngine.Random.Range(0, 10000);
        int randY = UnityEngine.Random.Range(0, 10000);

        TerrainData terrainData = terrain.terrainData;


        for (int i = 0; i < TerrainNoiseDatas.Length; i++)
        {
            // Create a new texture
            //noiseDatas[i].texture = new Texture2D(heightmapWidth, heightmapHeight, GraphicsFormat.R16_UNorm, TextureCreationFlags.None);
            TerrainNoiseDatas[i].texture = new Texture2D(heightmapWidth, heightmapHeight);

            TerrainNoiseDatas[i].heights = new float[heightmapWidth, heightmapHeight];

            //noiseDatas[i].texture = new Texture2D(heightmapWidth, heightmapHeight, TextureFormat.ARGB32, false);


            TerrainNoiseDatas[i].heights = GetPerlinNoise(TerrainNoiseDatas[i].heights, randX, randY, TerrainNoiseDatas[i].scale, TerrainNoiseDatas[i].noiseType, ref TerrainNoiseDatas[i].texture);


            // Apply the changes to the texture
            //TerrainNoiseDatas[i].texture.Apply();

            TerrainNoiseDatas[i].texture = XizukiMethods.Textures.Xi_Helper_Texture.AdjustContrast(TerrainNoiseDatas[i].texture, TerrainNoiseDatas[i].contrast);

            //noiseDatas[i].heights = (ConvertTextureToFloatArray(noiseDatas[i].texture));


            //print(" noise " + i + " Min Value = " + TerrainNoiseDatas[i].heights.Cast<float>().Min());
            //print(" noise " + i + " Max Value = " + TerrainNoiseDatas[i].heights.Cast<float>().Max());


            //eights = noiseDatas[i].heights;
        }


        heights = new float[heightmapWidth, heightmapHeight];


        for (int i = 0; i < TerrainNoiseDatas.Length; i++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                for (int x = 0; x < heights.GetLength(0); x++)
                {
                    //heights[x, y] = increment;
                    heights[x, y] += TerrainNoiseDatas[i].heights[x, y] * TerrainNoiseDatas[i].importance;
                }
            }
        }


        terrainData.SetHeights(0, 0, heights);
    }






    #endregion


    #region Perlin Noise Scripts

    public float[,] GetLayeredPerlinNoise(float[,] arr, float xSeed, float ySeed, float scale, NoiseType noiseType, ref Texture2D texture)
    {
        float[,] result = new float[arr.GetLength(0), arr.GetLength(1)];

        return result;
    }


    public float[,] GetPerlinNoise(float[,] arr, float xSeed, float ySeed, float scale, NoiseType noiseType, ref Texture2D texture)
    {
        float[,] result = new float[arr.GetLength(0), arr.GetLength(1)];

        // Loop through each pixel in the texture
        for (int x = 0; x < arr.GetLength(1); x++)
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                // Introduce randomness to the coordinates
                float xCoord = (float)x / scale;
                float yCoord = (float)y / scale;


                float noisePixelValue = 0;

                switch (noiseType)
                {
                    case NoiseType.Perlin:
                        noisePixelValue = UnityEngine.Mathf.PerlinNoise(xCoord + xSeed, yCoord + ySeed);
                        break;
                    case NoiseType.c:
                        noisePixelValue = noise.cnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.s:
                        noisePixelValue = noise.snoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.sr:
                        noisePixelValue = noise.srnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                }

                if (noisePixelValue > 1)
                    noisePixelValue = 1;
                else if (noisePixelValue < 0)
                    noisePixelValue = 0;

                //noisePixelValue = NoiseTrim(noisePixelValue, noiseDatas[i].trimDir, noiseDatas[i].trimCutOffPoint);


                // Set the pixel color based on the Perlin noise value
                //noiseDatas[i].texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));
                texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));
                result[x, y] = noisePixelValue;
            }
        }

        texture.Apply();

        return result;
    }

    public float[,] GetPerlinNoise(float[,] arr, float xSeed, float ySeed, float scale, NoiseType noiseType, ref Texture2D texture, NoiseInteract noiseInteract)
    {
        float[,] result = new float[arr.GetLength(0), arr.GetLength(1)];

        // Loop through each pixel in the texture
        for (int x = 0; x < arr.GetLength(1); x++)
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                // Introduce randomness to the coordinates
                float xCoord = (float)x / scale;
                float yCoord = (float)y / scale;


                float noisePixelValue = 0;

                switch (noiseType)
                {
                    case NoiseType.Perlin:
                        noisePixelValue = UnityEngine.Mathf.PerlinNoise(xCoord + xSeed, yCoord + ySeed);
                        break;
                    case NoiseType.c:
                        noisePixelValue = noise.cnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.s:
                        noisePixelValue = noise.snoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.sr:
                        noisePixelValue = noise.srnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                }

                if (noisePixelValue > 1)
                    noisePixelValue = 1;
                else if (noisePixelValue < 0)
                    noisePixelValue = 0;

                //noisePixelValue = NoiseTrim(noisePixelValue, noiseDatas[i].trimDir, noiseDatas[i].trimCutOffPoint);

                noiseInteract(noisePixelValue);

                // Set the pixel color based on the Perlin noise value
                //noiseDatas[i].texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));
                texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));
                result[x, y] = noisePixelValue;
            }
        }

        texture.Apply();

        return result;
    }

    public delegate void NoiseInteract(float noiseValue);

    public delegate float NoiseCheck(int x, int y, float pixelNoiseValue);

    public delegate float NoiseCompare(int x, int y, float pixelNoiseValue, float[,] comparedNoise);

    public NoiseCompare noiseCompareDelegate;
    public NoiseCheck noiseCheckDelegate;



  

    public float[,] GetPerlinNoise_Compare(float[,] arr, float xSeed, float ySeed, float scale,
    NoiseType noiseType, ref Texture2D texture, float[,] comparedNoise, NoiseCompare noiseCompare)
    {
        float[,] result = new float[arr.GetLength(0), arr.GetLength(1)];

        // Loop through each pixel in the texture
        for (int x = 0; x < arr.GetLength(1); x++)
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                // Introduce randomness to the coordinates
                float xCoord = (float)x / scale;
                float yCoord = (float)y / scale;


                float noisePixelValue = 0;

                switch (noiseType)
                {
                    case NoiseType.Perlin:
                        noisePixelValue = UnityEngine.Mathf.PerlinNoise(xCoord + xSeed, yCoord + ySeed);
                        break;
                    case NoiseType.c:
                        noisePixelValue = noise.cnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.s:
                        noisePixelValue = noise.snoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.sr:
                        noisePixelValue = noise.srnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                }

                if (noisePixelValue > 1)
                    noisePixelValue = 1;
                else if (noisePixelValue < 0)
                    noisePixelValue = 0;


                noisePixelValue = noiseCompare(x, y, noisePixelValue, comparedNoise);


                //noisePixelValue = NoiseTrim(noisePixelValue, noiseDatas[i].trimDir, noiseDatas[i].trimCutOffPoint);


                // Set the pixel color based on the Perlin noise value
                //noiseDatas[i].texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));
                texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));
                result[x, y] = noisePixelValue;
            }
        }

        texture.Apply();

        return result;
    }


    public float[,] GetPerlinNoise(float[,] arr, float xSeed, float ySeed, float scale, NoiseType noiseType)
    {
        float[,] result = new float[arr.GetLength(0), arr.GetLength(1)];

        // Loop through each pixel in the texture
        for (int x = 0; x < arr.GetLength(1); x++)
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                // Introduce randomness to the coordinates
                float xCoord = (float)x / scale;
                float yCoord = (float)y / scale;


                float noisePixelValue = 0;

                switch (noiseType)
                {
                    case NoiseType.Perlin:
                        noisePixelValue = UnityEngine.Mathf.PerlinNoise(xCoord + xSeed, yCoord + ySeed);
                        break;
                    case NoiseType.c:
                        noisePixelValue = noise.cnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.s:
                        noisePixelValue = noise.snoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                    case NoiseType.sr:
                        noisePixelValue = noise.srnoise(new float2(xCoord + xSeed, yCoord + ySeed));
                        break;
                }


                //noisePixelValue = NoiseTrim(noisePixelValue, noiseDatas[i].trimDir, noiseDatas[i].trimCutOffPoint);
                if (noisePixelValue > 1)
                    noisePixelValue = 1;
                else if (noisePixelValue < 0)
                    noisePixelValue = 0;

                // Set the pixel color based on the Perlin noise value
                //noiseDatas[i].texture.SetPixel(x, y, new Color(noisePixelValue, noisePixelValue, noisePixelValue));

                result[x, y] = noisePixelValue;
            }
        }

        return result;
    }

    #endregion


    #region Terrain Noise Trim WIP!!
    public float NoiseTrim(float baseValue, float trimDir, float trimCutOff)
    {
        if (trimDir == 0) return baseValue;

        else if (trimDir >= 1)
        {
            if (baseValue > trimCutOff) return baseValue;
            else return 0;
        }
        else if (trimDir <= -1)
        {
            if (baseValue < trimCutOff) return baseValue;
            else return 0;
        }

        return baseValue;
    }
    #endregion


    #region Terrain Texture Scripts



    public void AddTerrianTextureLayer()
    {
        TerrainData t = terrain.terrainData;

        List<TerrainLayer> terrainLayers = t.terrainLayers.ToList();

        for (int i = 0; i < groundTerrainTextures.Length; i++)
        {
            #region Same Texture Edge Case Checking 
            bool sameTextureCheck = false;
            int layerIndex = 0;
            foreach (TerrainLayer layer in terrainLayers)
            {
                if (sameTextureCheck) continue;
                ////print(groundTerrainTextures[i].texture == null);
                ////print(layer.diffuseTexture == null);

                // IF BELOW HAS ERROR, 
                // MAKE SURE YOU REFRESH THE UI IN PAINT TEXTURES TAB UI ON THE TERRAIN 
                if (layer.diffuseTexture == groundTerrainTextures[i].texture)
                {
                    groundTerrainTextures[i].index = layerIndex;
                    sameTextureCheck = true;
                    continue;
                }
                layerIndex++;
            }
            if (sameTextureCheck) continue;
            #endregion

            TerrainLayer newLayer = new TerrainLayer();

            newLayer.diffuseTexture = groundTerrainTextures[i].texture;
            //newLayer.diffuseRemapMax = Color.red;

            groundTerrainTextures[i].index = terrainLayers.Count;

            terrainLayers.Add(newLayer);
        }

        for (int i = 0; i < floraTerrainTextures.Length; i++)
        {

            #region Same Texture Edge Case Checking 
            bool sameTextureCheck = false;
            int layerIndex = 0;

            TerrainLayer newLayer = new TerrainLayer();

            foreach (TerrainLayer layer in terrainLayers)
            {
                if (sameTextureCheck) continue;
                if (layer.diffuseTexture == floraTerrainTextures[i].texture)
                {
                    floraTerrainTextures[i].index = layerIndex;

                    if (useFloraColor4Textures[i])
                        newLayer.diffuseRemapMax = floraColor;
                    else
                        newLayer.diffuseRemapMax = Color.white;

                    sameTextureCheck = true;
                    continue;
                }
                layerIndex++;
            }
            if (sameTextureCheck) continue;
            #endregion


            newLayer.diffuseTexture = floraTerrainTextures[i].texture;

            if (useFloraColor4Textures[i])
                newLayer.diffuseRemapMax = floraColor;
            else
                newLayer.diffuseRemapMax = Color.white;


            floraTerrainTextures[i].index = terrainLayers.Count;

            terrainLayers.Add(newLayer);
        }


        t.terrainLayers = terrainLayers.ToArray();
    }


    public float AdjustValueToCurve(float value, AnimationCurve curve, bool capped)
    {
        float result = value * curve.Evaluate(value);

        if (capped)
        {
            if (result < 0) result = 0;
            if (result > 1) result = 1;
        }
        return result;
    }

    public float[] NormalizeArrayValues(float[] values)
    {
        float[] result = new float[values.Length];
        float totalValue = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] < 0) { values[i] = 0; }

            totalValue += values[i];
        }

        for (int i = 0; i < values.Length; i++)
        {
            if (values[i] < 0) { values[i] = 0; }

            result[i] = values[i] / totalValue;
        }

        // Bandaid Fix for Black Holes on Terrain
        if (totalValue <= 0)
        {
            result[0] = 1;
        }
        return result;
    }


    public void TerrainTexturing()
    {

        TerrainData terrainData = terrain.terrainData;
        int alphaMapResolution = terrainData.alphamapResolution;

        float[,] map = new float[alphaMapResolution, alphaMapResolution];
        float[,,] alphaMap = new float[alphaMapResolution, alphaMapResolution, terrainData.terrainLayers.Length];


        int randX = UnityEngine.Random.Range(0, 10000);
        int randZ = UnityEngine.Random.Range(0, 10000);

        // IDK WHY I NEED TO +1 HERE, also has a bug where it wont iterate on the last value;
        float terrainAlphaScale = (alphaMapResolution / terrain.terrainData.heightmapResolution) + 1;

        float dirtRatio = 1;

        float[] floraValues = new float[floraTerrainTextures.Length];
        float[] dirtValues = new float[groundTerrainTextures.Length];


        int[,] detailMap = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, 0);


        // Iterate through each point on the alpha map
        for (int x = 0; x < alphaMapResolution; x++)
        {
            for (int z = 0; z < alphaMapResolution; z++)
            {
                for (int i = 0; i < floraTerrainTextures.Length; i++)
                {
                    floraValues[i] = AdjustValueToCurve(
                         FloraNoiseData.heights[x, z]
                        , floraTextureCurveDistribution[i]
                        , true);
                }

                floraValues = NormalizeArrayValues(floraValues);

                for (int i = 0; i < floraTerrainTextures.Length; i++)
                {
                    floraValues[i] = floraValues[i] * FloraNoiseData.heights[x, z];
                }

                float totalFloraValue = 0;
                for (int i = 0; i < floraValues.Length; i++)
                {
                    alphaMap[x, z, floraTerrainTextures[i].index] = floraValues[i];
                    totalFloraValue += floraValues[i];
                }



                dirtRatio = 1 - FloraNoiseData.heights[x, z];
                float groundPercentange = 1 - totalFloraValue;




                for (int i = 0; i < groundTerrainTextures.Length; i++)
                {
                    float noiseValue = heights[Mathf.FloorToInt(x / terrainAlphaScale), Mathf.FloorToInt(z / terrainAlphaScale)];

                    dirtValues[i] = groundTextureCurveDistribution[i].Evaluate(noiseValue);
                }

                dirtValues = NormalizeArrayValues(dirtValues);

                for (int i = 0; i < dirtValues.Length; i++)
                {
                    alphaMap[x, z, groundTerrainTextures[i].index] = dirtValues[i] * groundPercentange;
                }


            }
        }


        // Apply the alpha map to the terrain
        terrainData.SetAlphamaps(0, 0, alphaMap);
        terrainData.SetDetailLayer(0, 0, 0, detailMap);
    }




    #endregion


    #region Water Scripts

    [ContextMenu("Generate Water")]
    public void GenerateWater()
    {
        // ADD IN LOGIC FOR WATER HEIGHT

        waterLevel = UnityEngine.Random.Range(0, 0.8f);
        WaterGO.transform.transform.position = new Vector3(0, waterLevel * size.y, 0);
        WaterGO.transform.transform.localScale = new Vector3(size.x, 1, size.z) * 5f;
    }

    #endregion


    #region Tunnel Generation Scripts

    [ContextMenu("Generate EntranceExit")]
    public void GenerateEntranceExit()
    {
        if (tunnelEntranceGO != null) { Destroy(tunnelEntranceGO); }
        if (tunnelExitGO != null) { Destroy(tunnelExitGO); }



        entrancePosition = new int[] {
            UnityEngine.Random.Range(0, heightmapWidth) ,
            UnityEngine.Random.Range(0, heightmapHeight)
        };

        int x = entrancePosition[0];
        int y = entrancePosition[1];

        tunnelEntranceGO = GameObject.Instantiate(tunnelEntrancePrefab,
            GetSurfacePositionOfTerrainFrom2dArray(x, y),
            Quaternion.identity);


        //print("heights[x, y] = " + heights[x, y]);


        exitPosition = new int[] {
            UnityEngine.Random.Range(0, heightmapWidth) ,
            UnityEngine.Random.Range(0, heightmapHeight)
        };

        x = exitPosition[0];
        y = exitPosition[1];

        tunnelExitGO = GameObject.Instantiate(tunnelExitPrefab,
            GetSurfacePositionOfTerrainFrom2dArray(x, y),
            Quaternion.identity);

    }


    public Vector3 GetSurfacePositionOfTerrainFrom2dArray(float x, float y)
    {
        Vector3 result = Vector3.zero;


        result = new Vector3(
           (((float)x) / (float)heightmapWidth) * size.x,
           (terrain.terrainData.GetHeight((int)x,(int)y)),//(terrain.terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight)[(int)y, (int)x]) * size.y, // terrain.SampleHeight(new Vector3(x,0,y)),

           (((float)y) / (float)heightmapHeight) * size.z);


        return result;
    }

    #endregion
    

    #region OLD OLD OLD
    #region Road Generation Scripts 

    //public enum RoadShape
    //{
    //    Directional,
    //    Loop,
    //    Spiral
    //}

    //[Header("Road Variables")]
    //public List<Vector2> roadPoints;
    //public float roadPadding;
    //public float chance4RoadToGoToHotspot;
    //public float roadLength;
    //public float roadPointsDistance;
    //[Tooltip("% of outer terrain that roadpoints are not allowed on")]
    //public float terrainRoadPointPaddings;
    //public float roadAngle;
    //public float roadReverseAngleChance;
    //public Vector2 curSplineDirection;
    //public int splineGeneralAngle;
    //public RoadShape roadShape;


    //public void GenerateRoadPoints()
    //{
    //    float roadCount = 0;

    //    #region Set Range where roadpoints are allowed to spawn
    //    Vector2 xRange = new Vector2(heights.GetLength(0) * terrainRoadPointPaddings,
    //        heights.GetLength(0) - (heights.GetLength(0) * terrainRoadPointPaddings));

    //    Vector2 yRange = new Vector2(heights.GetLength(1) * terrainRoadPointPaddings,
    //        heights.GetLength(1) - (heights.GetLength(1) * terrainRoadPointPaddings));
    //    #endregion

    //    #region Spawn first RoadPoint
    //    roadPoints = new List<Vector2>();

    //    roadPoints.Add(new Vector2(
    //        UnityEngine.Random.Range(xRange.x, xRange.y),
    //        UnityEngine.Random.Range(yRange.x, yRange.y)));
    //    #endregion

    //    #region Generate Random Road Points 
    //    int index = 0;
    //    while (roadCount < roadLength)
    //    {

    //        Vector2 randomSpot = new Vector2(
    //        UnityEngine.Random.Range(xRange.x, xRange.y),
    //        UnityEngine.Random.Range(yRange.x, yRange.y));


    //        #region Generate Points


    //        Vector2 newRoadPoint = new Vector2(randomSpot.x, randomSpot.y);

    //        float roadDistance = Mathf.Abs((newRoadPoint - roadPoints[roadPoints.Count - 1]).magnitude);


    //        if (roadDistance < roadPointsDistance)
    //        {
    //            continue;
    //        }


    //        roadPoints.Add(newRoadPoint);

    //        #endregion


        
    //        roadCount += roadDistance;
    //        index++;
    //    }

    //    #endregion

    //    #region Scale and Add Civilziation HotSpots to Road Points

    //    // Set Civilization HotSpot as Road Points
    //    for (int i = 0; i < hotSpots.Count; i++)
    //    {
    //        Vector2 scaledHotSpot = hotSpots[i];

    //        scaledHotSpot *= ((float)heights.GetLength(0) / (float)CivilizationNoise.heights.GetLength(0));

    //        if (scaledHotSpot.x < xRange.x || scaledHotSpot.x > xRange.y) continue;
    //        if (scaledHotSpot.y < yRange.x || scaledHotSpot.y > yRange.y) continue;

    //        roadPoints.Add(scaledHotSpot);

    //    }

    //    #endregion

    //    #region Sort Road Points To Road Shape

    //    #region Directional Road Shape
    //    if (roadShape == RoadShape.Directional)
    //    {
    //        Vector2 dir = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)) ;
    //        roadPoints.Sort(SortByX);

    //        // Comparison function to sort by x-axis values
    //        int SortByX(Vector2 v1, Vector2 v2)
    //        {
    //            return v1.x.CompareTo(v2.x);
    //        }

    //        //for(int i = 1;i < roadPoints.Count;i++)
    //        //{
    //        //    if(i%2 == 1) { continue; }

    //        //    Vector2 valueHolder = roadPoints[i-1];
    //        //    roadPoints[i - 1] = roadPoints[i];
    //        //    roadPoints[i] = valueHolder;
    //        //}
    //    }
    //    #endregion
    //    #region Spiral Road Shape
    //    else if(roadShape == RoadShape.Spiral)
    //    {



    //    }
    //    #endregion
    //    #region Loop Road Shape
    //    else if (roadShape == RoadShape.Loop)
    //    {

    //    }
    //    #endregion


    //    #endregion

    //    #region Generate Spline

    //    roadSplineContainer.Spline = new Spline();
    //    roadSplineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);

    //    for (int i = 0; i < roadPoints.Count; i++)
    //    {

    //        BezierKnot bezierKnot = new BezierKnot(GetSurfacePositionOfTerrainFrom2dArray(roadPoints[i].x, roadPoints[i].y));
    //        roadSplineContainer.Spline.Add(bezierKnot);
    //        roadSplineContainer.Spline.SetKnot(i, bezierKnot, BezierTangent.In);
       
    //    }
    //    roadSplineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);
    //    roadSplineContainer.Spline.SetTangentMode(TangentMode.Mirrored);
    //    List< BezierKnot> bezierKnots = roadSplineContainer.Spline.Knots.ToList();



    //    //for (int i = 1; i < roadSplineContainer.Spline.Knots.Count(); i++)
    //    //{
    //    //    float rotationalMagnitude = (Quaternion.ToEulerAngles( bezierKnots[i].Rotation) -
    //    //                                Quaternion.ToEulerAngles(bezierKnots[i-1].Rotation)).magnitude;

    //    //    BezierKnot bezierKnot = roadSplineContainer.Spline.Knots.ToList()[i];
    //    //    bezierKnot.TangentOut = 100 * rotationalMagnitude;
    //    //    bezierKnot.TangentIn = 100 * rotationalMagnitude;

    //    //    roadSplineContainer.Spline.SetKnot(i, bezierKnot, BezierTangent.In);

    //    //}
    //    #endregion


    //}

    //public void GenerateSplineSubSegments()
    //{
    //    Spline newSpline = new Spline();
    //    for (int i = 0; i < 1000; i++)
    //    {
    //        Vector3 splinePos = roadSplineContainer.Spline.EvaluatePosition((float)i/100);
    //        Vector2 splineV2 = new Vector2(splinePos.x, splinePos.z );

    //        //print("splineV2 = " + splineV2);
    //        //print("splineV2 v2 = " + new float3(splineV2.x, terrain.SampleHeight(new Vector3(splineV2.x / size.x, 0, splineV2.y / size.z)), splineV2.y));

    //        BezierKnot bezierKnot = new BezierKnot(
    //            new float3(splineV2.x, terrain.SampleHeight(new Vector3(splineV2.x / size.x, 0, splineV2.y / size.z)), splineV2.y));

    //        newSpline.Add(bezierKnot);
    //        newSpline.SetKnot(i, bezierKnot, BezierTangent.In);


    //    }
    //    newSpline.SetTangentMode(TangentMode.AutoSmooth);
    //    roadSplineContainer.Spline = newSpline;

    //}


    //#region old
    ////public void GenerateRoadPoints()
    ////{
    ////    float roadCount = 0;

    ////    #region Set Range where roadpoints are allowed to spawn
    ////    Vector2 xRange = new Vector2(heights.GetLength(0) * terrainRoadPointPaddings,
    ////        heights.GetLength(0) - (heights.GetLength(0) * terrainRoadPointPaddings));

    ////    Vector2 yRange = new Vector2(heights.GetLength(1) * terrainRoadPointPaddings,
    ////        heights.GetLength(1) - (heights.GetLength(1) * terrainRoadPointPaddings));
    ////    #endregion

    ////    #region Spawn first RoadPoint
    ////    roadPoints = new List<Vector2>();

    ////    roadPoints.Add(new Vector2(
    ////        UnityEngine.Random.Range(xRange.x, xRange.y),
    ////        UnityEngine.Random.Range(yRange.x, yRange.y)));
    ////    #endregion

    ////    splineGeneralAngle = UnityEngine.Random.Range(0, 2);
    ////    if (splineGeneralAngle == 0) { splineGeneralAngle = -1; }

    ////    float curX = roadPoints[0].x;
    ////    float curY = roadPoints[0].y;

    ////    roadSplineContainer.Spline = new Spline();

    ////    bool uTurn = false;


    ////    int index = 0;
    ////    while (roadCount < roadLength)
    ////    {
    ////        bool overArrayCheck = false;
    ////        Vector2 diff = Vector2.zero;


    ////        do
    ////        {
    ////            if (diff == Vector2.zero)
    ////            {
    ////                diff.x = UnityEngine.Random.Range(-1, 1);
    ////                diff.y = UnityEngine.Random.Range(-1, 1);
    ////            }


    ////            curSplineDirection = new Vector2(diff.x, diff.y);

    ////            curSplineDirection.Normalize();


    ////            if (UnityEngine.Random.Range(0, 100) < roadReverseAngleChance)
    ////            {
    ////                splineGeneralAngle *= -1;
    ////            }




    ////            // Angle in degrees
    ////            float angleInDegrees = UnityEngine.Random.Range(1, roadAngle) * splineGeneralAngle;

    ////            // Create a rotation quaternion around the Z-axis
    ////            Quaternion rotation = Quaternion.Euler(0, 0, angleInDegrees);

    ////            // Rotate the 2D vector using the quaternion
    ////            Vector2 rotatedVector = rotation * curSplineDirection;

    ////            rotatedVector.Normalize();



    ////            diff = new Vector2(rotatedVector.x, rotatedVector.y) * UnityEngine.Random.Range(1, roadPointsDistance);



    ////            if (curX + diff.x < xRange.x ||
    ////                curX + diff.x > xRange.y ||
    ////                curY + diff.y < yRange.x ||
    ////                curY + diff.y > yRange.y)
    ////            {
    ////                //overArrayCheck = true;


    ////                if (curX + diff.x < xRange.x || curX + diff.x > xRange.y)
    ////                    diff.x *= -1;
    ////                if (curY + diff.y < yRange.x || curY + diff.y > yRange.y)
    ////                    diff.y *= -1;

    ////            }
    ////            else
    ////            {
    ////                overArrayCheck = false;
    ////            }
    ////        }
    ////        while (overArrayCheck);

    ////        if (diff == Vector2.zero) continue;

    ////        curX += diff.x;
    ////        curY += diff.y;

    ////        #region Generate Spline

    ////        Vector2 newRoadPoint = new Vector2(curX, curY);

    ////        roadPoints.Add(newRoadPoint);

    ////        BezierKnot bezierKnot = new BezierKnot();
    ////        //bezierKnot.Rotation = Quaternion.Euler(0, 0, 0);
    ////        roadSplineContainer.Spline.Add(bezierKnot);
    ////        roadSplineContainer.Spline.SetKnot(index, new BezierKnot(GetSurfacePositionOfTerrainFrom2dArray(newRoadPoint.x, newRoadPoint.y)), BezierTangent.In);

    ////        #endregion

    ////        roadCount += newRoadPoint.magnitude;
    ////        index++;
    ////    }

    ////    roadSplineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);

    ////}
    //#endregion

    //public void GenerateRoad()
    //{
    //    //int[] tunnelDifference = new int[] { exitPosition[0] - entrancePosition[0], exitPosition[1] - entrancePosition[1] };

    //    int roadCount = 0;
    //    roadSplineContainer.Spline = new Spline();

    //    int y = 5;
    //    for (int x = 0; x < heights.GetLength(0); x++)
    //    {
    //        if (roadCount >= roadLength) return;

    //        //if(roadCount%10 == 0) 
    //        //    roadSplineContainer.AddSpline(new Spline());

    //        BezierKnot bezierKnot = new BezierKnot();
    //        roadSplineContainer.Spline.Add(bezierKnot);
    //        roadSplineContainer.Spline.SetKnot(roadCount, new BezierKnot(GetSurfacePositionOfTerrainFrom2dArray(x, 0.14f)), BezierTangent.In);


    //        roadCount++;
    //        //y++;
    //    }

    //    roadSplineContainer.Spline.SetTangentMode(TangentMode.AutoSmooth);

    //}

    #endregion
    #endregion


    #region Flora Scripts


    [Header("Flora Variables")]

    [SerializeField]
    public TerrainDetailPrefab[] floraTerrainDetailPrefabs;
    public AnimationCurve[] floraDetailsCurveDistribution;

    [SerializeField]
    public TerrainTexture[] floraTerrainTextures;
    public AnimationCurve[] floraTextureCurveDistribution;
    public bool[] useFloraColor4Textures;

    [SerializeField]
    public TerrainObject[] floraObjects;
    public AnimationCurve[] floraObjectsSizeCurve;
    public AnimationCurve[] floraObjectsCurveDistribution;


    public float totalFloraNoiseValue;
    public float totalFloraObjectValues;
    public float floraNoisePeakValue;
    public Color floraColor;
    [SerializeField]
    public NoiseData FloraNoiseData;
    public float floraLevel;
    public float floraRange;
    public Vector2 floraGrassHeightRange;
    public float floraDrawDistance;
    public float floraDensity;
    public int floraObjectResolutionScale;
    public int floraObjectOverlapCheckLimit;


    #region Noise Map Generation


    public void SumOfFloraNoise(float noiseValue)
    {
        totalFloraNoiseValue += noiseValue;
    }

    [ContextMenu("Generate Flora Map")]
    public void GenerateFloraMap()
    {
        TerrainData t = terrain.terrainData;
        int alphaMapResolution = t.alphamapResolution;


        //print("alphaMapResolution = " + alphaMapResolution);

        // Use a random seed each time the script runs

        int randX = UnityEngine.Random.Range(0, 10000);
        int randY = UnityEngine.Random.Range(0, 10000);


        FloraNoiseData.heights = new float[alphaMapResolution, alphaMapResolution];
        //print("FloraNoiseData.heights.GetLength(0) = " + FloraNoiseData.heights.GetLength(0));

        FloraNoiseData.texture = new Texture2D(alphaMapResolution, alphaMapResolution, TextureFormat.ARGB32, false);

        FloraNoiseData.heights = GetPerlinNoise(FloraNoiseData.heights, randX, randY, FloraNoiseData.scale, 
                                                FloraNoiseData.noiseType, ref FloraNoiseData.texture, SumOfFloraNoise);


        // Apply the changes to the texture
        //FloraNoiseData.texture.Apply();

        //FloraNoiseData.texture = XizukiMethods.Textures.Xi_Helper_Texture.AdjustContrast(FloraNoiseData.texture, FloraNoiseData.contrast);
    }
    #endregion

    public float terrainDetailToAlphaMapScale;

    #region Pre - Add Flora Details to Terrain 

    [ContextMenu("Add Flora Details to Terrain")]
    public void AddFloraDetailsToTerrain()
    {
        TerrainData t = terrain.terrainData;

        //int originalLength = t.detailPrototypes.Length;

        List<DetailPrototype> newDetails = new List<DetailPrototype>();



        //for (int i = 0; i < originalLength; i++)
        //{
        //    bool isInDetailList = false;

        //    for (int j = 0; j < floraTerrainDetailPrefabs.Length; j++)
        //    {
        //        if (floraTerrainDetailPrefabs[j].GameObject[0] == t.detailPrototypes[i].prototype)
        //        {
        //            isInDetailList = true;
        //            break;
        //        }
        //    }

        //    if(isInDetailList)
        //        newDetails.Add(t.detailPrototypes[i]);
        //}

        //print("floraTerrainDetailPrefabs.length = " + floraTerrainDetailPrefabs.Length);

        for (int i = 0; i < floraTerrainDetailPrefabs.Length; i++)
        {
            TerrainDetailPrefab detail = floraTerrainDetailPrefabs[i];
            floraTerrainDetailPrefabs[i].index = i;



            #region Same Detail Prefab Edge Case Checking

            //bool samePrefabCheck = false;
            //for (int j = 0; j < originalLength; j++)
            //{
            //    if (detail.GameObject[0] == t.detailPrototypes[j].prototype)
            //    {
            //        samePrefabCheck = true;
            //        break;
            //    }
            //}
            //if (samePrefabCheck)
            //{
            //    continue;
            //}

            #endregion



            //detail.GameObject[0].GetComponent<MeshRenderer>().material.color = floraColor;


            newDetails.Add(new DetailPrototype());

            //newDetails[i].prototypeTexture = detail.texture;
            newDetails[i].prototype = detail.GameObject[0];
            newDetails[i].usePrototypeMesh = true;
            newDetails[i].useInstancing = true;
            newDetails[i].renderMode = DetailRenderMode.VertexLit;

            newDetails[i].density = detail.detailDensity;

            newDetails[i].minHeight = detail.heightRange[0];
            newDetails[i].maxHeight = detail.heightRange[1];
            newDetails[i].minWidth = detail.widthRange[0];
            newDetails[i].maxWidth = detail.widthRange[1];

            //floraTerrainDetailPrefabs[prefabIndex];
        }

        t.detailPrototypes = newDetails.ToArray();

    }
    #endregion


    #region Generate Grass Scripts

    [ContextMenu("Generate Grass")]  
    public void GenerateGrass()
    {
        TerrainData t = terrain.terrainData;

        terrainDetailToAlphaMapScale = t.alphamapResolution / t.detailResolution;
        float terrainDetailToFloraMapScale = FloraNoiseData.heights.GetLength(0) / t.detailResolution;


        for (int i = 0; i < floraTerrainDetailPrefabs.Length; i++)
        {
            int[,] map = new int[ t.detailWidth, t.detailHeight];


            for (int x = 0; x < t.detailResolution; x++)
            {
                for (int y = 0; y < t.detailResolution; y++)
                {
                    // READ READ READ >> This ASSUMES detail map is 4x smaller than alphamap, (ex. 1024x1024 to 4096x4096)
                    #region Scale X and Y of details to Alphamap

                    int scaledX = (int)(x / terrainDetailToFloraMapScale);
                    int scaledY = (int)(y / terrainDetailToFloraMapScale);


                    ////print("scaledX , scaledY  = " + scaledX  + ",  " + scaledY );

                    float noiseValue =
                        (
                        FloraNoiseData.heights[scaledX, scaledY]
                        );

                    //noiseValue = AdjustValueToCurve(noiseValue, floraDetailsCurveDistribution[i], true);


                    if (noiseValue < 0) continue;

                    if (floraTerrainDetailPrefabs[i].inverse == TerrainDetailNoiseType.Inverse)
                    {
                        noiseValue = 1 - noiseValue;
                    }
                    else if (floraTerrainDetailPrefabs[i].inverse == TerrainDetailNoiseType.Ignore)
                    {
                        noiseValue = UnityEngine.Random.Range(0f, 1f);
                    }
                    #endregion


                    int roadScaledY = (int)((float)scaledY / roadOverlayScale);
                    int roadScaledX = (int)((float)scaledX / roadOverlayScale);


                    #region Check if Position Overlaps Road

                    bool roadOverlap = false;

                    if (roadOverlayingHeightsArray[roadScaledY, roadScaledX] == 1)
                        roadOverlap = true;

                    //int value = 0;


                    //if (roadScaledY > 0 && roadScaledX > 0)
                    //    value += roadOverlayingHeightsArray[roadScaledY - 1, roadScaledX - 1];
                    //if (roadScaledX > 0)
                    //    value += roadOverlayingHeightsArray[roadScaledY, roadScaledX - 1];
                    //if (roadScaledY < roadOverlayingHeightsArray.GetLength(0) - 1 && roadScaledX > 0)
                    //    value += roadOverlayingHeightsArray[roadScaledY + 1, roadScaledX - 1];

                    //if (roadScaledY > 0)
                    //    value += roadOverlayingHeightsArray[roadScaledY - 1, roadScaledX];
                    //value += roadOverlayingHeightsArray[roadScaledY, roadScaledX];




                    //if (roadScaledX < roadOverlayingHeightsArray.GetLength(0) - 1)
                    //{
                    //    if (roadScaledY > 0)
                    //        value += roadOverlayingHeightsArray[roadScaledY - 1, roadScaledX + 1];

                    //    value += roadOverlayingHeightsArray[roadScaledY, roadScaledX + 1];

                    //    if (roadScaledY < roadOverlayingHeightsArray.GetLength(0) - 1)
                    //        value += roadOverlayingHeightsArray[roadScaledY + 1, roadScaledX + 1];
                    //}

                    //if (roadOverlayingHeightsArray[(int)(y / roadOverlayScale), (int)(x / roadOverlayScale)] >= 1)
                    //    roadOverlap = true;

                    if (roadOverlap)
                    {
                        //print("grass overlapped");
                        civilizationObjectNoiseValue = 0;
                        noiseValue = 0;
                        continue;
                    }




                    #endregion

                    if (!roadOverlap)
                        map[x, y] = (int)(floraTerrainDetailPrefabs[(i)].maxAmount * noiseValue);


                }
            }
            t.SetDetailLayer(0, 0, floraTerrainDetailPrefabs[i].index, map);
        }
    }


  
    #endregion

    //[ContextMenu("GetClampedDetailPatches")]
    //public void GetClampedDetailPatches()
    //{
    //    //print("======================== GetClampedDetailPatches ======================== ");

    //    Vector2Int[] arr = terrain.terrainData.GetClampedDetailPatches(1);

    //    foreach(Vector2Int p in arr) 
    //    {
    //        //print("GetClampedDetailPatches = " + p);
    //    }

    //}


    #region Pre - Add Flora Objects to Terrain 


    [ContextMenu("Add FloraObject To Terrain")]
    public void AddFloraObjectToTerrain()
    {
        {
            TerrainData t = terrain.terrainData;

            t.treeInstances = new TreeInstance[0];

            int originalLength = floraObjects.Length;

            List<TreePrototype> newTreePrototypes = new List<TreePrototype>();



            for (int i = 0; i < floraObjects.Length; i++)
            {
                TerrainObject flora = floraObjects[i];
                floraObjects[i].index = i;


                //flora.GameObject[0].GetComponent<MeshRenderer>().material.color = floraColor;


                newTreePrototypes.Add(new TreePrototype());

                //newDetails[i].prototypeTexture = detail.texture;
                newTreePrototypes[i].prefab = flora.GameObject[0];
                newTreePrototypes[i].bendFactor = newTreePrototypes[i].bendFactor;

                //floraTerrainDetailPrefabs[prefabIndex];
            }
            
            t.treePrototypes = newTreePrototypes.ToArray();

        }
    }

    #endregion


    #region Helper Methods 
    public bool OverlappingCheck(Vector2 newPosition, Vector2 buffPadding, Vector2[] pastPositions, Vector2[] pastBuffPadding)
    {
        bool result = false;

        // Calculate bounds for the new position
        float newMinX = newPosition.x - buffPadding.x / 2;
        float newMaxX = newPosition.x + buffPadding.x / 2;
        float newMinY = newPosition.y - buffPadding.y / 2;
        float newMaxY = newPosition.y + buffPadding.y / 2;

        for (int i = 0; i < pastPositions.Length; i++)
        {
            // Calculate bounds for the past position
            float pastMinX = pastPositions[i].x - pastBuffPadding[i].x / 2;
            float pastMaxX = pastPositions[i].x + pastBuffPadding[i].x / 2;
            float pastMinY = pastPositions[i].y - pastBuffPadding[i].y / 2;
            float pastMaxY = pastPositions[i].y + pastBuffPadding[i].y / 2;

            // Check for overlap in the x-axis and y-axis
            bool overlapX = newMinX < pastMaxX && newMaxX > pastMinX;
            bool overlapY = newMinY < pastMaxY && newMaxY > pastMinY;

            // If there is an overlap in both x-axis and y-axis, set result to true and break the loop
            if (overlapX && overlapY)
            {
                result = true;
                //break;
            }
        }

        return result;
    }

    public float[,] CompressAndAverageArray(float[,] originalArray, int compressionFactor)
    {
        int newWidth = (int)(originalArray.GetLength(1) / compressionFactor);
        int newHeight = (int)(originalArray.GetLength(0) / compressionFactor);

        float[,] compressedArray = new float[newWidth, newHeight];

        for (int i = 0; i < newWidth; i++)
        {
            for (int j = 0; j < newHeight; j++)
            {
                float sum = 0;

                for (int x = 0; x < compressionFactor; x++)
                {
                    for (int y = 0; y < compressionFactor; y++)
                    {
                        sum += originalArray[i * compressionFactor + x, j * compressionFactor + y];
                    }
                }

                compressedArray[i, j] = sum / (compressionFactor * compressionFactor);
            }
        }

        return compressedArray;
    }
    #endregion


    #region Generate Flora Object


    [ContextMenu("Generate FloraObjects")]
    public void GenerateFloraObjects()
    {
        terrain.drawTreesAndFoliage = true;

        TerrainData t = terrain.terrainData;

        List<TreeInstance> treeInstances = new List<TreeInstance>();


        for (int x = 0; x < t.detailResolution /floraObjectResolutionScale;  x++)
        {
            for (int y = 0; y < t.detailResolution / floraObjectResolutionScale; y++)
            {
                #region Scale Flora Noise to Flora Object Resolution 

                float noiseValue = FloraNoiseData.heights[x, y];

                float sum = 0;

                for (int xx = 0; xx < floraObjectResolutionScale; xx++)
                {
                    for (int yy = 0; yy < floraObjectResolutionScale; yy++)
                    {
                        sum += FloraNoiseData.heights[x * floraObjectResolutionScale + xx, y * floraObjectResolutionScale + yy];
                    }
                }

                noiseValue = sum / (floraObjectResolutionScale * floraObjectResolutionScale);

                #endregion


                float floraObjectNoiseValue = noiseValue * floraNoisePeakValue;

                List<Vector2> previousPositions = new List<Vector2>();
                List<Vector2> previousPaddings = new List<Vector2>();

                int overlaps = 0;

                while (floraObjectNoiseValue > 0)
                {
                    #region Select Flora Object to Spawn

                    int selectedObjectIndex = -1;


                    float[] floraObjectWeights = new float[floraObjects.Length];

                    for (int i = 0; i < floraObjects.Length; i++)
                    {
                        floraObjectWeights[i] += AdjustValueToCurve(noiseValue, floraObjectsCurveDistribution[i], true);
                        if (floraObjectWeights[i] < 0) floraObjectWeights[i] = 0;
                    } 

                    floraObjectWeights = NormalizeArrayValues(floraObjectWeights);


                    float randomValue = UnityEngine.Random.Range(0f, 1f);
                    float total=0;

                 

                    for (int i = 0; i < floraObjectWeights.Length; i++)
                    {
                        total += floraObjectWeights[i];
                        if (randomValue > total-floraObjectWeights[i] && randomValue < total)
                        {
                            selectedObjectIndex = i;
                        }
                    }
                 
                    #endregion



                    float randomX = UnityEngine.Random.Range((float)x *floraObjectResolutionScale  / FloraNoiseData.heights.GetLength(0),
                                                        ((float)x + 1)* floraObjectResolutionScale  / FloraNoiseData.heights.GetLength(0));
                    float randomZ = UnityEngine.Random.Range((float)y * floraObjectResolutionScale  / FloraNoiseData.heights.GetLength(1),
                                                        ((float)y+ 1) * floraObjectResolutionScale  / FloraNoiseData.heights.GetLength(1));



                    #region Spawn Flora Object
                    if (selectedObjectIndex >= 0)
                    {

                        bool overlapCheck = OverlappingCheck(new Vector2(randomZ, randomX),
                                                            civilizationObjects[selectedObjectIndex].bufferPadding,
                                                            previousPositions.ToArray(),
                                                            previousPaddings.ToArray());


                        if (overlapCheck) { overlaps++; continue; }
                        if (overlaps >= floraObjectOverlapCheckLimit) { floraObjectNoiseValue = 0; continue; }



                        float terrainHeight = terrain.SampleHeight(new Vector3(randomZ * t.size.z, 0f, randomX * t.size.x));
                        Vector3 treePosition = new Vector3(randomZ, terrainHeight, randomX);


                        #region Check if Position Overlaps Road

                        bool roadOverlap = false;


                        Vector3 truePosition = new Vector3(randomZ * terrain.terrainData.size.z,
                           terrainHeight, randomX * terrain.terrainData.size.x);


                        for (int j = 0; j < 100; j++)
                        {
                            Vector3 roadSplinePointPosition = roadSplineContainer.Spline.EvaluatePosition((float)j / 100);


                            //if(x<10 && y<10 && j<20)
                            //{
                            //    //print("roadSplinePointPosition = " + roadSplinePointPosition);
                            //    //print("truePosition = " + truePosition);
                            //    //print("(roadSplinePointPosition - truePosition).magnitude = " + (roadSplinePointPosition - truePosition).magnitude);
                            //}

                            if ((roadSplinePointPosition - truePosition).magnitude <= roadPadding)
                            {
                                roadOverlap = true;
                                continue;
                            }

                        }

                        if (roadOverlap)
                        {
                            civilizationObjectNoiseValue = 0;
                            continue;
                        }

                        #endregion


                        float size = AdjustValueToCurve(noiseValue, floraObjectsSizeCurve[selectedObjectIndex], true);

                        if (size > 0f)
                        {
                            treeInstances.Add(new TreeInstance
                            {
                                position = treePosition,
                                rotation = UnityEngine.Random.Range(0, 360),
                                prototypeIndex = selectedObjectIndex,
                                heightScale = size,
                                widthScale = size,
                                color = Color.white
                            }); ;

                            floraObjectNoiseValue -= floraObjects[selectedObjectIndex].valueCost;

                            previousPositions.Add(new Vector2(randomZ, randomX));
                            previousPaddings.Add(floraObjects[selectedObjectIndex].bufferPadding);
                        }
                    }
                    else
                    {
                        floraObjectNoiseValue = 0;
                    }

                    #endregion
                }
            }
        }
        t.SetTreeInstances(treeInstances.ToArray(), true);
    }
        #endregion

    #endregion


    public void TerrainObjectTest()
    {
        for(int i = 0; i< floraObjects.Length; i++)
        {
            for (int j = 0; j < floraObjects[i].amount; j++)
            {

            }
        }
    }


    #region Civilization Scripts


    [Header("Civilization Variables")]
    [SerializeField]
    public float CivilizationToTerrainDetailMapScale;
    [SerializeField]
    public NoiseData CivilizationNoise;
    public float totalCivilizationObjectValues;
    public float civilizationNoisePeakValue;
    //public bool hasCity;
    //public float townLimit;
    //public float cityLimit;
    //public int hotSpotCount;
    //public float hotSpotClumpMinDistanceMagnitude;

    //public List<Vector2> hotSpots;
    //public List<float> hotSpotValues;
    public NoiseData CityMap;
    public GameObject CivilizationTestPrefab;
    public TerrainDetailPrefab[] civlizationTestPrefab;
    public float civilizationObjectNoiseValue;

    
    //[SerializeField]
    //public TerrainDetailPrefab[] floraTerrainDetailPrefabs;
    //public AnimationCurve[] floraDetailsCurveDistribution;

    //[SerializeField]
    //public TerrainTexture[] floraTerrainTextures;
    //public AnimationCurve[] floraTextureCurveDistribution;
    //public bool[] useFloraColor4Textures;

    [SerializeField]
    public TerrainObject[] civilizationObjects;
    //public AnimationCurve[] civilizationObjectsSizeCurve;
    public AnimationCurve[] civilizationObjectsCurveDistribution;
    public int civilizationObjectResolutionScale;
    public int civilizationObjectOverlapCheckLimit;

    public void GenerateCivilizationNoise()
    {
      
        // Use a random seed each time the script runs

        int randX = UnityEngine.Random.Range(0, 10000);
        int randY = UnityEngine.Random.Range(0, 10000);


        //CivilizationNoise.texture = new Texture2D(terrain.terrainData.detailResolution, terrain.terrainData.detailResolution);

        CivilizationNoise.heights = new float[terrain.terrainData.detailResolution, terrain.terrainData.detailResolution];

        //CivilizationNoise.heights = GetPerlinNoise(CivilizationNoise.heights,
        //    randX, randY, CivilizationNoise.scale, CivilizationNoise.noiseType,
        //    ref CivilizationNoise.texture);

        CivilizationNoise.heights = GetPerlinNoise_Compare(CivilizationNoise.heights,
            randX, randY, CivilizationNoise.scale, CivilizationNoise.noiseType,
            ref CivilizationNoise.texture, FloraNoiseData.heights, CivilizationFloraInteraction);

        //CivilizationNoise.texture = XizukiMethods.Textures.Xi_Helper_Texture.AdjustContrast(CivilizationNoise.texture, CivilizationNoise.contrast);
    }


    public void CivilizationHotSpotsCheck(float noiseValue, Vector2 pos)
    {
        bool hasHigherValue = false;
        bool inMinimalDistance = false; ;

        //Vector2[] previousHotSpots = hotSpots.ToArray();

        //for (int i = 0; i < hotSpots.Count; i++)
        //{
        //    float magnitude = Mathf.Abs((hotSpots[i]-pos).magnitude);

        //    if(magnitude < hotSpotClumpMinDistanceMagnitude)
        //    {
        //        return;
        //    }
        //}

        //for (int i = 0; i < hotSpots.Count; i++)
        //{
        //    if (hasHigherValue)
        //    {
        //        hotSpots[i] = previousHotSpots[i - 1];
        //    }

        //    else if (noiseValue > hotSpotValues[i])
        //    {
        //        hotSpotValues[i] = noiseValue;
        //        hotSpots[i] = pos;
        //        hasHigherValue = true;
        //    }

        //}
    }


    public float CivilizationFloraInteraction(int x, int y, float CivilizationPixelNoiseValue, float[,] floraNoise)
    {
        float result = CivilizationPixelNoiseValue;

        float InverseFlora = 1 - floraNoise[x, y];

        if (CivilizationPixelNoiseValue > InverseFlora)
        {
            result = InverseFlora;
        }

        //CivilizationHotSpotsCheck(result, new Vector2(x, y));

        return result;
    }

    public float CivilizationHotSpot(int x, int y, float CivilizationPixelNoiseValue)
    {
        float result = CivilizationPixelNoiseValue;

        if (CivilizationPixelNoiseValue > 0)
        {
            result = 0;
        }

        return result;
    }


    public void CivilizationHotSpots()
    {
       
    }






    #region Pre - Add Flora Objects to Terrain 


    [ContextMenu("Add civilizationObject To Terrain")]
    public void AddCivilizationObjectsToTerrain()
    {
        {
            TerrainData t = terrain.terrainData;


            List<TreePrototype> newTreePrototypes = t.treePrototypes.ToList();

            int originalLength = newTreePrototypes.Count;


            for (int i = 0; i < civilizationObjects.Length; i++)
            {
                int trueIndex = originalLength + i;
                civilizationObjects[i].index = trueIndex;

                TerrainObject civilization = civilizationObjects[i];


                //civilization.GameObject[0].GetComponent<MeshRenderer>().material.color = civilizationColor;

                TreePrototype newTreePrototype = new TreePrototype();

                //newDetails[i].prototypeTexture = detail.texture;
                newTreePrototype.prefab = civilization.GameObject[0];
                newTreePrototype.bendFactor = newTreePrototype.bendFactor;
                newTreePrototypes.Add(newTreePrototype);

                //civilizationTerrainDetailPrefabs[prefabIndex];
            }

            t.treePrototypes = newTreePrototypes.ToArray();

        }
    }

    #endregion


    [ContextMenu("CivilizationTest")]
    public void CivilizationTest()
    {
        terrain.drawTreesAndFoliage = true;

        TerrainData t = terrain.terrainData;

        List<TreeInstance> treeInstances = new List<TreeInstance>();


        for (int x = 0; x < t.detailResolution / civilizationObjectResolutionScale; x++)
        {
            for (int y = 0; y < t.detailResolution / civilizationObjectResolutionScale; y++)
            {
                #region Scale civilization Noise to civilization Object Resolution 

                float noiseValue = CivilizationNoise.heights[x, y];

                float sum = 0;

                for (int xx = 0; xx < civilizationObjectResolutionScale; xx++)
                {
                    for (int yy = 0; yy < civilizationObjectResolutionScale; yy++)
                    {
                        sum += CivilizationNoise.heights[x * civilizationObjectResolutionScale + xx, y * civilizationObjectResolutionScale + yy];
                    }
                }

                noiseValue = sum / (civilizationObjectResolutionScale * civilizationObjectResolutionScale);

                #endregion


                float civilizationObjectNoiseValue = noiseValue * civilizationNoisePeakValue;

                List<Vector2> previousPositions = new List<Vector2>();
                List<Vector2> previousPaddings = new List<Vector2>();

                int overlaps = 0;


                int test = 0;
                while (civilizationObjectNoiseValue > civilizationObjects[0].valueCost)
                {
                    #region Select civilization Object to Spawn

                    int selectedObjectIndex = -1;


                    float[] civilizationObjectWeights = new float[civilizationObjects.Length];

                    for (int i = 0; i < civilizationObjects.Length; i++)
                    {
                        civilizationObjectWeights[i] += AdjustValueToCurve(noiseValue, civilizationObjectsCurveDistribution[i], true);
                        if (civilizationObjectWeights[i] < 0) civilizationObjectWeights[i] = 0;
                    }

                    civilizationObjectWeights = NormalizeArrayValues(civilizationObjectWeights);


                    float randomValue = UnityEngine.Random.Range(0f, 1f);
                    float total = 0;


                    int terrainObjectTrueIndex = 0;

                    for (int i = 0; i < civilizationObjectWeights.Length; i++)
                    {
                        total += civilizationObjectWeights[i];
                        if (randomValue > total - civilizationObjectWeights[i] && randomValue < total)
                        {
                            selectedObjectIndex = i;
                            terrainObjectTrueIndex = civilizationObjects[i].index;
                        }
                    }



                    #endregion



                    float randomX = UnityEngine.Random.Range((float)x * civilizationObjectResolutionScale / CivilizationNoise.heights.GetLength(0),
                                                        ((float)x + 1) * civilizationObjectResolutionScale / CivilizationNoise.heights.GetLength(0));
                    float randomZ = UnityEngine.Random.Range((float)y * civilizationObjectResolutionScale / CivilizationNoise.heights.GetLength(1),
                                                        ((float)y + 1) * civilizationObjectResolutionScale / CivilizationNoise.heights.GetLength(1));
                        


                    #region Spawn civilization Object

                    if (selectedObjectIndex >= 0)
                    {

                        bool overlapCheck = OverlappingCheck(new Vector2(randomZ, randomX),
                                                            civilizationObjects[selectedObjectIndex].bufferPadding,
                                                            previousPositions.ToArray(),
                                                            previousPaddings.ToArray());


                        if (overlapCheck) { overlaps++; continue; }
                        if (overlaps >= civilizationObjectOverlapCheckLimit) { civilizationObjectNoiseValue = 0; continue; }



                        float terrainHeight = terrain.SampleHeight(new Vector3(randomZ * t.size.z, 0f, randomX * t.size.x));
                        Vector3 treePosition = new Vector3(randomZ, terrainHeight, randomX);


                        bool roadOverlap = false;


                        Vector3 truePosition = new Vector3(randomZ * terrain.terrainData.size.z,
                           terrainHeight, randomX * terrain.terrainData.size.x);

                    
                        for (int j = 0; j < 100; j++)
                        {
                            Vector3 roadSplinePointPosition = roadSplineContainer.Spline.EvaluatePosition((float)j / 100);


                            //if(x<10 && y<10 && j<20)
                            //{
                            //    //print("roadSplinePointPosition = " + roadSplinePointPosition);
                            //    //print("truePosition = " + truePosition);
                            //    //print("(roadSplinePointPosition - truePosition).magnitude = " + (roadSplinePointPosition - truePosition).magnitude);
                            //}

                            if ((roadSplinePointPosition - truePosition).magnitude <= roadPadding)
                            {
                                roadOverlap = true;
                                continue;
                            }

                        }

                        if (roadOverlap)
                        {
                            civilizationObjectNoiseValue = 0;
                            continue;
                        }

                        float size = 0.2f;//AdjustValueToCurve(noiseValue, civilizationObjectsSizeCurve[selectedObjectIndex], true);

                        if (size > 0f)
                        {
                            treeInstances.Add(new TreeInstance
                            {
                                position = treePosition,
                                rotation = UnityEngine.Random.Range(0, 360),
                                prototypeIndex = terrainObjectTrueIndex,
                                heightScale = size,
                                widthScale = size,
                                color = Color.white
                            }); ;

                            civilizationObjectNoiseValue -= civilizationObjects[selectedObjectIndex].valueCost;

                            previousPositions.Add(new Vector2(randomZ, randomX));
                            previousPaddings.Add(civilizationObjects[selectedObjectIndex].bufferPadding);
                        }

                    }
                    else
                    {
                        civilizationObjectNoiseValue = 0;
                    }

                    #endregion
                }
            }
        }

        List<TreeInstance> originalTreeInstances = t.treeInstances.ToList();
        originalTreeInstances.AddRange(treeInstances);
        t.SetTreeInstances(originalTreeInstances.ToArray(), true);
    }


    #endregion







    public int GetTerrainDetailPrefabFromIndex(int index)
    { 
        for(int i = 0; i < floraTerrainDetailPrefabs.Length; i++) 
        {
            if (floraTerrainDetailPrefabs[i].index == index)
            {
                return i;
            }
        }

        return 0;
    }






    public void GenerateStructures()
    {

    }

    public void GenerateClouds()
    {

    }

    public void GenerateRain()
    {

    }











    float[,] ConvertTextureToFloatArray(Texture2D texture)
    {
        // Get the pixel colors from the texture
        Color[] pixels = texture.GetPixels();

        // Create a 2D array of floats
        float[,] floatArray = new float[texture.width, texture.height];

        // Convert colors to grayscale values and store in the float array
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                float grayscaleValue = pixels[y * texture.width + x].grayscale;

                floatArray[x, y] = grayscaleValue;
            }
        }

        return floatArray;
        // Now, 'floatArray' contains the grayscale values ranging from 0 to 1
        // You can use this array as needed in your code
    }


}
