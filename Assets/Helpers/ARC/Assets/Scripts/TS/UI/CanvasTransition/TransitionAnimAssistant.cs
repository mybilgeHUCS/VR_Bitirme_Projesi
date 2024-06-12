//Decription: TransitionAnimAssistant.cs. The script is used in association with a transition using animation.
using UnityEngine;

namespace TS.Generics
{
    public class TransitionAnimAssistant : MonoBehaviour
    {
        public float sfxTransitionVolume = .75f;

        //-> This method is used with transition that use Animation.
        // The method is called when the current page is disabled and the new page is enable
        public void TransitionPart1Ended()
        {
            TransitionManager.instance.TransitionPart1Ended();
            Debug.Log("Transition Part 2");
        }

        //-> This method is used with transition that use Animation.
        // The method is called when the animation ended.
        public void TransitionPart2Ended()
        {
            //TransitionManager.instance.isTransitionPart2Progress = false;
            Debug.Log("Transition Part 1");

           TransitionManager.instance.TransitionPart2Ended();
        }

        public bool PlaySfx(int value)
        {
            SoundFxManager.instance.Play(SfxList.instance.listAudioClip[value], sfxTransitionVolume);
            return true;
        }
    }
}

