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

    public Vector3 minPositionalOffset;
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

    public void UpdateObstacles(SplineInstantiate reference)
    {
        splineInstantiate = reference;
    }

    [ContextMenu("GenerateObstacles")]
    public void GenerateObstaclesContextMenu()
    {
        GenerateObstacles(4);
    }
    public void GenerateObstacles(int difficulty)
    {
        splineInstantiate.MinPositionOffset = minPositionalOffset;

        splineInstantiate.MaxPositionOffset = maxPositionalOffset;

        splineInstantiate.MinSpacing = minMaxSpacingDifficultySettings[difficulty].x;
        splineInstantiate.MaxSpacing = minMaxSpacingDifficultySettings[difficulty].y;

        // splineInstantiate.positionSpace = new SplineInstantiate.OffsetSpace();

        splineInstantiate.Randomize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
