// Description: QualityUIManager: Methods used in the Quality in the main Menu scene
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

namespace TS.Generics
{
    public class QualityUIManager : MonoBehaviour
    {
        public static QualityUIManager  instance = null;

        public List<GameObject>         l_QualityCheckmarks = new List<GameObject>();   // list of Quality Checkmarks

        private int                     currentResolution = 0;
        public GameObject               objTxt_Resolution;                              // Use to display the actual resolution on screen
        private int                     LastHeightResolution = 0;
        private int                     LastWidthResolution = 0;

        [System.Serializable]
        public class idText
        {
            public string   name = "";
            public int      listID = 0;
            public int      entryID = 2;
            public int      selectedQuality = 0;
        }
        public List<idText>             idTextList = new List<idText>();
        public CurrentText              qualityText;
        public int                      currentSelectedQualityInIDTextList = 0;


        public UnityEvent InitQuality;

        public List<UniversalRenderPipelineAsset> urpRendererList;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }


        public bool B_InitQuality()
        {
            InitQualitySettingsOnScreen(QualitySettings.GetQualityLevel());
            InitResolution();
            //Debug.Log("Quality page Initialized");
            return true;
        }

        public void ExitAudioMenu()
        {
            //Debug.Log("Exit Quality Menu");
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }


        // --> The quality settings Fastest, Fast, good, beautiful, fantastic	
        public void ChooseQualitySettings(int dir)
        {
            currentSelectedQualityInIDTextList += dir + idTextList.Count;
            currentSelectedQualityInIDTextList %= idTextList.Count;

            InitQualitySettingsOnScreen(idTextList[currentSelectedQualityInIDTextList].selectedQuality);
        }


        // --> Init Quality on screen
        private void InitQualitySettingsOnScreen(int tmpQuality)
        {
            for (int i = 0; i < idTextList.Count; i++)
            {
                if (idTextList[i].selectedQuality == tmpQuality)
                {
                    qualityText.NewTextWithSpecificID(idTextList[i].entryID, idTextList[i].listID);
                    currentSelectedQualityInIDTextList = i;
                    QualitySettings.SetQualityLevel(idTextList[currentSelectedQualityInIDTextList].selectedQuality, true);
                    break;
                }
            }

            InitQuality.Invoke();
            //Debug.Log("Change Quality");
        }


        //-> Init Resolution
        private void InitResolution()
        {
            #if UNITY_STANDALONE
            Resolution[] resolutions = Screen.resolutions;

            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolution = i;                                          // Init resolution value that represent the current Resolution settings
                }
            }
            displayNewReolution(resolutions);
            #endif
        }

        // --> Press button " Next_Resolution" or "Last Resolution"
        public void ChooseResolution(int value)
        {
            #if UNITY_STANDALONE
            Resolution[] resolutions = Screen.resolutions;
            LastHeightResolution = resolutions[currentResolution].height;
            LastWidthResolution = resolutions[currentResolution].width;

            while (LastHeightResolution == resolutions[currentResolution].height || LastWidthResolution == resolutions[currentResolution].width)
            {
                currentResolution += value;
                if (currentResolution < 0)
                    currentResolution = resolutions.Length - 1;
                else
                    currentResolution = currentResolution % resolutions.Length;

                //if (txt_Resolution) txt_Resolution.text = resolutions[currentResolution].width + "x" + resolutions[currentResolution].height;
                displayNewReolution(resolutions);
            }
            #endif
        }

        private void displayNewReolution(Resolution[] resolutions)
        {
            #if UNITY_STANDALONE
            string newRes = resolutions[currentResolution].width + "x" + resolutions[currentResolution].height;
            objTxt_Resolution.GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(newRes) });
            #endif
        }

        // --> Press button "Validate_Resolution"
        public void ValidateResolution()
        {
            #if UNITY_STANDALONE
            Resolution[] resolutions = Screen.resolutions;
            Screen.SetResolution(resolutions[currentResolution].width, resolutions[currentResolution].height, true);
            #endif
        }

        public void URPShadowDistanceLow(int value = 100)
        {
            if (QualitySettings.GetQualityLevel() == 0)
            {
                urpRendererList[0].shadowDistance = value;
                Debug.Log("Low");
            }
        }

        public void URPShadowDistanceMedium(int value = 350)
        {
            if (QualitySettings.GetQualityLevel() == 1)
            {
                urpRendererList[1].shadowDistance = value;
                Debug.Log("Medium");
            }
        }

        public void URPShadowDistanceHigh(int value = 1000)
        {
            if (QualitySettings.GetQualityLevel() == 2)
            {
                urpRendererList[2].shadowDistance = value;
                Debug.Log("High");
            }
        }
    }
}
