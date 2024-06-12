//Description: MusicManager. Work in association with MusicList
//Allows to play Music. Allows to use cross fade between two musics. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager  instance = null;

        public List<AudioSource>    ListAudioSource = new List<AudioSource>();
        public int                  currentAudioSource = -1;
        public bool                 b_IsFading;
        public List<AnimationCurve> listAnimationCurves = new List<AnimationCurve>();
        public AudioClip            currentAudioClip;

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;

            #endregion
        }



        public void MCrossFade(float crossFadeSpeed, AudioClip newClip = null, int whichAnimCurve = 0, float aStartPosition = -1)
        {
            if(currentAudioSource != -1 &&
                ListAudioSource[currentAudioSource].isPlaying &&
                ListAudioSource[currentAudioSource].clip != newClip
                ||
                currentAudioSource == -1
                )
            {
                StopAllCoroutines();
                StartCoroutine(sCrossFade(crossFadeSpeed, newClip, whichAnimCurve, aStartPosition));
            }
        }

        private IEnumerator sCrossFade(float crossFadeSpeed, AudioClip newClip = null, int whichAnimCurve = 0,float aStartPosition = -1)
        {
            #region
            //Debug.Log("Cross Fade");

            currentAudioClip = newClip;

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

            if(aStartPosition != -1)
                ListAudioSource[currentAudioSource].time = aStartPosition;

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
            StopAllCoroutines();
            StartCoroutine(sFadeOut(crossFadeSpeed, whichAnimCurve));
        }

        private IEnumerator sFadeOut(float FadeOutSpeed, int whichAnimCurve = 0)
        {
            #region
            b_IsFading = true;

            currentAudioClip = null;

            if (currentAudioSource != -1)
            {
                float t = ListAudioSource[currentAudioSource].volume;


                while (t > 0)
                {
                    t = Mathf.MoveTowards(t, 0, Time.deltaTime * FadeOutSpeed);
                    ListAudioSource[currentAudioSource].volume = listAnimationCurves[whichAnimCurve].Evaluate(t);
                    yield return null;
                }

                ListAudioSource[currentAudioSource].gameObject.SetActive(false);
                ListAudioSource[currentAudioSource].Stop();

                currentAudioSource = -1;
            }
            

            b_IsFading = false;
            yield return null;
            #endregion
        }

    }

}
