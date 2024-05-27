//Description: AmbianceManager. Work in association with AmbianceList
//Allows to play Ambiance. Allows to use cross fade between two ambiances. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class AmbianceManager : MonoBehaviour
    {
        public static AmbianceManager   instance = null;

        public List<AudioSource>        ListAudioSource = new List<AudioSource>();
        public int                      currentAudioSource = -1;
        public bool                     b_IsFading;
        public List<AnimationCurve>     listAnimationCurves = new List<AnimationCurve>();

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            #endregion
        }



        public void MCrossFade(float crossFadeSpeed, AudioClip newClip = null, int whichAnimCurve = 0)
        {
            StartCoroutine(sCrossFade(crossFadeSpeed, newClip, whichAnimCurve));
        }

        private IEnumerator sCrossFade(float crossFadeSpeed, AudioClip newClip = null, int whichAnimCurve = 0)
        {
            #region
            Debug.Log("Cross Fade");

            b_IsFading = true;
            float t = 0;

            currentAudioSource++;
            currentAudioSource %= ListAudioSource.Count;

            //
            ListAudioSource[currentAudioSource].gameObject.SetActive(true);
            ListAudioSource[currentAudioSource].volume = 0;

            if (newClip)
                ListAudioSource[currentAudioSource].clip = newClip;
            else
                ListAudioSource[currentAudioSource].clip = MusicList.instance.ListAudioClip[currentAudioSource];

            if(ListAudioSource[currentAudioSource].gameObject.activeInHierarchy)
                ListAudioSource[currentAudioSource].Play();


            while (t < 1)
            {
                t = Mathf.MoveTowards(t, 1, Time.deltaTime * crossFadeSpeed);

                if (currentAudioSource - 1 == -1)
                {
                    ListAudioSource[ListAudioSource.Count - 1].volume = listAnimationCurves[whichAnimCurve].Evaluate(1 - t);
                }
                if (currentAudioSource - 1 != -1)
                {
                    ListAudioSource[currentAudioSource - 1].volume = listAnimationCurves[whichAnimCurve].Evaluate(1 - t);
                }

                ListAudioSource[currentAudioSource].volume = listAnimationCurves[whichAnimCurve].Evaluate(t);
                yield return null;
            }

            if (currentAudioSource - 1 == -1)
            {
                ListAudioSource[ListAudioSource.Count - 1].Stop();
                ListAudioSource[ListAudioSource.Count - 1].gameObject.SetActive(false);

                yield return new WaitUntil(() => !ListAudioSource[ListAudioSource.Count - 1].gameObject.activeSelf);
            }
            if (currentAudioSource - 1 != -1)
            {
                ListAudioSource[currentAudioSource - 1].Stop();
                ListAudioSource[currentAudioSource - 1].gameObject.SetActive(false);
                yield return new WaitUntil(() => !ListAudioSource[currentAudioSource - 1].gameObject.activeSelf);
            }

            b_IsFading = false;
            yield return null;
            #endregion
        }


        public void MFadeOut(float crossFadeSpeed, int whichAnimCurve = 0)
        {
            StartCoroutine(sFadeOut(crossFadeSpeed, whichAnimCurve));
        }

        private IEnumerator sFadeOut(float FadeOutSpeed, int whichAnimCurve = 0)
        {
            #region
            b_IsFading = true;
            float t = ListAudioSource[currentAudioSource].volume;


            while (t > 0)
            {
                t = Mathf.MoveTowards(t, 0, Time.deltaTime * FadeOutSpeed);
                ListAudioSource[currentAudioSource].volume = listAnimationCurves[whichAnimCurve].Evaluate(t);
                yield return null;
            }

            ListAudioSource[currentAudioSource].gameObject.SetActive(false);
            ListAudioSource[currentAudioSource].Stop();

            b_IsFading = false;
            yield return null;
            #endregion
        }
    }

}
