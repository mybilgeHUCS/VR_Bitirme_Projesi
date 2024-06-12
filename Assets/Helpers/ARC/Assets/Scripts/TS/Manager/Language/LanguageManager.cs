// Description: LanguageManager: Manage and manipulate multilanguage texts at runtime. Attached to LanguageManager.
// 
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics {
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager   instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
        public bool                     SeeInspector;
        public bool                     helpBox = true;

        public int currentLanguage;


        [System.Serializable]
        public class mLang
        {
            public List<string> s_Text = new List<string>();
        }

        public List<mLang>              mLangList = new List<mLang>();


        public globalTextDatas          _GlobalTextDatas;


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

        public bool Bool_InitLanguage()
        {
            if (PlayerPrefs.HasKey("LG"))
                currentLanguage = PlayerPrefs.GetInt("LG");
            else
                Bool_UpdateSelectedLanguage(0);

            //Debug.Log("Language Loaded: " + currentLanguage);
            return true;
        }

        public bool Bool_UpdateSelectedLanguage(int value, bool b_UpdateAllTexts = false)
        {
            PlayerPrefs.SetInt("LG", value);
            currentLanguage = value;
            if (b_UpdateAllTexts) Bool_UpdateAllTexts();

            return true;
        }


        //-> Return a text from a specific data file
        public string String_ReturnText(int _WhichTextData,int _Entry)
        {
            string sReturn = "";
            
            if (_GlobalTextDatas.textDatasList.Count > _WhichTextData &&
                _GlobalTextDatas.textDatasList[_WhichTextData].TextsList.Count > _Entry &&
                _GlobalTextDatas.textDatasList[_WhichTextData].TextsList[_Entry].multiLanguage.Count > currentLanguage)
                sReturn = _GlobalTextDatas.textDatasList[_WhichTextData].TextsList[_Entry].multiLanguage[currentLanguage];
            else
                sReturn = "Error: Text Doesn't Exist";

            return sReturn;
        }


        //-> Update all Texts in scene view
        public bool Bool_UpdateAllTexts()
        {
            //Debug.Log("Text Init Starts");
            var _Canvas = FindObjectsOfType(typeof(Canvas));

            foreach (Canvas child in _Canvas)
            {
                CurrentText[] allTexts = child.GetComponentsInChildren<CurrentText>(true);

                foreach (CurrentText txtChild in allTexts)
                {
                    if(txtChild.bUpdateDependingLanguage)
                        txtChild.UpdateText();  
                }
            }
            //Debug.Log("Text Init Ended");
            return true;
        }
    }
}
