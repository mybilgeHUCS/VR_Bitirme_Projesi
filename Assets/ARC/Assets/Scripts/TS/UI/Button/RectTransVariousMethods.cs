// Description: RectTransVariousMethods:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class RectTransVariousMethods : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        //[Header("Change Scale")]
        public float scaleSpeed = 1;
        public List<Vector3> vectorThreeaList = new List<Vector3>(2) {new Vector3(1f,1f,1f),new Vector3(1.05f,1.05F,1f) };
        public bool bShowEditorScale = true;

        //[Header("Change Pivot")]
        public float pivotSpeed = 1;
        public List<Vector2> pivotaList = new List<Vector2>(2) { new Vector2(.5f, .4f), new Vector2(.5f,0) };
        public bool bShowEditorPivot = true;

        public void ChangePivotSmooth(int vectorID)
        {
            if(bShowEditorPivot)ChangePivot(vectorID, true);
        }
        public void ChangePivotStraight(int vectorID)
        {
            if (bShowEditorPivot) ChangePivot(vectorID, false);
        }
        public void ChangeScaleSmooth(int vectorID)
        {
            if (bShowEditorScale) ChangeScale(vectorID, true);
        }
        public void ChangeScaleStraight(int vectorID)
        {
            if (bShowEditorScale) ChangeScale(vectorID, false);
        }

        public void ChangeScale(int vectorID, bool bSmooth = true)
        {

            if (gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(ChangeScaleRoutine(vectorThreeaList[vectorID], bSmooth));
            }
            else
            {
                ScaleStraight(vectorThreeaList[vectorID]);
            }
        }

        IEnumerator ChangeScaleRoutine(Vector3 target, bool bSmooth = true)
        {
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            
            if (bSmooth)
            {
                while (rectTrans.localScale != target)
                {
                    rectTrans.localScale = Vector3.MoveTowards(rectTrans.localScale, target, Time.deltaTime * scaleSpeed);
                    yield return null;
                }
            }
            else
            {
                rectTrans.localScale = target;
            }

            yield return null;
        }

        public void ChangePivot(int vectorID,bool bSmooth = true)
        {
            if (gameObject.activeInHierarchy)
            {
                StopAllCoroutines();
                StartCoroutine(ChangePivotRoutine(pivotaList[vectorID], bSmooth));
            }
            else
            {
                PivotStraight(pivotaList[vectorID]);
            }
            
        }


        IEnumerator ChangePivotRoutine(Vector2 target, bool bSmooth = true)
        {
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();

            if (bSmooth)
            {
                while (rectTrans.pivot != target)
                {
                    rectTrans.pivot = Vector3.MoveTowards(rectTrans.pivot, target, Time.deltaTime * pivotSpeed);
                    yield return null;
                }
            }
            else
            {
                rectTrans.pivot = target;
            }
            yield return null;
        }

        void PivotStraight(Vector2 target) {
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            rectTrans.pivot = target;
        }

        void ScaleStraight(Vector2 target)
        {
            RectTransform rectTrans = gameObject.GetComponent<RectTransform>();
            rectTrans.localScale = target;
        }
    }
}