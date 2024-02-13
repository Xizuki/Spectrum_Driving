using System.Diagnostics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;


struct Cube
{
    public float3 position;
    public float4 color;
}



public class ComputeShaderTester : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;

    private Cube[] data;

    // Start is called before the first frame update
    void Start()
    {
        GeneratePerlinNoise();
        //CubesStructInitialization();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int repetitions;
    public GameObject[] objects;

    [ContextMenu("Randomized CPU")]
    public void OnRandomizedCPU()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        for(int i =0; i < repetitions; i ++)
        {
            for (int j = 0; j < objects.Length; j++)
            {
                GameObject obj = objects[j];
                obj.transform.position = new Vector3(obj.transform.position.x, UnityEngine.Random.Range(-0.1f, 0.1f) , obj.transform.position.z);
                obj.GetComponent<MeshRenderer>().material.color = UnityEngine.Random.ColorHSV();
            }
        }

        stopwatch.Stop();

        //print("Elapsed Time = " + stopwatch.Elapsed);
    }

    public ComputeShader perlinComputeShader;
    public int width = 256;
    public int height = 256;

    private RenderTexture noiseTexture;
    private float[,] noiseMap;
    [ContextMenu("GeneratePerlinNoise")]
    void GeneratePerlinNoise()
    {
        // Create a render texture to store the generated noise
        noiseTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);
        noiseTexture.enableRandomWrite = true;
        noiseTexture.Create();

        // Create a 2D array to store the noise values
        noiseMap = new float[width, height];

        // Set the compute shader parameters
        int kernelIndex = perlinComputeShader.FindKernel("CSMain"); // Adjust "CSMain" if your kernel name is different
        perlinComputeShader.SetTexture(kernelIndex, "Result", noiseTexture);
        perlinComputeShader.Dispatch(kernelIndex, width / 8, height / 8, 1); // Adjust thread group size as needed

        // Read the render texture data into the noise map
        RenderTexture.active = noiseTexture;
        var noiseData = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        noiseData.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        RenderTexture.active = null;

        // Convert the texture data to a 2D array
        Color[] pixels = noiseData.GetPixels();
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = pixels[y * width + x].r;
            }
        }

        // Cleanup
        Destroy(noiseData);
    }

    // Example method to access the noise map
    void ExampleMethod()
    {
        // Access noise values from the noise map
        float value = noiseMap[10, 20];
        UnityEngine.Debug.Log("Noise value at position (10, 20): " + value);
    }

    void OnDestroy()
    {
        // Release the render texture
        if (noiseTexture != null)
            noiseTexture.Release();
    }



    [ContextMenu("Randomized GPU")]
    public void OnRandomizedGPU()
    {
        Stopwatch stopwatch = Stopwatch.StartNew();


        int colorSize = sizeof(float) * 4;
        int vector3Size = sizeof(float) * 3;
        int totalSize = colorSize + vector3Size;

        ComputeBuffer cubesBuffer = new ComputeBuffer(data.Length, totalSize);
        cubesBuffer.SetData(data);

        computeShader.SetBuffer(0, "cubes", cubesBuffer);
        computeShader.SetFloat("resolution", data.Length);
        computeShader.SetFloat("repetitions", repetitions);
        computeShader.SetInt("randSeedValue", (int)DateTime.Now.Ticks);
        computeShader.Dispatch(0, data.Length / 10, 10, 10);


        cubesBuffer.GetData(data);

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject obj = objects[i];
            Cube cube = data[i];

            obj.transform.position = cube.position;
            obj.GetComponent<MeshRenderer>().material.color = new Color(cube.color.x, cube.color.y, cube.color.z, cube.color.y);
        }

        cubesBuffer.Dispose();



        stopwatch.Stop();

        //print("Elapsed Time = " + stopwatch.Elapsed);
    }

    public void CubesStructInitialization()
    {
        data = new Cube[objects.Length];
        int index = 0;
        foreach(GameObject GO in objects)
        {
            data[index].position = GO.transform.position;
            data[index].color = new float4( 125, 120, 125, 125);

            index++;
        }
    }
}
