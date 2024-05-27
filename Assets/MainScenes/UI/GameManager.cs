using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
        Debug.Log("Uçak sahnesi açılıyor");
        SceneManager.LoadScene("PlaneScene");
    }

    public void LoadRaceScene()
    {
        SceneManager.LoadScene("F1_RaceScene");
    }

}

