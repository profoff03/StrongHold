using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    private static readonly string FirstPlay = "FirstPlay";
    private static readonly string MusicPref = "MusicPref";
    private static readonly string SoundEffectsPref = "SoundEffectsPref";

    private int FirstPlayInt;
    public Slider musicSlider, soundEffectSlider;
    private float musicFloat, soundEffectsFloat;
    public AudioMixerGroup musicMixerGroup;

    // Start is called before the first frame update
    void Start()
    {
        FirstPlayInt = PlayerPrefs.GetInt(FirstPlay);
        if(FirstPlayInt == 0)
        {
            musicFloat = 0.25f;
            soundEffectsFloat = 0.75f;
            musicSlider.value = musicFloat;
            soundEffectSlider.value = soundEffectsFloat;
            PlayerPrefs.SetFloat(MusicPref, musicFloat);
            PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsFloat);
            PlayerPrefs.SetInt(FirstPlay, - 1);
        } else
        {
            musicFloat = PlayerPrefs.GetFloat(MusicPref, musicFloat);
            musicSlider.value = musicFloat;
            soundEffectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
            soundEffectSlider.value = soundEffectsFloat;
        }
    }

    public void SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(MusicPref, musicSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectSlider.value);
    }

    private void OnApplicationFocus(bool InFocus)
    {
        if(!InFocus)
        {
            SaveSoundSettings();
        }
    }

    public void UpdateSound()
    {
        musicMixerGroup.audioMixer.SetFloat("Music", Mathf.Lerp(-50, 0 , musicSlider.value));
        musicMixerGroup.audioMixer.SetFloat("Effect", Mathf.Lerp(-50, 0, soundEffectSlider.value));

        
    }


}
