using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CarPathRecorder : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform[] recordTransforms;
    List<Quaternion> rotationList;
    List<Vector3> positionList;
    [SerializeField] float recordInterval;

    float timer;

    private void Awake()
    {
        rotationList = new List<Quaternion>();
        positionList = new List<Vector3>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= recordInterval)
        {
            foreach (var item in recordTransforms)
            {
                rotationList.Add(item.rotation);
                positionList.Add(item.position);
            }
        }
    }

    void SaveScoresToFile()
    {


        string folderPath = Application.dataPath + "/F1RaceRecordedPath/";





        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, "TransformCount_"+recordTransforms.Length + "__"
                        + DateTime.Now.ToString("dd_MMM_yyyy__HH_mm_ss") + ".txt");


        int transformCount = recordTransforms.Length;

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < rotationList.Count;)
            {
                for (int j = 0; j < transformCount; j++)
                {

                }
            }
        }

        Debug.Log("Values saved to " + filePath);
    }
}
//writer.WriteLine(score);
