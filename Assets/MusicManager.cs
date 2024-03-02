using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioClip[] musics;

    private void Awake()
    {
        instance = this;
    }

    public void RandomizeMusic(ref CarScript carScript)
    {
        int randomIndex = Random.Range(0, musics.Length);
        AudioClip randomClip = musics[randomIndex];

        carScript.carFeedbackScript.musicSource.clip = randomClip;
        carScript.carFeedbackScript.musicSource.Play();
    }
}
