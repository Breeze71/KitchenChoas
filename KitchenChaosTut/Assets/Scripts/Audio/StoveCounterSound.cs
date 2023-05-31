using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;
    private float warningSoundTimer;
    private bool isPlaywarningSound;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }
    private void Start() 
    {
        stoveCounter.OnFoodStateChanged += StoveCounter_OnFoodStateChanged;
        
        stoveCounter.OnProgressBarChanged += stoveCounter_OnProgressBarChange;
    }

    // warning Sound
    private void stoveCounter_OnProgressBarChange(object sender, IHasProgress.OnProgressBarChangeEventArgs e)
    {
        float burnProgressAmount = .5f;

        isPlaywarningSound = stoveCounter.IsFried() && e.progressBarNormalized >= burnProgressAmount;
    }

    // Sisi Sound
    private void StoveCounter_OnFoodStateChanged(object sender, StoveCounter.OnFoodStateChangedEventArgs e)
    {
        bool playSound = e.foodState == StoveCounter.FoodState.Frying || e.foodState == StoveCounter.FoodState.Fried;

        if(playSound)
            audioSource.Play();
        else
            audioSource.Pause();
    }

    private void Update() 
    {
        if(!isPlaywarningSound)
            return;

        warningSoundTimer -= Time.deltaTime;
        if(warningSoundTimer <= 0f)
        {
            float warningSoundTimerMax = .5f;
            warningSoundTimer = warningSoundTimerMax;

            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
        }
    }
}
