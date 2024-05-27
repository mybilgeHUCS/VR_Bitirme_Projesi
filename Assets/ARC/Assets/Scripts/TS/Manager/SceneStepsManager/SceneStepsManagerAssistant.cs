// Description:  SceneStepsManagerAssistant: Attached to SceneStepsManager. Methods used in SceneStepsManager script
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    public class SceneStepsManagerAssistant : MonoBehaviour
    {
        public bool Step1()
        {
            StartCoroutine(GoStep2());
            return true;
        }

        IEnumerator GoStep2()
        {
            /*Debug.Log("Wait for Step2");
            //yield return new WaitForSeconds(3);
            yield return null;
            Debug.Log("GoStep2");*/

            yield return new WaitUntil(() => SceneStepsManager.instance.b_IsSceneStepManagerRunning == false);
            SceneStepsManager.instance.NextStep();
            yield return null;
        }

        public bool Step2()
        {
            StartCoroutine(GoStep3());
            return true;
        }

        IEnumerator GoStep3()
        {
           /* Debug.Log("Wait for Step3");
            yield return null;
            Debug.Log("GoStep3");*/
            yield return new WaitUntil(() => SceneStepsManager.instance.b_IsSceneStepManagerRunning == false);
            SceneStepsManager.instance.NextStep();
            yield return null;
        }

        public bool Step3()
        {
            StartCoroutine(GoEnd());
            return true;
        }

        IEnumerator GoEnd()
        {
            //Debug.Log("Wait for End");
            yield return new WaitUntil(() => SceneStepsManager.instance.b_IsSceneStepManagerRunning == false);
           // Debug.Log("It is the End");
            yield return null;
        }

        public bool DisplayNewPage(int PageNumber)
        {
            if (CanvasMainMenuManager.instance.currentSelectedPage != PageNumber)
            {
                //Debug.Log("returnCheckState: " + InfoPlayerTS.instance.returnCheckState(0));
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }
            return true;
        }

        public bool DisplayFirstMenuScreen()
        {
            int pageToOpen = GameModeGlobal.instance.lastSelectedMenuPage;
           
            if (CanvasMainMenuManager.instance.currentSelectedPage != pageToOpen)
            {
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[pageToOpen].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(pageToOpen);
            }
            //Debug.Log("First PAge");
            return true;
        }

        public bool PlayMusic(int musicID = 0)
        {
            if (musicID == -1)
                MusicManager.instance.MFadeOut(1);
            else
            {
                if (MusicList.instance.ListAudioClip.Count > musicID)
                {
                    MusicManager.instance.MCrossFade(1, MusicList.instance.ListAudioClip[musicID]);
                }
            }

            return true;
        }

        public bool FadeOutVehiclesAudioSources()
        {
            for(var i = 0;i< VehiclesRef.instance.listVehicles.Count; i++)
            {
                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInitAudio>().FadeOut();
            }
            return true;
        }

    }
}