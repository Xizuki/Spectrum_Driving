using Unity.Burst.CompilerServices;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public struct MapPreset
{
    public string name;


    public int[] terrainPresets;
    public int[] floraPresets;
    public int[] civilizationPresets;
    public int[] waterPrefabs;
    public int[] skyboxes;

    public bool hasWater;
}


[System.Serializable]
public struct TerrainPreset
{
    public string name;

    public float[] terrainHeight;

    public NoiseData[] noises;

    public float waterVariance;

}

[System.Serializable]
public struct FloraPresets
{
    public string name;

    public NoiseData noise;

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

    public float floraNoisePeakValue;
    public Color floraColor;
}
[System.Serializable]
public struct CivilizationPresets
{
    public string name;

    public NoiseData noise;

    public TerrainObject[] civilizationObjects;
    public AnimationCurve[] civilizationObjectsCurveDistribution;

    public float civilizationNoisePeakValue;
}


[RequireComponent (typeof (MapManager))]
public class MapRandomizer : MonoBehaviour
{
    public MapManager mapManager;

    public GameObject[] roadSplineReferences;

    [SerializeField]
    ForcedPreset forcedPreset;

    [SerializeField]
    MapPreset[] presets;

    [SerializeField]
    public TerrainPreset[] terrainPresets;

    [SerializeField]
    public FloraPresets[] floraPresets;

    [SerializeField]
    public CivilizationPresets[] civilizationPresets;

    [SerializeField]
    public GameObject[] waterPrefabs;

    [SerializeField]
    public Material[] skyboxes;


    // Start is called before the first frame update
    void Start()
    {
        mapManager = GetComponent<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    [ContextMenu("Assign To Map Manager")]
    public void AssingSpecificVariables()
    {
        if (!forcedPreset.forcedPreset) { RandomizeMapManager(); return; }

        mapManager.roadSplinePrefabGO = roadSplineReferences[forcedPreset.roadSplinePrefabGO];

        mapManager.waterPrefab = waterPrefabs[forcedPreset.waterPrefab];

        int mapPresetIndex = forcedPreset.mapPresetIndex;

        if (forcedPreset.forcedPresetNonRadom)
        {
            int index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            int value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.TerrainNoiseDatas = terrainPresets[value].noises;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.waterVariance = terrainPresets[value].waterVariance;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.floraTerrainDetailPrefabs = floraPresets[value].floraTerrainDetailPrefabs;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.floraDetailsCurveDistribution = floraPresets[value].floraDetailsCurveDistribution;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.floraTerrainTextures = floraPresets[value].floraTerrainTextures;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index]; mapManager.floraTextureCurveDistribution = floraPresets[value].floraTextureCurveDistribution;
            mapManager.useFloraColor4Textures = floraPresets[value].useFloraColor4Textures;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index]; mapManager.floraObjects = floraPresets[value].floraObjects;
            mapManager.floraObjectsSizeCurve = floraPresets[value].floraObjectsSizeCurve;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.floraObjectsCurveDistribution = floraPresets[value].floraObjectsCurveDistribution;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.floraNoisePeakValue = floraPresets[value].floraNoisePeakValue;

            index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            mapManager.floraColor = floraPresets[value].floraColor;

            index = Random.Range(0, presets[mapPresetIndex].skyboxes.Length);
            value = presets[mapPresetIndex].terrainPresets[index];
            RenderSettings.skybox = skyboxes[value];
        }
        else
        {
            mapManager.TerrainNoiseDatas = terrainPresets[forcedPreset.terrainNoise].noises;

            mapManager.waterVariance = terrainPresets[forcedPreset.waterVariance].waterVariance;

            mapManager.floraTerrainDetailPrefabs = floraPresets[forcedPreset.floraTerrainDetailPrefabs].floraTerrainDetailPrefabs;

            mapManager.floraDetailsCurveDistribution = floraPresets[forcedPreset.floraDetailsCurveDistribution].floraDetailsCurveDistribution;

            mapManager.floraTerrainTextures = floraPresets[forcedPreset.floraTerrainTextures].floraTerrainTextures;

            mapManager.useFloraColor4Textures = floraPresets[forcedPreset.useFloraColor4Textures].useFloraColor4Textures;

            mapManager.floraObjectsSizeCurve = floraPresets[forcedPreset.floraObjectsSizeCurve].floraObjectsSizeCurve;
    
            mapManager.floraObjectsCurveDistribution = floraPresets[forcedPreset.floraObjectsCurveDistribution].floraObjectsCurveDistribution;

            mapManager.floraNoisePeakValue = floraPresets[forcedPreset.floraNoisePeakValue].floraNoisePeakValue;

            mapManager.floraColor = floraPresets[forcedPreset.floraColor].floraColor;


            mapManager.CivilizationNoise = civilizationPresets[forcedPreset.civilizationNoise].noise;
            mapManager.civilizationObjects = civilizationPresets[forcedPreset.civilizationObjects].civilizationObjects;
            mapManager.civilizationObjectsCurveDistribution = civilizationPresets[forcedPreset.civilizationObjectsCurveDistribution].civilizationObjectsCurveDistribution;
            mapManager.civilizationNoisePeakValue = civilizationPresets[forcedPreset.civilizationNoisePeakValue].civilizationNoisePeakValue;


            RenderSettings.skybox = skyboxes[forcedPreset.skybox];
        }
    }

    [System.Serializable]
    public struct ForcedPreset
    {
        public bool forcedPreset;


        public int roadSplinePrefabGO;

        public int waterPrefab;

        public int waterVariance;

        public int skybox;



        public int mapPresetIndex;

        public bool forcedPresetNonRadom;


        public int terrainNoise;

        public int floraNoise;

        public int floraTerrainDetailPrefabs;
        public int floraDetailsCurveDistribution;

        public int floraTerrainTextures;
        public int floraTextureCurveDistribution;
        public int useFloraColor4Textures;

        public int floraObjects;
        public int floraObjectsSizeCurve;
        public int floraObjectsCurveDistribution;

        public int floraNoisePeakValue;
        public int floraColor;



        public int civilizationNoise;

        public int civilizationObjects;
        public int civilizationObjectsCurveDistribution;

        public int civilizationNoisePeakValue;
    }

    [ContextMenu("Randomize Map Manager")]
    public void RandomizeMapManager()
    {

        int index = Random.Range(0, roadSplineReferences.Length);
        mapManager.roadSplinePrefabGO = roadSplineReferences[index];


        index = Random.Range(0, waterPrefabs.Length);
        mapManager.waterPrefab = waterPrefabs[index];


        int mapPresetIndex = Random.Range(0, presets.Length);

        mapManager.hasWater = presets[mapPresetIndex].hasWater;


        index = Random.Range(0, presets[mapPresetIndex].terrainPresets.Length);

        int value = presets[mapPresetIndex].terrainPresets[index];

        mapManager.TerrainNoiseDatas = terrainPresets[value].noises;

        float terrainHeight = Random.Range(terrainPresets[value].terrainHeight[0], terrainPresets[value].terrainHeight[1]);

        mapManager.size = new Vector3(mapManager.size.x, terrainHeight, mapManager.size.z);

        mapManager.waterVariance = terrainPresets[value].waterVariance;




        index = Random.Range(0, presets[mapPresetIndex].floraPresets.Length);

        value = presets[mapPresetIndex].floraPresets[index];

        mapManager.floraTerrainDetailPrefabs = floraPresets[value].floraTerrainDetailPrefabs;

        mapManager.floraDetailsCurveDistribution = floraPresets[value].floraDetailsCurveDistribution;

        mapManager.floraTerrainTextures = floraPresets[value].floraTerrainTextures;

        mapManager.floraTextureCurveDistribution = floraPresets[value].floraTextureCurveDistribution;

        mapManager.useFloraColor4Textures = floraPresets[value].useFloraColor4Textures;

        mapManager.floraObjects = floraPresets[value].floraObjects;


        mapManager.floraObjectsSizeCurve = floraPresets[value].floraObjectsSizeCurve;

        mapManager.floraObjectsCurveDistribution = floraPresets[value].floraObjectsCurveDistribution;

        mapManager.floraNoisePeakValue = floraPresets[value].floraNoisePeakValue;
        
        mapManager.floraColor = floraPresets[value].floraColor;



        value = presets[mapPresetIndex].civilizationPresets[index]; 
        mapManager.floraObjects = floraPresets[value].floraObjects;
        mapManager.floraObjectsSizeCurve = floraPresets[value].floraObjectsSizeCurve;

        value = presets[mapPresetIndex].floraPresets[index];
        mapManager.floraObjectsCurveDistribution = floraPresets[value].floraObjectsCurveDistribution;

        value = presets[mapPresetIndex].terrainPresets[index];
        mapManager.floraNoisePeakValue = floraPresets[value].floraNoisePeakValue;

        value = presets[mapPresetIndex].terrainPresets[index];
        mapManager.floraColor = floraPresets[value].floraColor;




        index = Random.Range(0, presets[mapPresetIndex].skyboxes.Length);
        value = presets[mapPresetIndex].skyboxes[index];
        RenderSettings.skybox = skyboxes[value];
    }
}
