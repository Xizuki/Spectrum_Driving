using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Splines.Interpolators;
using UnityEngine.UI;
using UnityEngine.VFX;

[RequireComponent(typeof(AudioSource))]
public class CarFeedbackScript : MonoBehaviour
{
    public GameObject[] wheels;

    public ParticleSystem[] windVFXs;

    public AudioSource musicSource;
    public AudioSource soundSource;
    public float musicBaseValue;

    public Image speedLines;

    public ParticleSystem smokePrefab;
    public float smokeRate;

    public float wheelSpeed;

    [Header("Smoke VFX Values")]
    public VisualEffect smokeVFX;
    public float smokeRateMin;
    private float smokeRateMax;
    public float smokeSizeMin;
    public float smokeSizeMax;
    public float smokeSizeRanRatio;
    public float smokeLifeTimeMinRatio;
    public float smokeLifeTimeRanMin; 
    public float smokeLifeTimeRanMax;

    [Header("Lights VFX Values")]
    public Light[] frontLights;
    public Light backLight;
    public Light spotLight;

    public float frontLightsLowestRatio;
    public float backLightsLowestRatio;

    private float frontLightsIntensityMax;
    private float frontLightsIntensityMin;
    private float frontLightsRangeMax;
    private float frontLightsRangeMin;
    private float backLightsIntensityMax;
    private float backLightsIntensityMin;
    private float backLightsRangeMax;
    private float backLightsRangeMin;

    [Header("Car Tracks VFX Values")]
    public VisualEffect[] carTracks;





    // Start is called before the first frame update
    void Start()
    {
        baseScale = transform.localScale;
        musicSource = XizukiMethods.GameObjects.Xi_Helper_GameObjects.ConditionalAssignment<AudioSource>(GetComponent<AudioSource>());
        InitializeBaseValues();
    }

    void InitializeBaseValues()
    { 
        smokeRateMax = smokeVFX.GetFloat("Smoke Rate");



        backLightsIntensityMax = backLight.intensity;
        backLightsRangeMax = backLight.range;
        frontLightsIntensityMax = frontLights[0].intensity;
        frontLightsRangeMax = frontLights[0].range;

        backLightsIntensityMin = backLightsIntensityMax * backLightsLowestRatio;
        backLightsRangeMin = backLightsRangeMax * backLightsLowestRatio;
        frontLightsIntensityMin = frontLightsIntensityMax * frontLightsLowestRatio;
        frontLightsRangeMin = frontLightsRangeMax * frontLightsLowestRatio;

    }

    // Update is called once per frame
    void Update()
    {
    }


    public float spotLightIntesityMin;
    public float spotLightIntesityMax;

    public void LightFeedBack(float eegValue)
    {
        for(int i = 0; i < frontLights.Length; i++) 
        {
            frontLights[i].intensity = Mathf.Lerp( frontLightsIntensityMin, frontLightsIntensityMax,eegValue);
            frontLights[i].range = Mathf.Lerp(frontLightsRangeMin, frontLightsRangeMax, eegValue);

            spotLight.intensity = Mathf.Lerp(spotLightIntesityMin, spotLightIntesityMax, eegValue);

            backLight.intensity = Mathf.Lerp(backLightsIntensityMax, backLightsIntensityMin, eegValue);
            backLight.range = Mathf.Lerp(backLightsRangeMax, backLightsRangeMin, eegValue);
        }
    }

    public void Music(float eegValue)
    {
        musicSource.volume = musicBaseValue + ((1 - musicBaseValue)* eegValue);
        soundSource.volume = musicBaseValue + ((1 - musicBaseValue) * eegValue);
    }

    public void SmokeVFX(float eegRate)
    {
        float smokeRate = Mathf.Lerp(smokeRateMin, smokeRateMax, eegRate);
        smokeVFX.SetFloat("Smoke Rate", smokeRate);
        
        float smokeSize = Mathf.Lerp(smokeSizeMin, smokeSizeMax, eegRate);
        smokeVFX.SetFloat("Smoke Size Min", smokeSize*smokeSizeRanRatio);
        smokeVFX.SetFloat("Smoke Size Max", smokeSize);

        float eegSizeRatio = Mathf.Lerp(smokeSizeRanRatio, 1, eegRate);
        smokeVFX.SetFloat("Smoke Life Time Ran Min", smokeLifeTimeRanMin * eegSizeRatio);
        smokeVFX.SetFloat("Smoke Life Time Ran Max", smokeLifeTimeRanMax * eegSizeRatio);
    }

    public void CarTrackVFX(float eegRate)
    {
        carTracks[0].SetVector3("Rotation", transform.eulerAngles);
        carTracks[1].SetVector3("Rotation", transform.eulerAngles);

        carTracks[0].SetFloat("Speed", eegRate);
        carTracks[1].SetFloat("Speed", eegRate);

    }

    public void SpeedLinesVFX(float eegRate)
    { 
    
    }

    public GameObject bopParentalOffset;
    public Vector3 baseScale;
    public float bopT;
    public float bopAmount;
    public float bopSpeed;
    public float bopYOffset;
    public void Bopping(float eegRate)
    {
        bopT += Time.deltaTime* bopSpeed* eegRate;
        bopParentalOffset.transform.localScale = baseScale+ (new Vector3(Mathf.Sin(bopT), Mathf.Cos(bopT + bopYOffset),0)/100)*bopAmount* eegRate;
    }

    public void AnimateWheels(float eegRate)
    {
        foreach (GameObject wheel in wheels)
        {
            wheel.transform.eulerAngles += new Vector3(0, 0, eegRate* wheelSpeed) * Time.deltaTime;
        }
    }
}
