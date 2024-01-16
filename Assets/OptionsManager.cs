using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class OptionsManager : MonoBehaviour
{
    public float soundVolume;
    public float musicVolume;
    public int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        
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

    public void ChangeDifficulty(int value)
    {
        difficulty = value;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}


