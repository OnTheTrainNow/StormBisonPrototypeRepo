using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class volumeControl : MonoBehaviour
{
    [SerializeField] string volumeParameter = "MasterVolume";
    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider slider;
    [SerializeField] float multiplier = 30f;
    [SerializeField] private Toggle toggle;
    private bool disableToggleEvent;

    private void Awake()
    {
        if (slider && toggle)
        {
            slider.onValueChanged.AddListener(HandleSliderValueChanged);
            toggle.onValueChanged.AddListener(HandleToggleValueChanged);
        } 
        else if(slider && toggle == null)
        {
            
        }
    }

    void Start()
    {
        slider.value = PlayerPrefs.GetFloat(volumeParameter, slider.value);
    }

    private void HandleToggleValueChanged(bool enableSound)
    {
        if (disableToggleEvent)
        {
            return;
        }
        if (enableSound)
        {
            slider.value = slider.maxValue;
        }
        else
        {
                slider.value = slider.minValue;
        }
    }

    private void HandleSliderValueChanged(float value)
    {
        mixer.SetFloat(volumeParameter, value:Mathf.Log10(value) * multiplier);
        disableToggleEvent = true;
        toggle.isOn = slider.value > slider.minValue;
        disableToggleEvent = false;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(volumeParameter, slider.value);
    }
}
