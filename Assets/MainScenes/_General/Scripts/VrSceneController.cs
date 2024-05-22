using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;

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
    [SerializeField ] AudioSource warningAudioSource;
    [SerializeField ] AudioClip warningAudio;
    [SerializeField] float cyberSicknessValueDetectInterval = 2f;
    float timerCSVDI = 0;

    [SerializeField] float cyberSicknessInputWarningInterval = 5f;
    float lastCSValueGiven = 0;

    bool isWarningAlreadyGiving = false;

    List<int> cyberSicknessValueList;

    private void Awake() {
        cyberSicknessValueList = new List<int>();
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
            lastCSValueGiven = Time.time;
            isWarningAlreadyGiving = false;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || (leftControllerAxis.y <= -axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Max(0, cyberSicknessValue - 1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false;
            lastCSValueGiven = Time.time;
            isWarningAlreadyGiving = false;
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

        timerCSVDI += Time.deltaTime;
        if(timerCSVDI >= cyberSicknessValueDetectInterval){
            if(!gameIsPaused){
                timerCSVDI = 0;
                cyberSicknessValueList.Add(cyberSicknessValue);
            }
        }


        if(Time.time >= cyberSicknessInputWarningInterval  + lastCSValueGiven){
            if(!gameIsPaused && !isWarningAlreadyGiving){
                //isWarningAlreadyGiving = true;
                lastCSValueGiven = Time.time;
                GiveWarning();
            }
        }

    }

    void GiveWarning(){
        warningAudioSource.PlayOneShot(warningAudio);
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

    private void OnDestroy() {
        SaveScoresToFile();
    }

    void SaveScoresToFile()
    {
        string folderPath = Application.dataPath + "/CyberSicknessData/"+ SceneManager.GetActiveScene().name ;

        

        

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, SceneManager.GetActiveScene().name + "__"
                        + DateTime.Now.ToString("dd_MMM_yyyy__HH_mm_ss")  +".txt");


        int total = 0;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Interval: " + cyberSicknessValueDetectInterval + " seconds" );
            foreach (int score in cyberSicknessValueList)
            {
                total += score;
                writer.WriteLine(score);
            }
            //total /= cyberSicknessValueList.Count;
            writer.WriteLine("Average: " + total/(float) cyberSicknessValueList.Count);
        }

        Debug.Log("Values saved to " + filePath);
    }
}
