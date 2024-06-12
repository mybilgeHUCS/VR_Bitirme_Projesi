//Description: SoundFxManager. Work in association with SfxList
//Allows to play Sfx.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class SoundFxManager : MonoBehaviour
    {
        public static SoundFxManager    instance = null;

        public List<AudioSource>        listAudioSourceCards = new List<AudioSource>();


        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;

            #endregion
        }

        //-> Play an AudioClip from listAudioClip (SfxList.cs)
        public void Play(AudioClip newClip, float newVolume = .5f)
        {
            for (var i = 0; i < listAudioSourceCards.Count; i++)
            {
                if (!listAudioSourceCards[i].gameObject.activeSelf)
                {
                    StartCoroutine(IPlay(i,newClip, newVolume));
                    break;
                }
            }
        }

        IEnumerator IPlay(int whichAudioSource, AudioClip newClip, float newVolume = .5f)
        {
            listAudioSourceCards[whichAudioSource].gameObject.SetActive(true);

            yield return new WaitUntil(() => listAudioSourceCards[whichAudioSource].gameObject.activeSelf);

            listAudioSourceCards[whichAudioSource].clip = newClip;
            listAudioSourceCards[whichAudioSource].volume = newVolume;

            if(listAudioSourceCards[whichAudioSource].gameObject.activeInHierarchy)
                listAudioSourceCards[whichAudioSource].Play();

            yield return new WaitUntil(() => !listAudioSourceCards[whichAudioSource].isPlaying);

            listAudioSourceCards[whichAudioSource].gameObject.SetActive(false);

            yield return null;
        }


        //-> Play an AudioClip from listRandomAudioList (SfxList.cs)
        public void PlayRandomAudioList(List<AudioClip> listClip, float newVolume = .5f)
        {
            int rand = UnityEngine.Random.Range(0, listClip.Count);

            for (var i = 0; i < listAudioSourceCards.Count; i++)
            {
                if (!listAudioSourceCards[i].isPlaying && listAudioSourceCards[i].gameObject.activeInHierarchy)
                {
                    listAudioSourceCards[i].clip = listClip[rand];
                    listAudioSourceCards[i].volume = newVolume;
                    listAudioSourceCards[i].Play();
                    break;
                }
            }
        }

    }
}
