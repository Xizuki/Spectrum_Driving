using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using TMPro;
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
    public Vector3[] wheelsRotateAxis;


    public float transitionTime;

    public GameObject maskQuad;
    public GameObject maskQuadEndPos;
    public ParticleSystem carUpgradeVFX;

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
        carUpgradeVFX.Play();

        //StartCoroutine(CarUpgradeVFX());

        cars[currentCarIndex].SetActive(false);
        currentCarIndex++;
        cars[currentCarIndex].SetActive(true);

        carScript.carFeedbackScript.wheels = wheels[currentCarIndex].gameobjects;
        carScript.carFeedbackScript.rotateAxis = wheelsRotateAxis[currentCarIndex];

    }

    IEnumerator CarUpgradeVFX()
    {
        float timer = 0;

        Vector3 originalPos = maskQuad.transform.localPosition;

        maskQuad.SetActive(true);
        cars[currentCarIndex+1].SetActive(true);


        cars[currentCarIndex].layer = 23;
        foreach (Transform go in cars[currentCarIndex].GetComponentsInChildren<Transform>()) 
        {
            go.gameObject.layer = 23;
        }

        cars[currentCarIndex + 1].layer = 24;
        foreach (Transform go in cars[currentCarIndex + 1].GetComponentsInChildren<Transform>())
        {
            go.gameObject.layer = 24;
        }

        while (timer < transitionTime)
        {
            maskQuad.transform.localPosition = Vector3.Lerp(originalPos, maskQuadEndPos.transform.localPosition, timer / transitionTime);

            yield return new WaitForFixedUpdate();
            timer += Time.deltaTime;
        }

        cars[currentCarIndex].SetActive(false);
        maskQuad.SetActive(false);

        currentCarIndex++;

        maskQuad.transform.localPosition = originalPos;

        cars[currentCarIndex].layer = 0;
        foreach (Transform go in cars[currentCarIndex].GetComponentsInChildren<Transform>())
        {
            go.gameObject.layer = 0;
        }
    }

    public TMP_Text debugText1;
    public TMP_Text debugText2;

    // Start is called before the first frame update
    void Start()
    {

        //if(carScript)
        //    debugText1.text = "true";
        //else
        //    debugText1.text = "false";

        carScript = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CarScript>();

        carScript = GetComponent<CarScript>();

        carScript.carFeedbackScript.wheels = wheels[currentCarIndex].gameobjects;

    }

    int frameCounter = 0;
    // Update is called once per frame
    void Update()
    {
        if(frameCounter==0)
        {
            //carScript = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<CarScript>();

            //carScript = GetComponent<CarScript>();

            //carScript.carFeedbackScript.wheels = wheels[currentCarIndex].gameobjects;
        }

        frameCounter++;

        if (carScript)
            debugText2.text = "true";
        else
            debugText2.text = "false";
    }
}
