// Description: StepsAssistantTimeTrial: Attached to SceneStepsManager. Methods called by SceneStepsManager.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class StepsAssistantTimeTrial : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;

        //-> Time Trial Step 0:  -> Instantiate Vehicle
        public bool bStep0_TT_InstantiateVehicle()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(Step0_TT_InstantiateVehicleRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator Step0_TT_InstantiateVehicleRoutine()
        {
            #region
            b_InitDone = false;
            //Debug.Log("Time Trial Step 0:  -> Instantiate Vehicle");            
            yield return new WaitUntil(() => VehiclesRef.instance.bInstantiateVehicle(new List<int> { 0, 0, 0, 0 }) == true);

            b_InitDone = true;
            yield return null;
            #endregion
        }

        //-> Time Trial Step 1:  -> Init Game Modules
        public bool bStep1_TT_InitialiseGameModules()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(Step1_TT_InitialiseGameModulesRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;
             
            return b_InitDone;
            #endregion
        }

        IEnumerator Step1_TT_InitialiseGameModulesRoutine()
        {
            #region
            b_InitDone = false;

            //Debug.Log("Time Trial Step 1:  -> Init Game Modules");
            //-> Init obj CheckIfPlaneVisibleByCamera (Hierarchy: GameManager -> CheckIfPlaneVisibleByCamera)
            yield return new WaitUntil(() => VehiclesVisibleByTheCamList.instance.bInitVehiclesVisibleByCamera() == true);

            //-> Init Lap counter
            yield return new WaitUntil(() => LapCounterAndPosition.instance.bInitLapCounter() == true);

            //-> Init Minimap
            yield return new WaitUntil(() => MinimapManager.instance.bInitMiniMap() == true);

            //-> Init Vehicle flags for P1 and P2
            yield return new WaitUntil(() => VehicleFlagManager.instance.bInitVehicleFlag() == true);

            //-> Init Spitscreen UI
            yield return new WaitUntil(() => InitUIDependingGameMode.instance.bInitSpliScreenUI() == true);

            b_InitDone = true;

            yield return null;
            #endregion
        }

        //-> Time Trial Step 1:  -> Init Game Modules
        public bool bStep2_TT_AllowVehiclesToMove()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(Step2_TT_AllowVehiclesToMoveRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator Step2_TT_AllowVehiclesToMoveRoutine()
        {
            #region
            b_InitDone = false;
            Debug.Log("Time Trial Step 3:  -> Allow Vehicles To Move");

           
            for(var i = 0;i< VehiclesRef.instance.listVehicles.Count; i++)
            {
                //-> Allow car to move
                VehiclesRef.instance.listVehicles[i].b_IsVehicleAvailableToMove = true;

                //-> Init Audio Volumes
                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInitAudio>().bInitAudioSource();
            }

            b_InitDone = true;

            yield return null;
            #endregion
        }

        public bool DisablePowerUPs()
        {
            if(PowerUpsItemsGlobalParams.instance)
                PowerUpsItemsGlobalParams.instance.gameObject.SetActive(false);
            return true;
        }

        public bool DisableUIPlayerPosition()
        {
            CanvasInGameTag.instance.objList[4].SetActive(false);
            return true;
        }
    }
}