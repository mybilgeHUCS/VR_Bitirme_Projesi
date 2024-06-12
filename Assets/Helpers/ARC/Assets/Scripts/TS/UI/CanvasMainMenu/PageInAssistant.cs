//Description: PageInAssistant.cs. Use in association with PageIn.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class PageInAssistant : MonoBehaviour
    {
        public List<GameObject> listGrpBtn  = new List<GameObject>();
        public List<GameObject> listGrpBtn2 = new List<GameObject>();


        //-> Example 01: Method 1:  Disable a list of objects 
        public void DisableListObj()
        {
            //Debug.Log(" DisableListObj()");
            for (var i = 0; i < listGrpBtn.Count; i++)
            {
                listGrpBtn[i].SetActive(false);
            }
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        // Example 01: Method 2 Part 1: Display objects one by one 
        public void Seq()
        {
            StartCoroutine(ISeq());
        }

        // Example 01: Method 2 Part 2: Display objects one by one 
        IEnumerator ISeq()
        {
            yield return new WaitUntil(() => TransitionManager.instance.isTransitionPart2Progress == false);
            for (var i = 0;i< listGrpBtn.Count; i++)
            {
                float t = 0;
                float duration = .15f;
                listGrpBtn[i].SetActive(true);
                while (t < duration)
                {
                    t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                    yield return null;
                }
            }
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            yield return null;
        }



        //-> Example 02: Method 1: Disable a list objects 
        public void DisableListObj2()
        {
            for (var i = 0; i < listGrpBtn2.Count; i++)
            {
                listGrpBtn2[i].SetActive(false);
            }
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        // Example 02: Method 2 Part 1: Enable a list of objects one by one 
        public void Seq2()
        {
            StartCoroutine(ISeq2());
        }

        // Example 02: Method 2 Part 2: Enable a list of objects one by one 
        IEnumerator ISeq2()
        {
            yield return new WaitUntil(() => TransitionManager.instance.isTransitionPart2Progress == false);
            for (var i = 0; i < listGrpBtn2.Count; i++)
            {
                float t = 0;
                float duration = .5f;
                listGrpBtn2[i].SetActive(true);
                while (t < duration)
                {
                    t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                    yield return null;
                }
            }
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            yield return null;
        }


        //-> Example 03: Start New Music
        public void StartNewMusic(int newMusic)
        {
            MusicManager.instance.MCrossFade(1, MusicList.instance.ListAudioClip[newMusic]);
            // A methods used as a page event MUST be ended by the next line.
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }


       
    }
}