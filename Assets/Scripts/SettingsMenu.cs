using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;
    public TMP_Dropdown resDropdown;
    public Toggle FullScreenToggle;

    Resolution[] AllResolutions;
    bool IsFullScreen;
    int SelectedResolution;
    List<Resolution> SelectedResolutionList = new List<Resolution>();

    public void ChangeGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
    // Start is called before the first frame update
    void Start()
    {
        IsFullScreen = true;
        AllResolutions = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRes;
        foreach (Resolution res in AllResolutions)
        {
            newRes = res.width.ToString() + " x " + res.height.ToString();
            if (!resolutionStringList.Contains(newRes))
            {
                resolutionStringList.Add(newRes);
                SelectedResolutionList.Add(res);
            }
            
        }
        resDropdown.AddOptions(resolutionStringList);
        
    }

    public void ChangeResolution()
    {
        SelectedResolution = resDropdown.value;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }

    public void ChangeFullScreen()
    {
        IsFullScreen = FullScreenToggle.isOn;
        Screen.SetResolution(SelectedResolutionList[SelectedResolution].width, SelectedResolutionList[SelectedResolution].height, IsFullScreen);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
