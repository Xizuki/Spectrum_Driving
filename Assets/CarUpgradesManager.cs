using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct GameObjectArray
{
    public GameObject[] gameobjects;
}
public class CarUpgradesManager : MonoBehaviour
{
    public CarScript carScript;

    public AudioSource audioSource;

    public Transform currentCarPos;
    public float scoreToUpgrade;
    public float adjustedUpgradeScore;
    public int currentCarIndex;
    public GameObject[] cars;

    public AudioClip upgradeSound;

    [SerializeField]
    public GameObjectArray[] wheels;

    public void ChangeCar()
    {

    }
    public void UpgradeScoring(float addedScore)
    {
        adjustedUpgradeScore += addedScore;

        if(adjustedUpgradeScore >= scoreToUpgrade)
        {
            adjustedUpgradeScore -= scoreToUpgrade;
            CarUpgrade();
        }
    }

    public void CarUpgrade()
    {
        audioSource.PlayOneShot(upgradeSound);

        cars[currentCarIndex].SetActive(false);

        currentCarIndex++;

        cars[currentCarIndex].SetActive(true);

        carScript.carFeedbackScript.wheels = wheels[currentCarIndex].gameobjects;
    }

    // Start is called before the first frame update
    void Start()
    {
        carScript = GetComponent<CarScript>(); 
        
        carScript.carFeedbackScript.wheels = wheels[currentCarIndex].gameobjects;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
