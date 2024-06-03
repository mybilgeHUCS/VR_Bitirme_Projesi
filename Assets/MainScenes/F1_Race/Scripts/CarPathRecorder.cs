using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarPathRecorder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform[] recordTransforms;
    //public Transform[] RecordTransforms { get => recordTransforms; }
    List<Quaternion> rotationList;
    List<Vector3> positionList;
    List<float> engineVolumeList;
    [SerializeField] float recordInterval;
    //public float RecordInterval { get => recordInterval; }

    AudioSource engineAudioSource;
    float timer;
    [SerializeField] bool canRecord = true;

    

    private void Awake()
    {
        engineAudioSource = GetComponent<AudioSource>();        
        rotationList = new List<Quaternion>();
        positionList = new List<Vector3>();
        engineVolumeList = new List<float>();
    }

    private void Update()
    {
        if(!canRecord){
            return;
        }
        
        timer += Time.deltaTime;
        if (timer >= recordInterval )
        {
            foreach (var item in recordTransforms)
            {
                rotationList.Add(item.rotation);
                positionList.Add(item.position);
                

            }
            engineVolumeList.Add(engineAudioSource.volume);
            timer = 0;
        }

        if(Input.GetKeyDown(KeyCode.KeypadMinus)){
            canRecord = false;
            SaveScoresToFile();
        }
    }

    void SaveScoresToFile()
    {


        string folderPath = Application.dataPath + "/F1RaceRecordedPath/";





        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, "TransformCount_"+recordTransforms.Length + "__" +
                        "Interval_" + recordInterval.ToString().Replace(".","_") + "__"
                        + DateTime.Now.ToString("dd_MMM_yyyy__HH_mm_ss") + ".txt");


        int transformCount = recordTransforms.Length;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(recordInterval);
            foreach (var item in recordTransforms)
            {
                writer.WriteLine(item.name);
            }
            writer.WriteLine("START");

            string oneInterval = "";

            for (int i = 0; i < rotationList.Count;i++)
            {
                if(i % transformCount == 0 && i != 0){
                    //Debug.LogError(((i/rotationList.Count)-1)  + " " + engineVolumeList.Count);
                    writer.WriteLine(oneInterval + engineVolumeList[(i/transformCount)-1]);
                    oneInterval = "";
                }
                oneInterval += positionList[i].ToString() + " " + rotationList[i].ToString() + "\t";
            }
        }

        Debug.Log("Values saved to " + filePath);
    }
}
//writer.WriteLine(score);
