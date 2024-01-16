using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarUpgradesManager : MonoBehaviour
{
    public GameObject currentCar;
    public GameObject[] cars;
    public float scoreToUpgrade;
    public float adjustedUpgradeScore;
    public int currentCarIndex;

    public void UpgradeScoring(float addedScore)
    {
        adjustedUpgradeScore += addedScore;

        if(adjustedUpgradeScore >= scoreToUpgrade)
        {
            currentCarIndex += 1;
            adjustedUpgradeScore -= scoreToUpgrade;
            CarUpgrade();
        }
    }

    public void CarUpgrade()
    {
        // ADD More Stuff
        currentCar = GameObject.Instantiate(cars[currentCarIndex]);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
