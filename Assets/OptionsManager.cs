using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    public float soundVolume;
    public float musicVolume;
    public int difficulty;
    private int prevDifficulty;

    public float obstacleDestroyDistance;
    // Start is called before the first frame update
    void Start()
    {
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<OptionsManager>(ref instance, this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SoundSlider(Slider slider)
    {
        soundVolume = slider.value;
    }
    public void MusicSlider(Slider slider)
    {
        musicVolume = slider.value;
    }

    public void ChangeDifficulty(Slider slider)
    {
        difficulty = (int)slider.value;
    }

    public void UnPausedDifficulty()
    {
        if (prevDifficulty == difficulty) return;

        ObstacleManager.instance.GenerateObstacles(difficulty);

        prevDifficulty = difficulty;

        GameManager.instance.RemoveObstaclesNearPlayer();
    }

 

    public void ExitGame()
    {
        Application.Quit();
    }
}


