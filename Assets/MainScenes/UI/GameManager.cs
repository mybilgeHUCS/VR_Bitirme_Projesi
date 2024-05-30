using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    [SerializeField] Toggle visualWarning;
    [SerializeField] Toggle soundWarning;
    [SerializeField] Toggle dataInput;
    [SerializeField] Slider warningInterval;
    [SerializeField] Slider dataInputInterval;


    private void OnEnable() {
        LoadOptions();
    }

    public void OnToggleChanged(Toggle t){
        
        PlayerPrefs.SetFloat(t.name, t.isOn ? 1f : 0f);
        if(t.name == "DataInputToggle" && !t.isOn){
            visualWarning.isOn = false;
            soundWarning.isOn = false;
        }
        if(t.name != "DataInputToggle" && !dataInput.isOn && t.isOn){
            t.isOn = false;
        }
    }
    public void OnSliderChanged(Slider s){
        PlayerPrefs.SetFloat(s.name, s.value);
    }

    void LoadOptions(){
        visualWarning.isOn = PlayerPrefs.GetFloat(visualWarning.name) == 1f ? true:false;
        soundWarning.isOn = PlayerPrefs.GetFloat(soundWarning.name) == 1f ? true:false;
        dataInput.isOn = PlayerPrefs.GetFloat(dataInput.name) == 1f ? true:false;

        warningInterval.value = PlayerPrefs.GetFloat(warningInterval.name);
        dataInputInterval.value = PlayerPrefs.GetFloat(dataInputInterval.name);
        
    }

    public void LoadLunaparkScene(int CameraNumber)
    {
        PlayerPrefs.SetInt("CameraNumber", CameraNumber);

        SceneManager.LoadScene("RollerCoasterScene");
    }

    public void Load360Scene(int selectedVideo)
    {
        PlayerPrefs.SetInt("SelectedVideoIndex", selectedVideo);

        SceneManager.LoadScene("360VideoPlayerScene");
    }

    public void LoadBoatScene()
    {
        SceneManager.LoadScene("BoatTripScene");
    }

    public void LoadAuthoPathPlaneScene()
    {
        SceneManager.LoadScene("PlaneAutoPath");
    }

    public void LoadFlightScene()
    {
        SceneManager.LoadScene("PlaneNormal");
    }

    public void LoadRaceScene()
    {
        SceneManager.LoadScene("F1_RaceScene");
    }

}

