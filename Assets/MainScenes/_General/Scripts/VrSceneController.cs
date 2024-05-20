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

    private void Start()
    {
        _inputData = GetComponent<InputData>();
    }


    private void Update()
    {
        _inputData._leftController.TryGetFeatureValue(CommonUsages.menuButton, out bool isMenuButtonPressed);

        Debug.Log(isMenuButtonPressed);

        _inputData._leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 rightAxis);
        
        //Debug.Log("right axis " + rightAxis);
        






        if (Input.GetKeyDown(KeyCode.UpArrow) || (rightAxis.y >= axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Min(maxCyberSicknessValue, cyberSicknessValue+1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false; 
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || (rightAxis.y <= -axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Max(0, cyberSicknessValue - 1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false;
        }


        if ( rightAxis.y >= -axisThreshold/2f && rightAxis.y <= axisThreshold/2f)
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
    }

    public void Resume()
    {
        Debug.Log("Resume");
        pauseUI.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    public void Menu()
    {
        Resume();
        SceneManager.LoadScene("MainMenu");
    }
}
