//Description: SoundManager. It allows to load and save AudioMixer value.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System.Globalization;

namespace TS.Generics
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager  instance = null;
        [HideInInspector]
        public bool                 SeeInspector;
        [HideInInspector]
        public bool                 moreOptions;
        [HideInInspector]
        public bool                 helpBox = true;

        public AudioMixer           _AudioMixer;

        public string               SaveName = "AM";
        public string               sMasterGrp = "Master";

        public List<float>          listGroupVolume = new List<float>();
        

        public bool                 b_SaveProcess = false;
        public int                  loadInProgress;

        private float               maxVol = 0;     // volume max for an audio group
        private float               minVol = 80;    // volume min for an audio group

        public AnimationCurve       animCurves;     // Use for the volume curve of an audio group

        [System.Serializable]
        public class AudioGroupParams
        {
            public string exposedParameterName;
            public float volume = 0;
        }
        public List<AudioGroupParams> listAudioGroupParams = new List<AudioGroupParams>();

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            //If instance already exists and it's not this:
            else if (instance != this)
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            #endregion
        }

      
        //-> Save Mixer Values
        public bool Bool_SaveMixerValues()
        {
            #region
            b_SaveProcess = true;

            CultureInfo cultureInfo = new CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            string sParams = "";

            AudioMixerGroup[] AudioMixerGroups = _AudioMixer.FindMatchingGroups(sMasterGrp);
            for (var i = 0; i < AudioMixerGroups.Length; i++)
            {
                //Debug.Log(AudioMixerGroups[i].name + " : " + GetLevel(AudioMixerGroups[i].name + "Vol").ToString());
                sParams += listAudioGroupParams[i].volume + ":";
            }

            while (!SaveManager.instance.saveAndReturnTrueAFterSaveProcess(sParams, SaveName))
            {
                return false;
            }
            //Debug.Log("Audio Mixer Saved");

            b_SaveProcess = false;

            return true;
            #endregion
        }


        public bool B_LoadGroupVolumes()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(LoadGroupVolume());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }

        //-> Load Mixer Values
        public IEnumerator LoadGroupVolume()
        {
            string sParams = SaveManager.instance.LoadDAT(SaveName);
            AudioMixerGroup[] AudioMixerGroups = _AudioMixer.FindMatchingGroups(sMasterGrp);

            CultureInfo cultureInfo = new CultureInfo("en-US");
            System.Threading.Thread.CurrentThread.CurrentCulture = cultureInfo;

            //Debug.Log("AudioMixerGroups: " + AudioMixerGroups.Length + " : " + "sParams: " + sParams);

            if (sParams != "")
            {
                string[] codes = sParams.Split(':');

                for (var i = 0; i < codes.Length - 1; i++)
                {
                    float volume = float.Parse(codes[i]);
                    _AudioMixer.SetFloat(AudioMixerGroups[i].name + "Vol", returnVolume(volume));
                    listAudioGroupParams[i].volume = volume;

                    // Wait until the value is updated
                    float refValue = GetLevel(AudioMixerGroups[i].name + "Vol");
                    float value;
                    bool result = _AudioMixer.GetFloat(AudioMixerGroups[i].name + "Vol", out value);
                    if (result) yield return new WaitUntil(() => value == refValue);      
                }
            }
            else
            {
                // Init and save values the first time
                for(var i = 0;i< listAudioGroupParams.Count; i++)
                {
                    _AudioMixer.SetFloat(listAudioGroupParams[i].exposedParameterName, returnVolume(listAudioGroupParams[i].volume));
                    listAudioGroupParams[i].volume = listAudioGroupParams[i].volume;
                    //Debug.Log("volume: " + listAudioGroupParams[i].volume);
                }

                Bool_SaveMixerValues();

                yield return new WaitUntil(() => !b_SaveProcess);
            }

            loadInProgress = 2;
            //Debug.Log("Sound parameters Loaded");
            yield return null;
        }

        //-> Get Volume values from the AudioMixer
        public float GetLevel(string _Name)
        {
            #region
            float value;
            bool result = _AudioMixer.GetFloat(_Name, out value);
            if (result)
                return value;
            else
                return 0f;
            #endregion
        }

        public float GetLevelWithSliderID(int sliderID)
        {
            #region
            AudioMixerGroup[] AudioMixerGroups = _AudioMixer.FindMatchingGroups(sMasterGrp);
            return GetLevel(AudioMixerGroups[sliderID].name + "Vol");
            #endregion
        }

        public void UpdateAudioMixerGroups(int sliderID, float newVolume)
        {
            //Debug.Log("exposedParameterName: " + newVolume);
            _AudioMixer.SetFloat(listAudioGroupParams[sliderID].exposedParameterName, returnVolume(newVolume));
            listAudioGroupParams[sliderID].volume = newVolume;
        }



        float returnVolume(float newVolume)
        {
            return (animCurves.Evaluate(newVolume) * minVol + maxVol) - minVol;
        }
    }

}
