// Description: LoadSceneAssistant: Attached to LoadSceneManager object.
using UnityEngine;
using System.Collections;

namespace TS.Generics
{
    public class LoadSceneAssistant : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;

        public bool A_S101_ShowLoadingScreen()
        {
            StartCoroutine(CanvasLoadingManager.instance.DisplayCanvasLoading());
            return true;
        }

        public bool A_S102_CheckIfLoadingScreenIsDisplayed()
        {
            return CanvasLoadingManager.instance.b_CanvasLoadingIsDisplayed;
        }

        public bool A_S100_HideMouse()
        {
            MouseManager.instance.CusrorVisibility(false);
            return true;
        }



        public bool PlayLoadingPageMusic(int ID)
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(PlayLoadingPageMusicRoutine(ID));
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator PlayLoadingPageMusicRoutine(int ID)
        {
            #region
            b_InitDone = false;

            if (ID == -1)
                MusicManager.instance.MFadeOut(1);
            else
                MusicManager.instance.MCrossFade(1, MusicList.instance.ListAudioClip[ID]);

            yield return new WaitUntil(() => MusicManager.instance.b_IsFading == false);


            b_InitDone = true;
            yield return null;
            #endregion
        }
    }
}