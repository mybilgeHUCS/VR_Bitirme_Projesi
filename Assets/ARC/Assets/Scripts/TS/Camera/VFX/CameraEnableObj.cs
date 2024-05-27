// Description: CameraEnableObj: Enable/disable object.
// Called by CameraFx attached to the player Camera
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class CameraEnableObj
    {
        public GameObject   obj;
        public bool         b_Enable = true;
        public float        duration = 1;
        public float        delay;
        private bool        b_Available = true;



        public void VFXCameraEnableObj(MonoBehaviour mono,CameraFx camFx)
        {
            if (b_Available)
            {
                camFx.b_CameraEnableObjAvailable = false;
                mono.StopCoroutine(EnableObjRoutine(camFx));
                mono.StartCoroutine(EnableObjRoutine(camFx));
                b_Available = false;
            }
        }

        IEnumerator EnableObjRoutine(CameraFx camFx)
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
            obj.SetActive(true);
            while (t != duration)
            {
                if (!PauseManager.instance.Bool_IsGamePaused)
                    t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                yield return null;
            }
            obj.SetActive(false);
            b_Available = true;
            camFx.b_CameraEnableObjAvailable = true;
            yield return null;
        }
    }
}
