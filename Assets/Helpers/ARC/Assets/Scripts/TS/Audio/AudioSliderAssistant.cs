// Desciption: AudioSliderAssistant:
// In the Audio Menu the script is attached to each slider to initialize the value of the slider
// when the page is opened
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class AudioSliderAssistant : MonoBehaviour
    {
        public Slider           slider;
        public AnimationCurve   animCurves;

        public void UpdateAudioValue(int sliderID)
        {
            if(SoundManager.instance.listAudioGroupParams.Count > sliderID && this.gameObject.activeInHierarchy)
            {
                float sliderValue = slider.value;
                SoundManager.instance.UpdateAudioMixerGroups(sliderID, sliderValue);
            }
        }

        public void InitSlider(float value)
        {
            //UpdateAudioValue(0);
            //float newValue = 0.98f;
            //Debug.Log("value: " + value);
            //slider.value = value;
            //Debug.Log("slider.value : " + slider.value);
        }
    }
}
