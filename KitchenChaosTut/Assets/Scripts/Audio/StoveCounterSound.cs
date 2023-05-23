using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource audioSource;

    private void Awake() 
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start() 
    {
        stoveCounter.OnFoodStateChanged += StoveCounter_OnFoodStateChanged;
    }

    private void StoveCounter_OnFoodStateChanged(object sender, StoveCounter.OnFoodStateChangedEventArgs e)
    {
        bool playSound = e.foodState == StoveCounter.FoodState.Frying || e.foodState == StoveCounter.FoodState.Fried;

        if(playSound)
            audioSource.Play();
        else
            audioSource.Pause();
    }
}
