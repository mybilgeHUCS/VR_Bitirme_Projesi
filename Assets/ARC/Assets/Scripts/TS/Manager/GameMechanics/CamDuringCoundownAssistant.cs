// Description: CamDuringCoundownAssistant: Attached to CamDuringCountdown object in the Hierarchy.
// Methods used by CamDuringCountdown
using System.Collections;
using UnityEngine;


namespace TS.Generics
{
    public class CamDuringCoundownAssistant : MonoBehaviour
    {
        [HideInInspector]
        public  bool b_InitDone;
        private bool b_InitInProgress;

        //-> New Camera Position
        public bool Step0NewCameraPosition()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(Step0NewCameraPositionRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator Step0NewCameraPositionRoutine()
        {
            #region
            b_InitDone = false;

            for (var i = 0; i < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer; i++)
            {
                CamRef.instance.listCameras[i].transform.parent.parent.parent.GetComponent<Cam_Follow>().CameraMode(0);

                Transform newPos = VehiclesRef.instance.listVehicles[i].GetComponent<camSystem>().countdownRefPosCamList[0];
                CamRef.instance.listCameras[i].transform.parent.parent.parent.GetComponent<Cam_Follow>().ChooseNewCamPosFreeMode(newPos);
            }

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                yield return null;
            }


            b_InitDone = true;
            yield return null;
            #endregion
        }

        //-> New Camera Position
        public bool Step1NewCameraPosition()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(Step1NewCameraPositionRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator Step1NewCameraPositionRoutine()
        {
            #region
            b_InitDone = false;

            for (var i = 0; i < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer; i++)
            {
                Transform newPos = VehiclesRef.instance.listVehicles[i].GetComponent<camSystem>().countdownRefPosCamList[1];
                CamRef.instance.listCameras[i].transform.parent.parent.parent.GetComponent<Cam_Follow>().ChooseNewCamPosFreeMode(newPos);
            }

            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime;
                yield return null;
            }


            b_InitDone = true;
            yield return null;
            #endregion
        }

        //-> New Camera Mode
        public bool BNewCameraMode()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(NewModeRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator NewModeRoutine()
        {
            #region
            b_InitDone = false;
            for (var i = 0; i < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer; i++)
            {
                CamRef.instance.listCameras[i].transform.parent.parent.parent.GetComponent<Cam_Follow>().CameraMode(1);
            }
            b_InitDone = true;
            yield return null;
            #endregion
        }
    }
}
