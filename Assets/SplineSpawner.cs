using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SplineSpawner : MonoBehaviour
{
    // BANDAID FIX
    public MapManager mapManager;

    public List<GameObject> GOs;

    public SplineContainer splineContainer;

    public GameObject prefabToSpawn;

    public float distance;

    public float distanceRandomizer;

    public float resolution;

    public float distancePerResolution;

    public float tValuePerDistance;

    public float totalDistance;
    void Start()
    {
     
    }

    void Update()
    {
        
    }



    [ContextMenu("Reset Prefabs")]
    public void ResetPrefabs()
    {
        foreach (GameObject go in GOs)
        {
            GameObject.DestroyImmediate(go);
        }
        GOs.Clear();
    }


    //VERY BASIC
    [ContextMenu("Spawn Prefabs (BASIC)")]
    public void SpawnPrefabsBasic()
    {
        ResetPrefabs();

        float leftOverDistance = 0;


        for (float i = 1; i < resolution; i++)
        {
            Vector3 pos1 = splineContainer.Spline.EvaluatePosition(i-1 / resolution);
            Vector3 pos2 = splineContainer.Spline.EvaluatePosition(i / resolution);
            Vector3 posDiff = (pos2 - pos1);

            totalDistance += posDiff.magnitude;
            leftOverDistance += posDiff.magnitude;


            if (leftOverDistance > distance)
            {
                GOs.Add(GameObject.Instantiate(prefabToSpawn, pos2,Quaternion.identity, transform));
                leftOverDistance-= distance* resolution;
            }
        }
    }

    [SerializeField]
    public NoiseData noise;
    public Vector2 prefabPositionalOffset;
    public bool isClamped;
    public AnimationCurve[] prefabsCurveDistribution = new AnimationCurve[] { };

    [ContextMenu("Spawn Prefabs Noise")]
    public void SpawnPrefabsNoise()
    {
        ResetPrefabs();

        float leftOverDistance = 0;


        for (float i = 1; i < resolution; i++)
        {
            Vector3 pos1 = splineContainer.Spline.EvaluatePosition(i - 1 / resolution);
            Vector3 pos2 = splineContainer.Spline.EvaluatePosition(i / resolution);
            Vector3 posDiff = (pos2 - pos1);

            totalDistance += posDiff.magnitude;
            leftOverDistance += posDiff.magnitude;

            float detailToSizeScale = (float)mapManager.terrain.terrainData.detailResolution / (float)mapManager.terrain.terrainData.size.x;

            int upperX = Mathf.CeilToInt(pos1.x * detailToSizeScale);
            int lowerX = Mathf.FloorToInt(pos1.x * detailToSizeScale);
            int upperY = Mathf.CeilToInt(pos1.z * detailToSizeScale);
            int lowerY = Mathf.FloorToInt(pos1.z * detailToSizeScale);


            float[,] values = new float[2, 2];
            values[0, 0] = mapManager.CivilizationNoise.heights[lowerX, lowerY];
            values[1, 0] = mapManager.CivilizationNoise.heights[upperX, lowerY];
            values[0, 1] = mapManager.CivilizationNoise.heights[lowerX, upperY];
            values[1, 1] = mapManager.CivilizationNoise.heights[lowerX, lowerY];


            float averagedValue = (values[0,0] + values[1, 0] + values[0, 1] + values[1, 1])/4;

            float scaleValue = prefabsCurveDistribution[0].Evaluate(averagedValue);

            if (leftOverDistance > distance / averagedValue)
            {
                GOs.Add(GameObject.Instantiate(prefabToSpawn, pos2, Quaternion.identity, transform));
                GOs[GOs.Count - 1].transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
                leftOverDistance -= distance / averagedValue * resolution;
            }
        }
    }
}