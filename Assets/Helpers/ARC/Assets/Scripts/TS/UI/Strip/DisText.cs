using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class DisText : MonoBehaviour
    {
        public float scaleSpeed = .5f;
        public float delay;
        public AnimationCurve scaleCurve;
        private float t;

        public RectTransform rectGrp;

        public Vector2 initSizeDelta = new Vector2(0, 0);

        public bool disableLeftLineAtTheEnd = false;
        public GameObject rightLine;

        public void NewScale(float value = 1)
        {
            StartCoroutine(scaleRectTransformRoutine(value));
        }


        IEnumerator scaleRectTransformRoutine(float value = 1)
        {
            
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;

            if (disableLeftLineAtTheEnd && rightLine)
            {
                rightLine.SetActive(true);
                
            }

            rightLine.GetComponent<Image>().color =
                    new Color(rightLine.GetComponent<Image>().color.r,
                    rightLine.GetComponent<Image>().color.g,
                    rightLine.GetComponent<Image>().color.b,
                    1);

            t = 0;
            while (t < delay)
            {
                if (!PauseManager.instance
                    ||
                    PauseManager.instance && !PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / delay;
                    yield return null;
                }

                yield return null;
            }


            //Debug.Log("Scale UI");

            rectGrp.sizeDelta = initSizeDelta;

            t = 0;
            while (t < scaleSpeed)
            {
                if (!PauseManager.instance
                   ||
                   PauseManager.instance && !PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / scaleSpeed;
                    float localZEvaluate = scaleCurve.Evaluate(t / scaleSpeed);
                    rectGrp.sizeDelta = new Vector2(localZEvaluate * value, rectGrp.sizeDelta.y);
                    yield return null;
                }
                yield return null;
            }

            t = 0;
            while (t < scaleSpeed)
            {
                if (!PauseManager.instance
                    ||
                    PauseManager.instance && !PauseManager.instance.Bool_IsGamePaused)
                {
                    t += Time.deltaTime / scaleSpeed;
                    float localZEvaluate = scaleCurve.Evaluate(t / scaleSpeed);
                    //t %= scaleSpeed;
                    rectGrp.sizeDelta = new Vector2(localZEvaluate * value + value, rectGrp.sizeDelta.y);
                    rightLine.GetComponent<Image>().color =
                        new Color(rightLine.GetComponent<Image>().color.r,
                        rightLine.GetComponent<Image>().color.g,
                        rightLine.GetComponent<Image>().color.b,
                        1 - localZEvaluate);


                    yield return null;
                }
               
                yield return null;
            }

            //if (disableLeftLineAtTheEnd && rightLine) rightLine.SetActive(false);
            yield return null;
        }

        public void InitScale(float value = 0)
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            StopAllCoroutines();
            rectGrp.sizeDelta = new Vector2(0,0);
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void StopScale()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            StopAllCoroutines();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public bool BInitTitleScale(float value = 0)
        {
            rectGrp.sizeDelta = new Vector2(value * 2, rectGrp.sizeDelta.y);
            rightLine.GetComponent<Image>().color =
                   new Color(rightLine.GetComponent<Image>().color.r,
                   rightLine.GetComponent<Image>().color.g,
                   rightLine.GetComponent<Image>().color.b,
                   0);
            return true;
        }
    }

}
