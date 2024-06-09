using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager instance;

    public float soundVolume;
    public float musicVolume;
    public int difficulty;
    private int prevDifficulty;

    public float obstacleDestroyDistance;

    public int currentGraphics;


    public Slider graphicsSlider;
    public Slider soundSlider;
    public Slider musicSlider;
    public Slider difficultySlider;


    // Start is called before the first frame update
    void Start()
    {
        LoadPlayerPrefs();
        XizukiMethods.GameObjects.Xi_Helper_GameObjects.MonoInitialization<OptionsManager>(ref instance, this);
    }

    public void LoadPlayerPrefs()
    {
        currentGraphics = PlayerPrefs.GetInt("Quality", 3);
        soundVolume = PlayerPrefs.GetFloat("Sound", 0.5f);
        musicVolume = PlayerPrefs.GetFloat("Music", 0.5f);
        difficulty = PlayerPrefs.GetInt("Difficulty", 1);


        graphicsSlider.value = currentGraphics;
        soundSlider.value = soundVolume;
        musicSlider.value = musicVolume;
        difficultySlider.value = difficulty;


        QualitySettings.SetQualityLevel(currentGraphics);


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GraphicsSlider(Slider slider)
    {
        currentGraphics = (int)slider.value;
        QualitySettings.SetQualityLevel(currentGraphics);
        PlayerPrefs.SetInt("Quality", currentGraphics);

    }

    [ContextMenu("Change Graphics")]
    public void ChangeGraphics()
    {
        //GraphicsSettings.defaultRenderPipeline = graphicSettingsRenderPipeLines[currentGraphics];
        QualitySettings.SetQualityLevel(currentGraphics);
    }

    public void SoundSlider(Slider slider)
    {
        soundVolume = slider.value;
        PlayerPrefs.SetFloat("Sound", soundVolume);

    }
    public void MusicSlider(Slider slider)
    {
        musicVolume = slider.value;
        PlayerPrefs.SetFloat("Music", musicVolume);

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

        PlayerPrefs.SetInt("Difficulty", difficulty);

        GameManager.instance.RemoveObstaclesNearPlayer();

    }



    public void ExitGame()
    {
        Application.Quit();
    }
}


