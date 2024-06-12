// Description: SaveManager: Load and save data
using UnityEngine;
using System.IO;
using System;

namespace TS.Generics {
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        public bool SeeInspector;

        public int WhichDataType = 0;

        public string saveName = "";

        [Serializable]
        public class DataClass
        {
            public string data;
        }

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        //--> Save Datas in a .dat file
        public void saveDAT(string s_ObjectsDatas, string s_fileName)
        {
            #region
            if(WhichDataType == 0)  // .Dat
            {
                DataClass dataObject = new DataClass();
                dataObject.data = s_ObjectsDatas;
                string json = JsonUtility.ToJson(dataObject);
                File.WriteAllText(Application.persistentDataPath + "/" + s_fileName + ".dat", json);
            }
            else // PlayerPrefs
            {
                PlayerPrefs.SetString(s_fileName, s_ObjectsDatas);
            }
           
            //Debug.Log("Save Complete");
            #endregion
        }

        public bool saveAndReturnTrueAFterSaveProcess(string s_ObjectsDatas, string s_fileName)
        {
            #region
            if (WhichDataType == 0)  // .Dat
            {
                DataClass dataObject = new DataClass();
                dataObject.data = s_ObjectsDatas;

                string json = JsonUtility.ToJson(dataObject);
                File.WriteAllText(Application.persistentDataPath + "/" + s_fileName + ".dat", json);
            }
            else // PlayerPrefs
            {
                PlayerPrefs.SetString(s_fileName, s_ObjectsDatas);
            }

            return true;
            #endregion
        }

        //--> Load datas from a .dat file
        public string LoadDAT(string s_fileName)
        {
            #region
            if (WhichDataType == 0)  // .Dat
            {
                if (File.Exists(Application.persistentDataPath + "/" + s_fileName + ".dat"))
                {
                    string result;
                    string dat = File.ReadAllText(Application.persistentDataPath + "/" + s_fileName + ".dat");
                    DataClass dataObject = JsonUtility.FromJson<DataClass>(dat);
                    result = dataObject.data;

                    return result;
                }
                else
                    return "";
            }
            else // PlayerPrefs
            {
                if (PlayerPrefs.HasKey(s_fileName))
                {
                    return PlayerPrefs.GetString(s_fileName);
                }
                else
                    return "";
            }
            #endregion
        }

        [System.Serializable]
        public class sData
        {
            public string _s;
        }

    }
}
