// Description: SoundUIManager: Used in the Audio Page to init Sliders and Save volumes when the menu is closed.
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class SoundUIManager : MonoBehaviour
    {
        public static SoundUIManager    instance = null;
        public List<Slider>             slidersList = new List<Slider>();

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        public bool B_InitSliders()
        {
            int howManySliders = SoundManager.instance.listAudioGroupParams.Count;

            for (var i = 0; i < howManySliders; i++)
            {
                if (slidersList.Count > i && slidersList[i])
                {
                    slidersList[i].value = SoundManager.instance.listAudioGroupParams[i].volume;
                }
                else
                    Debug.Log("Slider Error: The slider doesn't exist");
            }
            return true;
        }

        public void ExitAudioMenu()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            SoundManager.instance.Bool_SaveMixerValues();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        } 
    }
}
