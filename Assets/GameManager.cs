using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;
using TMPro;
using XizukiMethods.Math;

public enum GameState { Playing, Interval, Loading }
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float intervalTimer;
    public bool isPaused;

    public UIManager uiManager;
    public CarUpgradesManager carUpgradesManager;
    public  GameState state;
    public GameObject CameraPivot;
    public float IntervalRotationSpeed;

    public int currentSession;
    public float highestScoreOfSession;
    public List<float> scores = new List<float>();
    public float scoreScale;

    public CarScript carScript;
    private Vector3 camPivotOriginalPos;

    public float obstacleDestroyDistance;


    public IEnumerator LoadNewWorld()
    {
        GameManager.instance.state = GameState.Loading;


        UIManager.Instance.loadingScreenUI.SetActive(true);

        yield return new WaitForSeconds(0.02f);

        MapManager.Instance.LoadScene();


        carScript.splineAnimate.NormalizedTime = 0;

        yield return new WaitForSeconds(0.02f);

        UIManager.Instance.loadingScreenUI.SetActive(false);


        GameManager.instance.state = GameState.Playing;

    }



    // Start is called before the first frame update
    void Start()
    {
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<GameManager>(ref instance, this);
        uiManager = XizukiMethods.GameObjects.Xi_Helper_GameObjects.ConditionalAssignment<UIManager>(GetComponent<UIManager>());
        carUpgradesManager = XizukiMethods.GameObjects.Xi_Helper_GameObjects.ConditionalAssignment<CarUpgradesManager>(GameObject.FindGameObjectWithTag("Player").GetComponent<CarUpgradesManager>());
        carScript = GameObject.FindObjectOfType<CarScript>();
        camPivotOriginalPos = CameraPivot.transform.localEulerAngles;


        MusicManager.instance.RandomizeMusic(ref carScript);
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 24;

        IntervalCamera();
        Scoring();
        uiManager.UpdateScore(Mathf.RoundToInt(scores[currentSession]));
    }

    public void Scoring()
    {
        scores[currentSession] += carScript.speed * Time.deltaTime * scoreScale;
        carUpgradesManager.UpgradeScoring(carScript.speed * Time.deltaTime * scoreScale);
    }

    [ContextMenu("RemoveObstaclesNearPlayer")]
    public void RemoveObstaclesNearPlayer()
    {
        Vector3 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

        GameObject[] GOs = GameObject.FindGameObjectsWithTag("Obstacle");

        XizukiMethods.GameObjects.Xi_Helper_GameObjects.FilterOutWithScript<GameObject>(ref GOs, CheckPosition);
        

        void CheckPosition(GameObject GO)
        {
            if ((GO.transform.position - playerPosition).magnitude < obstacleDestroyDistance) Destroy(GO);
        }
    }



    public void UIUpdate()
    {
        if (state == GameState.Playing)
        {

        }
        else if (state == GameState.Interval)
        {

        }

    }

    public void IntervalCamera()
    {
        if (state == GameState.Interval)
        {
            CameraPivot.transform.Rotate(new Vector3(0, 1, 0) * IntervalRotationSpeed * Time.deltaTime);
        }
        else if (state == GameState.Playing)  
        {

            if (Xi_Helper_Math.EstimatedEqual(CameraPivot.transform.localEulerAngles.y, 0, EstimatedEqualType.Range, 10))
            {
                CameraPivot.transform.localEulerAngles = new Vector3(0, 0, 0);

            }
            else
            {
                float dir = (CameraPivot.transform.localEulerAngles.y - 180);
                dir /= Mathf.Abs(dir);

                CameraPivot.transform.Rotate(new Vector3(0, 8.5f, 0) * dir * IntervalRotationSpeed * Time.deltaTime);

            }

        }
    }


    public void SetState(GameState newState)
    {
        if (state == newState) return;
        state = newState;

        if(state == GameState.Playing)
        {
            uiManager.gameCanvas.SetActive(true);
            uiManager.intervalCanvas.SetActive(false);
            currentSession++;

            MusicManager.instance.RandomizeMusic(ref carScript);
        }
        else if(state == GameState.Interval)
        {
            carUpgradesManager.adjustedUpgradeScore = 0;

            uiManager.gameCanvas.SetActive(false);
            uiManager.intervalCanvas.SetActive(true);
            scores[currentSession] = Mathf.RoundToInt(scores[currentSession]);
            ProgressBarUI();
        }
    }

    public void ProgressBarUI()
    {
        //print("ProgressBarUI");

        highestScoreOfSession = scores.Max();

        for (int i = 0; i <= currentSession; i++) 
        {
            uiManager.progressBars[i].gameObject.SetActive(true);

            uiManager.progressBars[i].fillAmount = scores[i] / highestScoreOfSession;

            uiManager.progressBars[i].gameObject.
                GetComponentInChildren<TMP_Text>().text = scores[i].ToString();
        }
    }

    public void Pause()
    {
        uiManager.Pause(true);
        isPaused = true;
    }
    public void Resume()
    {
        uiManager.Pause(false);
        isPaused = false;
    }


    public void IntervalStart()
    {
        SetState(GameState.Interval);
    }

    public void IntervalEnd()
    {
        SetState(GameState.Playing);
    }


}
