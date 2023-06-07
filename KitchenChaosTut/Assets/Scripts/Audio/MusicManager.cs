using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private const string PlayerRefsMusicVolume = "MusicVolume";


    public static MusicManager Instance
    {
        get;
        private set;
    }

    // 由於是循環播放
    private AudioSource audioSource;
    private float volume;

    private void Awake() 
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        volume = PlayerPrefs.GetFloat(PlayerRefsMusicVolume, 0.3f);
        audioSource.volume = volume;
    }

    public void ChangeVolume()
    {
        volume += 0.1f;

        if(volume > 1f)
            volume = 0f;

        audioSource.volume = volume;

        PlayerPrefs.SetFloat(PlayerRefsMusicVolume, volume);
        PlayerPrefs.Save();
    }
    public float GetVolume()
    {
        return volume;
    }
}
