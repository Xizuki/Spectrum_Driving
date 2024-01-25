using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject pauseCanvas;
    public GameObject intervalCanvas;

    public Image[] progressBars;

    public GameObject gameCanvas;
    public TMP_Text score_txt;
    private string score_txtPrefix;

    public Slider progressionUI;

    [ContextMenu("RandomizeMap()")]
    public void RandomizeMap()
    {

    }

    private void Awake()
    {
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<UIManager>(ref Instance, this);

        //print("Instance = " + Instance);

        GameObject.FindGameObjectWithTag("score_txt").GetComponent<TMP_Text>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        score_txtPrefix = score_txt.text;
    }

    public void Pause(bool state)
    {
        pauseCanvas.SetActive(state);
        if (state)
        {
            Time.timeScale = 0;
        }
        else
        {
            OptionsManager.instance.UnPausedDifficulty();
            Time.timeScale = 1;
        }
    }

    public void IntervalStart()
    {
        intervalCanvas.SetActive(true);
    }

    public void IntervalEnd()
    {
        intervalCanvas.SetActive(false);
    }

    public void UpdateScore(float amount)
    {
        score_txt.text = score_txtPrefix + amount.ToString();
    }

    public void UpdateProgressionUI(float tValue)
    {
        progressionUI.value = tValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
