using System.Collections;
using System.Collections.Generic;
 using UnityEngine;
using UnityEngine.Splines;
using XizukiMethods.GameObjects;
using XizukiMethods.Math;

public class CarScript : MonoBehaviour
{
    public Transform carParent;
    public NamedPipeServer namedPipeServer;
    public float speed;
    public float timeToMaxSpeed;
    public float maxSpeed;

    public float eegRate;

    public float sidewaysSpeed;
    public float sidewaysLimit;

    public CarFeedbackScript carFeedbackScript;
    public SplineAnimate splineAnimate;
    // Start is called before the first frame update
    void Start()
    {
        carFeedbackScript = Xi_Helper_GameObjects.ConditionalAssignment<CarFeedbackScript>(GetComponent<CarFeedbackScript>());
        splineAnimate = Xi_Helper_GameObjects.ConditionalAssignment<SplineAnimate>(transform.parent.GetComponent<SplineAnimate>());
        namedPipeServer = GameObject.FindObjectOfType<NamedPipeServer>();   

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }


    public bool justHit;
    public void EEGRateCalculation()
    {
        if (justHit)
        {
            if (eegRate > 0 + (1 / timeToMaxSpeed * Time.deltaTime * 3))
                eegRate -= 1 / timeToMaxSpeed * Time.deltaTime * 3;
            else
                eegRate = 0;


            return;
        }

        if (GameManager.instance.state == GameState.Interval)
        {
            if (eegRate > 0 + (1 / timeToMaxSpeed * Time.deltaTime))
                eegRate -= 1 / timeToMaxSpeed * Time.deltaTime;
            else
                eegRate = 0;


            return;
        }

        if (namedPipeServer.pipeValues[0].value == "true")
        {
            if (eegRate < 1 - (1 / timeToMaxSpeed * Time.deltaTime))
                eegRate += 1 / timeToMaxSpeed * Time.deltaTime;
            else
                eegRate = 1;
        }
        else if (namedPipeServer.pipeValues[0].value == "false")
        {
            if (eegRate > 0 + (1 / timeToMaxSpeed * Time.deltaTime))
                eegRate -= 1 / timeToMaxSpeed * Time.deltaTime;
            else
                eegRate = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(splineAnimate.NormalizedTime >= 0.995)
        {

            StartCoroutine( GameManager.instance.LoadNewWorld());

        }


        if (GameManager.instance.state != GameState.Playing)
        {
            carFeedbackScript.Bopping(1);
            carFeedbackScript.Music(1);

            eegRate = 0;
            EEGRateCalculation();

            return;  
        }

        EEGRateCalculation();


        carFeedbackScript.AnimateWheels(eegRate);
        carFeedbackScript.Bopping(eegRate);  
        carFeedbackScript.Music(eegRate);
        carFeedbackScript.LightFeedBack(eegRate);

        //carFeedbackScript.SmokeVFX(eegRate);
        //carFeedbackScript.CarTrackVFX(eegRate);
        //carFeedbackScript.SpeedLinesVFX(eegRate);

        UIManager.Instance.UpdateProgressionUI(splineAnimate.NormalizedTime);

        float lerpTValue = (tiltT + 1) / 2;
        carParent.localEulerAngles = Vector3.Lerp(new Vector3(0, -tiltAngleLimit, 0), new Vector3(0, tiltAngleLimit, 0), lerpTValue);
    }

    private void FixedUpdate()
    {
     
        AutoMove();
    }

    public void AutoMove()
    {
        speed = (eegRate * maxSpeed);

        if (speed <= 0) { splineAnimate.Pause(); return; }

        splineAnimate.Play();

        splineAnimate.ElapsedTime = (splineAnimate.ElapsedTime / speed) * splineAnimate.MaxSpeed;

        splineAnimate.MaxSpeed = speed;
    }

    

    public void AutoTilt()
    {
        float dir = 0;

        if (tiltT > 0) { dir = 1; }
        else dir = -1;

        float timeSpeed = 1 / timeToTiltBackToDefault;


        if(!XizukiMethods.Math.Xi_Helper_Math.EstimatedEqual(ref tiltT, 0, EstimatedEqualType.Range, 0.1f))
        {
            XizukiMethods.Math.Xi_Helper_Math.Clamp(ref tiltT, -1, 1, Time.deltaTime * -dir * timeSpeed * (0.3f + (eegRate * 0.7f)));
        }
    }

    public void Move(float dir)
    {
        float timeSpeed = 1 / timeToTilt;

        if(tiltT * dir < 0)
        {
            timeSpeed *= 2.5f;
        }

        transform.localPosition += new Vector3(dir * sidewaysSpeed * (0.3f+(eegRate*0.7f)), 0, 0)*Time.deltaTime;

        transform.localPosition =
            new Vector3(XizukiMethods.Math.Xi_Helper_Math.Clamp
            (transform.localPosition.x, -sidewaysLimit, sidewaysLimit, Time.deltaTime * dir * timeSpeed * (0.3f + (eegRate * 0.7f))), transform.localPosition.y, 0);


        XizukiMethods.Math.Xi_Helper_Math.Clamp(ref tiltT, -1, 1, Time.deltaTime * dir * timeSpeed * (0.3f + (eegRate * 0.7f)));
    }






    public float tiltAngleLimit;
    public float timeToTiltBackToDefault;
    public float timeToTilt;
    public float tiltT;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Obstacle") return;

        //print("TRIGGER ENTER Obstacle");

        Vector3 randomForce = new Vector3(UnityEngine.Random.Range(-0.15f,0.15f),
                                            -1,
                                            UnityEngine.Random.Range(-0.15f, 0.15f));

        other.gameObject.GetComponent<Rigidbody>().AddExplosionForce(10000f, randomForce, 10f);

        GetHit();
    }

    public MeshRenderer[] meshRenderers;
    public Material baseMat;
    public Material hitMat;

    public Color baseColor;
    public Color hitColor;
    public Color recoveryColor;

    public int hitTimeTicks;
    public float hitTime;

    public void GetHit()
    {       
        StartCoroutine(HitVFX());
    }

    IEnumerator HitVFX()
    {
        justHit = true;
        float perTickTime = hitTime / (float)hitTimeTicks;
        int tickCounter = 0;

        hitMat.color = hitColor;

        yield return new WaitForSeconds(perTickTime / 2);

        hitMat.color = baseColor;
        tickCounter++;

        while (tickCounter < hitTimeTicks)
        {
            yield return new WaitForSeconds(perTickTime/2);
            hitMat.color = recoveryColor;
            //foreach (MeshRenderer renderer in meshRenderers)
            //{
            //    renderer.enabled = false;
            //}
            yield return new WaitForSeconds(perTickTime/2);
            hitMat.color = baseColor;
            //foreach (MeshRenderer renderer in meshRenderers)
            //{
            //    renderer.enabled = true;
            //}
            tickCounter++;
        }

        justHit = false;
    }
}


