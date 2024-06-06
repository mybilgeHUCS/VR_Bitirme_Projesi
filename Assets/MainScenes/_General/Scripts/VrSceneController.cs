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
    public bool canEvaluate = true;



    public bool canPauseOrResume = true;




    public GameObject pauseUI;
    int cyberSicknessValue = 0;
    public int maxCyberSicknessValue = 5;
    public TextMeshProUGUI cyberSicknessValueText;
    public float axisThreshold = 0.5f;

    
    

    private InputData _inputData;
    public AudioSource[] audios;

    [SerializeField] Image visualWarningImage;
    [SerializeField] float visualWarningBlinkingTime  = 1f;
    float targetAlpha = 1f;
    float dir = 1f;

    [SerializeField ] AudioSource warningAudioSource;
    [SerializeField ] AudioClip warningAudio;
    [SerializeField] float cyberSicknessValueDetectInterval = 2f;
    float timerCSVDI = 0;

    [SerializeField] float cyberSicknessInputWarningInterval = 5f;
    float lastCSValueGiven = 0;

    bool isWarningAlreadyGiving = false;

    bool isVisualWarningEnabled = false;
    bool isSoundWarningEnabled = false;
    bool isCyberSicknessValueDetectEnabled = false;

    List<int> cyberSicknessValueList;

    private void Awake() {
        cyberSicknessValueList = new List<int>();
        _inputData = GetComponent<InputData>();
    }
    private void Start()
    {
        visualWarningImage.enabled = false;
        audios = GameObject.FindObjectsOfType<AudioSource>();
        cyberSicknessValueDetectInterval = PlayerPrefs.GetFloat("DataInputIntervalSlider");
        cyberSicknessInputWarningInterval = PlayerPrefs.GetFloat("WarningIntervalSlider");
        isVisualWarningEnabled = PlayerPrefs.GetFloat("VisualWarningToggle") ==1f ? true : false;
        isSoundWarningEnabled = PlayerPrefs.GetFloat("SoundWarningToggle") ==1f ? true : false;
        isCyberSicknessValueDetectEnabled = PlayerPrefs.GetFloat("DataInputToggle") ==1f ? true : false;


        cyberSicknessValueText.enabled = isCyberSicknessValueDetectEnabled;

        if(SceneManager.GetActiveScene().name == "MainMenu") {
            MainMenuVrControllerSettings();
        }
    }

    void MainMenuVrControllerSettings(){
        warningAudioSource.enabled = false;
        warningAudioSource.volume = 0;
        cyberSicknessValueText.enabled = false;
        canPauseOrResume = false;
        canEvaluate = false;
        pauseUI.SetActive(false);
        visualWarningImage.gameObject.SetActive(false);
    }


    private void Update()
    {

        //VisualWarningImageBlinking();

        _inputData._leftController.TryGetFeatureValue(CommonUsages.menuButton, out bool isLeftMenuButtonPressed);

        _inputData._leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isLeftSecondaryButtonPressed);

        //Debug.Log(isMenuButtonPressed);

        _inputData._leftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 leftControllerAxis);
        
        //Debug.Log("leftControllerAxis " + leftControllerAxis);

        if ((Input.GetKeyDown(KeyCode.Escape) || isLeftMenuButtonPressed) && canPauseOrResume)
        {
            /*if(canPause){
                canPause = false;
                Pause();
            }
            else{
                canPause = true;
                Resume();
            }*/

            if(gameIsPaused){
                Resume();
            }
            else{
                Pause();
            }
            canPauseOrResume = false;
            
        }

        if (!isLeftMenuButtonPressed || Input.GetKeyUp(KeyCode.Escape))
        {
            canPauseOrResume = true;
        }

        if (isLeftSecondaryButtonPressed|| Input.GetKeyDown(KeyCode.PageDown))
        {
            Menu();
        }
        





        if(!isCyberSicknessValueDetectEnabled){
            return;
        }


        if (Input.GetKeyDown(KeyCode.UpArrow) || (leftControllerAxis.y >= axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Min(maxCyberSicknessValue, cyberSicknessValue+1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false; 
            lastCSValueGiven = Time.time;
            visualWarningImage.enabled = false;
            //isWarningAlreadyGiving = false;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || (leftControllerAxis.y <= -axisThreshold && canEvaluate))
        {
            cyberSicknessValue = Math.Max(0, cyberSicknessValue - 1);
            cyberSicknessValueText.text = cyberSicknessValue.ToString();
            canEvaluate = false;
            lastCSValueGiven = Time.time;
            visualWarningImage.enabled = false;
            //isWarningAlreadyGiving = false;
        }

        if ( leftControllerAxis.y >= -axisThreshold/2f && leftControllerAxis.y <= axisThreshold/2f)
        {
            canEvaluate = true;
        }


        
        VisualWarningImageBlinking();



        timerCSVDI += Time.deltaTime;
        if(timerCSVDI >= cyberSicknessValueDetectInterval){
            if(!gameIsPaused && isCyberSicknessValueDetectEnabled){
                timerCSVDI = 0;
                cyberSicknessValueList.Add(cyberSicknessValue);
            }
        }

        if(Time.time >= cyberSicknessInputWarningInterval  + lastCSValueGiven){
            if(!gameIsPaused  && isCyberSicknessValueDetectEnabled){
                //isWarningAlreadyGiving = true;
                lastCSValueGiven = Time.time;
                GiveWarning();
            }
        }

    }

    void VisualWarningImageBlinking(){
        if(!isVisualWarningEnabled){
            return;
        }
        float alpha = Mathf.Lerp(visualWarningImage.color.a, targetAlpha, Time.deltaTime * visualWarningBlinkingTime);
        Color color = visualWarningImage.color;
        color.a = alpha;
        visualWarningImage.color = color;
        if (Mathf.Abs(alpha - targetAlpha) < 0.01f)
        {
            targetAlpha = targetAlpha == 0f ? 1f : 0f;
        }       
    }

    void GiveWarning(){
        if(isSoundWarningEnabled){
            warningAudioSource.PlayOneShot(warningAudio);
        }
        if(isVisualWarningEnabled){
            visualWarningImage.enabled = true;
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

    private void OnDestroy() {
        SaveScoresToFile();
    }

    void SaveScoresToFile()
    {

        if(SceneManager.GetActiveScene().name == "MainMenu" || !isCyberSicknessValueDetectEnabled){
            return;
        }


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
