using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarRecordedPathReader : MonoBehaviour
{
    [SerializeField] bool canRead = false;
    [SerializeField] float playSpeed = 1f;
    [SerializeField] TextAsset recordedPath;

    float globalSpeedMultiplier;

    int totalFrameCount;
    float recordInterval;
    public List<Transform> recordTransforms;

    float timer = 0;
    int playFrameIndex = 0;
    int recordTransformsLength;


    List<Quaternion> rotationList;
    List<Vector3> positionList;
    List<float> engineVolumeList;

    
    Quaternion[] targetRotationArr ;
    Vector3[] targetPositionArr;
    float targetEngineVolume;

    Quaternion[] initRotationArr ;
    Vector3[] initPositionArr;
    float initEngineVolume;

    AudioSource engineAudioSource;

    Vector3 steerInitLocalPos;

    private void Start() {

        if(!canRead){
            return;
        }

        globalSpeedMultiplier = PlayerPrefs.GetFloat("SpeedMultiplierSlider");

        engineAudioSource = GetComponent<AudioSource>();  
        GetComponent<CarController>().enabled = false;
        GetComponent<Rigidbody>().isKinematic = true;

        recordTransforms = new List<Transform>();
        rotationList = new List<Quaternion>();
        positionList = new List<Vector3>();
        engineVolumeList = new List<float>();

        
        ReadRecordedPath();
        
        steerInitLocalPos = recordTransforms[1].localPosition;
        Debug.Log(rotationList.Count);
        Debug.Log(positionList.Count);
        Debug.Log(engineVolumeList.Count);
        recordTransformsLength = recordTransforms.Count;

        targetRotationArr = new Quaternion[recordTransformsLength];
        targetPositionArr = new Vector3[recordTransformsLength];

        initRotationArr = new Quaternion[recordTransformsLength];
        initPositionArr = new Vector3[recordTransformsLength];

        for (int i = 0; i < recordTransformsLength; i++)
        {
            initRotationArr[i] = recordTransforms[i].rotation;
            initPositionArr[i] = recordTransforms[i].position;
            targetRotationArr[i] = recordTransforms[i].rotation;
            targetPositionArr[i] = recordTransforms[i].position;
        }
        

    }

    


    

    private void Update() {
        if(!canRead){
            return;
        }
        timer+= Time.deltaTime* Math.Abs(playSpeed)*globalSpeedMultiplier;
        if(timer >=  recordInterval){



            for (int i = 0; i < recordTransformsLength; i++)
            {
                //Debug.Log(positionList[i + playFrameIndex*recordTransformsLength] + " " + rotationList[i + playFrameIndex*recordTransformsLength]);
                //recordTransforms[i].position = positionList[i + playFrameIndex*recordTransformsLength];
                //recordTransforms[i].rotation = rotationList[i + playFrameIndex*recordTransformsLength];
                targetPositionArr[i] = positionList[i + playFrameIndex*recordTransformsLength];
                targetRotationArr[i] = rotationList[i + playFrameIndex*recordTransformsLength];

                initRotationArr[i] = recordTransforms[i].rotation;
                initPositionArr[i] = recordTransforms[i].position;
                
            }
           
            //engineAudioSource.volume = engineVolumeList[playFrameIndex];
            targetEngineVolume = engineVolumeList[playFrameIndex];
            initEngineVolume = engineAudioSource.volume;

            playFrameIndex += (int)(Math.Sign(playSpeed)*timer/recordInterval);

           // Debug.Log(positionList.Count + " " + recordTransformsLength);





            playFrameIndex %= totalFrameCount;
            if(playFrameIndex<0){
                playFrameIndex+= totalFrameCount;
            }
            timer = 0;



            
        }
        else{
            for (int i = 0; i < recordTransformsLength; i++)
            {
                
                recordTransforms[i].position = Vector3.Lerp(initPositionArr[i], targetPositionArr[i], timer/recordInterval);
                recordTransforms[i].rotation = Quaternion.Lerp(initRotationArr[i], targetRotationArr[i], timer/recordInterval);
            }
            engineAudioSource.volume = Mathf.Lerp(initEngineVolume, targetEngineVolume, timer/recordInterval);
        }


        recordTransforms[1].localPosition = steerInitLocalPos;



    }

    void ReadRecordedPath(){

        //Debug.Log(recordedPath.text);

        string[] lines = recordedPath.text.Split("\n");

        recordInterval = float.Parse(lines[0]);

        int index = 1;
        while(lines[index].Trim() != "START"){

                //Debug.Log(index  + " "+lines.Length + " "+ lines[index]);
                GameObject found = GameObject.Find(lines[index].Trim());
                if(found != null){
                    //Debug.Log(lines[index]);
                    //Debug.Log(found.name + " " +found);
                    recordTransforms.Add(found.transform);
                }

            
            index++;
        }

        bool aa = true;

        for (int i = index+1; i < lines.Length-1; i++)
        {
            totalFrameCount++;
            string line = lines[i];

            string[] transforms = line.Split("\t");
            //Debug.LogError(float.Parse( transforms[transforms.Length-1]) + " " + i);
            engineVolumeList.Add( float.Parse( transforms[transforms.Length-1].Replace(".",",")));
            transforms[transforms.Length-1] = "";
            //Debug.Log(line);

            foreach (var tf in transforms)
            {
                if(tf == ""){
                    continue;
                }

                string[] components = tf.Substring(1, tf.Length-2).Split(") (");
                
                string[] vec3 = components[0].Replace(", ","_ ").Replace(".",".").Split("_ ");
                string[] qua = components[1].Replace(", ","_ ").Replace(".",".").Split("_ ");

                positionList.Add(new Vector3(float.Parse(vec3[0]),float.Parse(vec3[1]),float.Parse(vec3[2])));
                rotationList.Add(new Quaternion(float.Parse(qua[0]),float.Parse(qua[1]),float.Parse(qua[2]),float.Parse(qua[3])));

                /*if(!aa){
                    return;
                }

                foreach (var item in vec3)
                {
                    Debug.Log(item);
                }

                foreach (var item in qua)
                {
                    Debug.Log(item);
                }

                aa = false;*/

            }
            

        }

    }
}
