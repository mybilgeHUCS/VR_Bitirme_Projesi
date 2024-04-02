using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VrSceneController : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseUI;
    public GameObject sliderObj;
    public Slider slider;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            slider.value += 1f;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            slider.value -= 1f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        pauseUI.SetActive(true);
        sliderObj.SetActive(false);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void Resume()
    {
        pauseUI.SetActive(false);
        sliderObj.SetActive(true);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
