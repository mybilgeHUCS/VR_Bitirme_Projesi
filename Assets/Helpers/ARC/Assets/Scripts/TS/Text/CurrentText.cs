// Description: CurrentText: Display text on Utext component.
// Work with Default Unity Text compnent and Text Mesh Pro component.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TS.Generics
{
    public class CurrentText : MonoBehaviour
    {
        public bool                 SeeInspector;
        public int                  tab; // 0: Text is always the same | 1: Text is manage by script

        public int                  _Entry;
        public int                  _WhichTextData;

        public List<TextEntry>      TextsList = new List<TextEntry>();

        public float                sentenceSpeedDuration = .01f;
        public float                sentenceHowManyLetter = 1; // THe number of letter displayed each sentenceSpeedDuration

        private AudioSource         aSource;

        // Text component attached to the gameobject
        private int                 whichTextComponent = -100;
        private TextMeshProUGUI     txtTMP;
        private Text                txtUD;

        public bool                 bBypass = false;

        public bool                 bUpdateDependingLanguage = true;

        // Start is called before the first frame update
        void Start()
        {
            if (!bBypass)
            {
                if (GetComponent<AudioSource>() != null)
                    aSource = GetComponent<AudioSource>();

                if (tab == 0)
                {
                    DisplayTextComponent(this.gameObject, LanguageManager.instance.String_ReturnText(_WhichTextData, _Entry));
                }
            }
           

        }
      
    
        //-> New multi-language text (Only one ID)
        public void NewTextWithSpecificID(int newID, int textFolder)
        {
            #region
            if (!bBypass)
            {
                tab = 0;
                _WhichTextData = textFolder;
                _Entry = newID ;

                DisplayTextComponent(this.gameObject, LanguageManager.instance.String_ReturnText(_WhichTextData, _Entry));
            }

            #endregion
        }

        //-> New multi-language text (Only one ID) displayed letter by letter
        public void NewTextWithSpecificID_LetterByLetter(int newID, int textFolder, AudioClip aClip = null, bool b_IsSentenceInProcess = true)
        {
            #region
            if (!bBypass)
            {
                tab = 0;
                _Entry = newID;
                _WhichTextData = textFolder;

                StartCoroutine(DisplayTextLetterByLetter(LanguageManager.instance.String_ReturnText(_WhichTextData, _Entry), aClip, b_IsSentenceInProcess));
            }

            #endregion
        }

        //-> New multiple texts multilanguage + variables
        public void NewTextManageByScript(List<TextEntry> newTextList, bool AutoSpaceBetweenTwoText = true)
        {
            #region
            if (!bBypass)
            {

            }
            tab = 1;
            this.gameObject.GetComponent<CurrentText>().TextsList = newTextList;
            UpdateText(AutoSpaceBetweenTwoText);
            #endregion
        }

        //-> New multiple texts multilanguage + variables displayed letter by letter
        public void NewTextManageByScript_LetterByLetter(List<TextEntry> newTextList, bool AutoSpaceBetweenTwoText = true, AudioClip aClip = null, bool b_IsSentenceInProcess = true)
        {
            #region
            if (!bBypass)
            {
                tab = 1;
                this.gameObject.GetComponent<CurrentText>().TextsList = newTextList;
                UpdateText(AutoSpaceBetweenTwoText, true, aClip, b_IsSentenceInProcess);
            }
           
            #endregion
        }

        //-> Update current Text
        public void UpdateText(bool AutoSpaceBetweenTwoText = true, bool bSentence = false,AudioClip aClip = null, bool b_IsSentenceInProcess = true)
        {
            #region
            if (!bBypass)
            {
                StopAllCoroutines();

                if (tab == 0)   // 0: Text is always the same
                {
                    if (!bSentence)
                    {
                        DisplayTextComponent(this.gameObject, LanguageManager.instance.String_ReturnText(_WhichTextData, _Entry));
                    }
                    else if (bSentence)
                    {
                        StartCoroutine(DisplayTextLetterByLetter(LanguageManager.instance.String_ReturnText(_WhichTextData, _Entry), aClip, b_IsSentenceInProcess));
                    }
                }
                else            // 1: Text is manage by script
                {
                    DisplayTextComponent(this.gameObject, "");

                    string newText = "";

                    for (var i = 0; i < TextsList.Count; i++)
                    {
                        if (TextsList[i].TextType == 0)
                        {
                            newText += LanguageManager.instance.String_ReturnText(TextsList[i].whichTextData, TextsList[i].whichTextID);
                        }
                        else
                        {
                            newText += TextsList[i].runtimeVariable;
                            //Debug.Log("Here we are -> " + newText);
                        }
                        if (i < TextsList.Count - 1 && AutoSpaceBetweenTwoText)
                            newText += " ";
                    }

                    if (!bSentence)
                    {
                        DisplayTextComponent(this.gameObject, newText);
                    }
                    else if (bSentence)
                    {
                        StartCoroutine(DisplayTextLetterByLetter(newText, aClip, b_IsSentenceInProcess));
                    } 
                }
            }
            
            #endregion
        }

        //-> Display a Text Letter By Letter
        public IEnumerator DisplayTextLetterByLetter(string TextToDisplay, AudioClip aClip = null, bool b_IsSentenceInProcess = true)
        {
            if (!bBypass)
            {
                InfoPlayerTS.instance.b_IsSentenceInProcess = b_IsSentenceInProcess;
                int currentLetter = 0;

                DisplayTextComponent(this.gameObject, "");

                float t = 0;
                float durationBetweenTwoLetters = sentenceSpeedDuration;

                float soundCount = 0;

                while (currentLetter < TextToDisplay.Length)
                {
                    if (!PauseManager.instance.Bool_IsGamePaused)
                    {
                        if (t < durationBetweenTwoLetters)
                        {
                            t = Mathf.MoveTowards(t, durationBetweenTwoLetters, Time.deltaTime);
                        }
                        else
                        {
                            if (TextToDisplay[currentLetter].ToString() == "|")
                            {
                                DisplayTextComponent(this.gameObject, "\n", true);
                            }

                            else
                            {
                                if (soundCount == 0 && aSource)
                                {
                                    if (aClip) aSource.clip = aClip;
                                    if (aSource.gameObject.activeInHierarchy)
                                        aSource.Play();
                                }

                                DisplayTextComponent(this.gameObject, TextToDisplay[currentLetter].ToString(), true);

                                for (var i = 0; i < sentenceHowManyLetter; i++)
                                {
                                    if (currentLetter + 1 < TextToDisplay.Length)
                                    {
                                        if (TextToDisplay[currentLetter + 1].ToString() == "|")
                                        {
                                            DisplayTextComponent(this.gameObject, "\n", true);
                                        }

                                        else
                                        {
                                            DisplayTextComponent(this.gameObject, TextToDisplay[currentLetter + 1].ToString(), true);
                                        }

                                        currentLetter++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                            soundCount++;
                            soundCount %= 2;
                            currentLetter++;
                            t = 0;
                        }
                    }
                    yield return null;
                }
                InfoPlayerTS.instance.b_IsSentenceInProcess = false;
            }

            yield return null;
        }

        //-> Display text depending the text component (Unity UI or Text Mesh Pro)
        public void DisplayTextComponent(GameObject refObjTxt, string txtToDisplay, bool b_LetterByLetter = false)
        {
            if (!bBypass)
            {
                // Debug.Log(refObjTxt + " -> " + txtToDisplay);
                // Find Text component attached to the gameobject
                if (whichTextComponent == -100)
                {
                    // Use Default Unity Text component
                    if (refObjTxt.GetComponent<Text>())
                    {
                        txtUD = refObjTxt.GetComponent<Text>();
                        whichTextComponent = 0;
                    }
                    else
                    {
                        txtTMP = refObjTxt.GetComponent<TextMeshProUGUI>();
                        whichTextComponent = 1;
                    }
                }

                if (b_LetterByLetter)
                {
                    if (whichTextComponent == 0) txtUD.text += txtToDisplay;
                    if (whichTextComponent == 1) txtTMP.text += txtToDisplay;
                }
                else
                {
                    if (whichTextComponent == 0) txtUD.text = txtToDisplay;
                    if (whichTextComponent == 1) txtTMP.text = txtToDisplay;
                }
            }
        }
    }

    [System.Serializable]
    public class TextEntry
    {
        #region
        public int TextType = 0; // 0: It is a text from the w_TextCreator (Multi language text) | 1: It is runtime variable

        // Multilanguage text
        public int whichTextData = 0;
        public int whichTextID = 0;

        // runtime variable
        public string runtimeVariable = "";

        public TextEntry(int  _whichTextData,int _whichTextID) {
            TextType        = 0;
            whichTextData   = _whichTextData;
            whichTextID     = _whichTextID;
            runtimeVariable = "";
        }

        public TextEntry(string _runtimeVariable)
        {
            TextType = 1;
            whichTextData = 0;
            whichTextID = 0;
            runtimeVariable = _runtimeVariable;
        }
        #endregion
    }
}
