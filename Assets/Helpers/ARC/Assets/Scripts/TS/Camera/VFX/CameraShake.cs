// Description: CameraShake: Shake the camera.
// Called by CameraFx attached to the player Camera
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class CameraShake
    {
        public bool             b_ShakeEnable = true;
        public AnimationCurve   animCurve;
        private bool            b_ShakeAvailable;
        public float            speed = 2;
        public float            amplitude = 4;
        public float            delay;


        public void VFXCameraShake(MonoBehaviour mono, Transform trans, CameraFx camFx)
        {
            if (!b_ShakeAvailable)
            {
                camFx.b_CameraShakeAvailable = false;
                mono.StopCoroutine(ShakeRoutine(null, camFx));
                mono.StartCoroutine(ShakeRoutine(trans,camFx));
                b_ShakeAvailable = true;
            }
        }

        IEnumerator ShakeRoutine(Transform trans, CameraFx camFx)
        {
            float t = 0;

            //-> Delay
            while (t != delay)
            {
                if(!PauseManager.instance.Bool_IsGamePaused)
                    t = Mathf.MoveTowards(t, delay, Time.deltaTime);
                yield return null;
            }


            //-> VFX
            t = 0;

            while (t != 1)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                {
                    t = Mathf.MoveTowards(t, 1, Time.deltaTime * speed);
                    float newXPos = amplitude * animCurve.Evaluate(t);
                    trans.localPosition = new Vector3(newXPos, 0, 0);
                }
                   
                yield return null;
            }

            b_ShakeAvailable = false;
            camFx.b_CameraShakeAvailable = true;
            yield return null;
        }
    }
}
