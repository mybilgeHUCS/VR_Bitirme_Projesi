// Description: BoostShakeCam: Manage camera shake. Attached on BoosterShake object.
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    public class BoostShakeCam : MonoBehaviour
    {
        public bool             b_ShakeEnable = true;
        public AnimationCurve   animCurveX;
        public AnimationCurve   animCurveY;
        public float            speedOn = 1;
        public float            speedOff = 1;
        public float            amplitudeX = .5f;
        public float            amplitudeY = .5f;

        public bool             loop = true;

        float                   t = 0;

        public void ShakeStart()
        {
            if(b_ShakeEnable)
            {
                StopAllCoroutines();
                t = 0;
                StartCoroutine(ShakeRoutine(this.transform));
            }
           
        }

        public void ShakeStop()
        {
            if (!b_ShakeEnable)
            {
                StopAllCoroutines();
                StartCoroutine(ShakeResetRoutine(this.transform));
            }
        }

        IEnumerator ShakeRoutine(Transform trans/*, CameraFx camFx*/)
        {
            b_ShakeEnable = false;
            //Debug.Log("Shake");
            //-> VFX
            //t = 0;

            while (t != 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, 1, Time.deltaTime * speedOn);
                    float newXPos = amplitudeX * animCurveX.Evaluate(t);
                    float newYPos = amplitudeY * animCurveY.Evaluate(t);
                    trans.localPosition = new Vector3(newXPos, newYPos, 0);
                }

                yield return null;
            }

            if (loop)
            {
                b_ShakeEnable = true;
                ShakeStart();
            }
            yield return null;
        }

        IEnumerator ShakeResetRoutine(Transform trans/*, CameraFx camFx*/)
        {
            b_ShakeEnable = true;
            //-> VFX
            //t = 0;

            //Debug.Log("t: " + t);

            float posX = trans.localPosition.x;
            float posY = trans.localPosition.y;

            while (t != 0)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, 0, Time.deltaTime * speedOff);
                    float newXPos = posX * t;
                    float newYPos = posY * t;
                    trans.localPosition = new Vector3(newXPos, newYPos, 0);
                }

                yield return null;
            }

            yield return null;
        }
    }
}

