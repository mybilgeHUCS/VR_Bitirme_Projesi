using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class TransitionAssistantHorizontal : MonoBehaviour
    {
        public RectTransform objTransition_01;

        //-> Transition Left To Right
        public bool Part_01_LeftToRight()
        {
            #region
            StartCoroutine(Part1((ReturnTrueAfterCoroutine) => {
                if (ReturnTrueAfterCoroutine) { TransitionManager.instance.isTransitionPart1Progress = false; }
            }, new Vector2(0, .5f), new Vector2(1, .5f),0,1, 2));

            return true;
            #endregion
        }

        public bool Part_02_LeftToRight()
        {
            #region
            StartCoroutine(Part1((ReturnTrueAfterCoroutine) => {
                if (ReturnTrueAfterCoroutine) { TransitionManager.instance.isTransitionPart2Progress = false; }
            }, new Vector2(1, .5f), new Vector2(1, .5f),1,0, 2));

            return true;
            #endregion
        }


        //-> Transition Left To Right
        public bool Part_01_RightToLeft()
        {
            #region
            StartCoroutine(Part1((ReturnTrueAfterCoroutine) => {
                if (ReturnTrueAfterCoroutine) { TransitionManager.instance.isTransitionPart1Progress = false; }
            }, new Vector2(1, .5f), new Vector2(1, .5f), 0, 1, 2));

            return true;
            #endregion
        }

        public bool Part_02_RightToLeft()
        {
            #region
            StartCoroutine(Part1((ReturnTrueAfterCoroutine) => {
                if (ReturnTrueAfterCoroutine) { TransitionManager.instance.isTransitionPart2Progress = false; }
            }, new Vector2(0, .5f), new Vector2(1, .5f),1,0, 2));
            
            return true;
            #endregion
        }

        IEnumerator Part1(System.Action<bool> callback,Vector2 pivotStart, Vector2 pivotEnd,float scaleStart, float scaleEnd, float speed)
        {
            #region
            objTransition_01.pivot = pivotStart;
            objTransition_01.localScale = new Vector3(
                    scaleStart,
                    objTransition_01.localScale.y,
                    objTransition_01.localScale.z);

            while (objTransition_01.localScale.x != scaleEnd)
            {
                objTransition_01.localScale = new Vector3(
                    Mathf.MoveTowards(objTransition_01.localScale.x, scaleEnd, Time.deltaTime * speed),
                    objTransition_01.localScale.y,
                    objTransition_01.localScale.z);
                yield return null;
            }

            objTransition_01.pivot = pivotEnd;

            //yield return new WaitForSeconds(2);

            callback(true);
            #endregion
        }

        

        /*IEnumerator Part2(System.Action<bool> callback, Vector2 pivotStart, Vector2 pivotEnd, float speed)
        {
            #region
            while (objTransition_01.localScale.x != 0)
            {
                objTransition_01.localScale = new Vector3(
                    Mathf.MoveTowards(objTransition_01.localScale.x, 0, Time.deltaTime * speedTransition01),
                    objTransition_01.localScale.y,
                    objTransition_01.localScale.z);
                yield return null;
            }

            //yield return new WaitForSeconds(2);

            callback(true);
            #endregion
        }*/
    }
}
