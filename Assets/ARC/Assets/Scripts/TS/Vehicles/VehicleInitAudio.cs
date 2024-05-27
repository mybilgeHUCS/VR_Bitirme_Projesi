using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TS.Generics;
using TS.ARC;
using System;

namespace TS.Generics
{
    public class VehicleInitAudio : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;
        public GameObject Grp_Audio;

        public List<AudioSource> listAudioSources;
        public List<float> volumes = new List<float>();

        private VehicleInfo vehicleInfo;

        //-> Init Audio
        public bool bInitAudioSource()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator InitRoutine()
        {
            #region
            b_InitDone = false;

            vehicleInfo = GetComponent<VehicleInfo>();

            volumes.Clear();

            for (var i = 0;i< listAudioSources.Count; i++)
            {
                float offsetVolume = 0;
                //-> Splitscreen case
                if ((vehicleInfo.playerNumber == 0 || vehicleInfo.playerNumber == 1) &&
                    InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
                {
                    if(vehicleInfo.playerNumber == 0)
                        offsetVolume = -.2f;
                    if (vehicleInfo.playerNumber == 1)
                    {
                        offsetVolume = -.2f;
                    }
                }

                //-> AI case
                if (vehicleInfo.playerNumber != 0  &&
                    InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 1)
                {
                    offsetVolume = -.1f;
                }

                volumes.Add(listAudioSources[i].volume + offsetVolume);
            }

            for (var i = 0; i < listAudioSources.Count; i++)
            {
                listAudioSources[i].volume = 0;
            }

            //-> Start Audio engine with a random position. Prevent Phasing effect
            //AudioClip clip = listAudioSources[0].clip;
            //listAudioSources[0].PlayDelayed(UnityEngine.Random.Range(0,1000));

            if(vehicleInfo.playerNumber > 0)
                listAudioSources[0].pitch = UnityEngine.Random.Range(-.01f, .01f);


            //-> Player 2. Change spatialBlend to hear the player 2 sound in split screen mode
            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
            if(vehicleInfo.playerNumber == 1 && howManyPlayer > 1)
                for (var i = 0; i < listAudioSources.Count; i++)
                {
                    if (i != 2)
                        listAudioSources[i].spatialBlend = 0f;
                    // Wind Case
                    else
                        listAudioSources[i].spatialBlend = .6f;
                }
                    


            float t = 0f;
            float timeToMove = 2f;

            Grp_Audio.SetActive(true);

            while (t < 1)
            {
                t += Time.deltaTime / timeToMove;

                for (var i = 0; i < listAudioSources.Count; i++)
                {
                    listAudioSources[i].volume = Mathf.Lerp(0, volumes[i],t);
                }

                yield return null;
            }

           

            b_InitDone = true;
            //SceneStepsManager.instance.NextStep();
            yield return null;
            #endregion
        }



        public void FadeOut()
        {
            StartCoroutine(FadeOutRoutine());
        }

        IEnumerator FadeOutRoutine()
        {
            #region
            b_InitDone = false;

            float t = 0;
            float timeToMove = 1f;

            List<float> currentVolumes = new List<float>();
            for (var i = 0; i < listAudioSources.Count; i++)
            {
                currentVolumes.Add(listAudioSources[i].volume);
            }

            while (t < 1)
            {
                t += Time.deltaTime / timeToMove;

                for (var i = 0; i < listAudioSources.Count; i++)
                {
                    listAudioSources[i].volume = Mathf.Lerp(currentVolumes[i], 0, t);
                }

                yield return null;
            }

            Grp_Audio.SetActive(false);

            b_InitDone = true;

            yield return null;
            #endregion
        }
    }

}
