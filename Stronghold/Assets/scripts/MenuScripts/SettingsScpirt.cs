using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SettingsScpirt : MonoBehaviour
{
    
    [SerializeField]
    TMP_Dropdown ResolutionDropdown;
    [SerializeField]
    TMP_Dropdown QualityDropdown;

    Resolution[] resolutions;
    // Start is called before the first frame update
    void Start()
    {
        ResolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int CurrentResolutionIndex = 0;

       for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height + " " + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            if(resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                CurrentResolutionIndex = i;
            }
        }

        ResolutionDropdown.AddOptions(options);
        ResolutionDropdown.RefreshShownValue();
        LoadSettings(CurrentResolutionIndex);

    }

    public void SetFoolScreen(bool IsFullScreen)
    {
        Screen.fullScreen = IsFullScreen;
    }

    public void SerResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int QualityIndex)
    {
        QualitySettings.SetQualityLevel(QualityIndex);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingsPreference", QualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", ResolutionDropdown.value);
        PlayerPrefs.SetInt("FullScreenPreference", System.Convert.ToInt32(Screen.fullScreen));
    }

    private void OnApplicationFocus(bool InFocus)
    {
        if (!InFocus)
        {
            SaveSettings();
        }
    }

    public void LoadSettings(int CurrentResolutionIndex)
    {
        if(PlayerPrefs.HasKey("QualitySettingsPreference"))
        {
            QualityDropdown.value = PlayerPrefs.GetInt("QualitySettingsPreference");
        } else
        {
            QualityDropdown.value = 3;
        }

        if(PlayerPrefs.HasKey("ResolutionPreference"))
        {
            ResolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        } else
        {
            ResolutionDropdown.value = CurrentResolutionIndex;
        }

        if (PlayerPrefs.HasKey("FullScreenPreference"))
        {
            Screen.fullScreen = System.Convert.ToBoolean(PlayerPrefs.GetInt("FullScreenPreference"));
        }
        else
        {
            Screen.fullScreen = true;
        }
    }
    
}
