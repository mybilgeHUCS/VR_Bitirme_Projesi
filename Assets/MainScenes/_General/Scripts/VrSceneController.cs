using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class VrSceneController : MonoBehaviour
{
    public static bool gameIsPaused = false;
    public GameObject pauseUI;
    int cyberSicknessValue = 0;
    public int maxCyberSicknessValue = 5;
    public TextMeshProUGUI cyberSicknessValueText;
    public float axisThreshold = 0.5f;

    public bool canEvaluate = true;
    public bool canPause = true;

    private InputData _inputData;
    public AudioSource[] audios;

    private void Awake() {
        _inputData = GetComponent<InputData>();
    }
    private void Start()
    {
         audios = GameObject.FindObjectsOfType<AudioSource>();
    }


    private void Update()
    {
        _inputData._leftController.TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuButtonPressed);

        //Debug.Log(isMenuButtonPressed);

        _inputData._leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftControllerAxis);
        
        //Debug.Log("leftControllerAxis " + leftControllerAxis);

        




        if (Input.GetKeyDown(KeyCode.UpArrow) || (leftControllerAxis.y >= axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Min(maxCyberSicknessValue, cyberSicknessValue+1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false; 
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || (leftControllerAxis.y <= -axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Max(0, cyberSicknessValue - 1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false;
        }


        if ( leftControllerAxis.y >= -axisThreshold/2f && leftControllerAxis.y <= axisThreshold/2f)
        {
            canEvaluate = true;
        }



        if (Input.GetKeyDown(KeyCode.Escape) || (isMenuButtonPressed && canPause))
        {
            canPause = false;
                Pause();
            
        }

        if (!isMenuButtonPressed)
        {
            canPause = true;
        }
    }

    void Pause()
    {
        Debug.Log("PAUSE");
        pauseUI.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;

        foreach (var item in audios)
        {
            item.Pause();
        }
    }

    public void Resume()
    {
        Debug.Log("Resume");
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        foreach (var item in audios)
        {
            item.UnPause();
        }
    }

    public void Menu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
