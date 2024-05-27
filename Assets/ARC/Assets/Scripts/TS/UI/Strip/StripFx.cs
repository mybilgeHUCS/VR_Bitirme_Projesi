using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class StripFx : MonoBehaviour
    {
        public float scaleSpeed = .5f;
        public float delay;
        public AnimationCurve scaleCurve;
        private float t;

        public RectTransform rect;

        public Vector3 refRectPivot = Vector3.zero;
        public Vector3 targetRectPivot = Vector3.zero;

        public void NewScale(float value = 1)
        {
            StartCoroutine(scaleRectTransformRoutine(value));
        }


        IEnumerator scaleRectTransformRoutine(float value = 1)
        {
            rect.pivot = refRectPivot;
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;

            t = 0;
            while (t < delay)
            {
                if(!PauseManager.instance.Bool_IsGamePaused)
                    t += Time.deltaTime / delay;

                yield return null;
            }

            //Debug.Log("Move Rect UI");
           
            t = 0;
            while (t < scaleSpeed)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / scaleSpeed;
                    float localZEvaluate = scaleCurve.Evaluate(t / scaleSpeed);
                    rect.pivot = Vector3.Lerp(refRectPivot, targetRectPivot, localZEvaluate);
                    yield return null;
                }
                yield return null;
            }
            yield return null;
        }

        public void InitScale()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            StopAllCoroutines();
            rect.pivot = refRectPivot;
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void StopScale()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            StopAllCoroutines();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }

}
