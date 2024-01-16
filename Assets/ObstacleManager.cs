using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using static UnityEngine.Splines.SplineInstantiate;
using Space = UnityEngine.Splines.SplineInstantiate.Space;


public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager instance;

    public float spawnFrequency;

    public List<GameObject> obstacles = new List<GameObject>();

    public List<float> obstaclesSpawnPercentage;

    public Vector3 maxPositionalOffset;

    public SplineInstantiate splineInstantiate;

    public Vector2[] minMaxSpacingDifficultySettings;


    // Start is called before the first frame update
    void Awake()
    {
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<ObstacleManager>(ref instance, this);
    }

    private void Start()
    {
        
    }

    public void SetObstacleObjects()
    {

    }

    public void GenerateObstacles()
    {
        splineInstantiate.MaxPositionOffset = maxPositionalOffset;

        splineInstantiate.MinSpacing = minMaxSpacingDifficultySettings[0].x;
        splineInstantiate.MaxSpacing = minMaxSpacingDifficultySettings[0].y;

        // splineInstantiate.positionSpace = new SplineInstantiate.OffsetSpace();

        splineInstantiate.Randomize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
