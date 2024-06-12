using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;
using TS.ARC;
using UnityEngine.Events;

namespace TS.Generics
{
    public class VehicleActionsWhenRaceEnded : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;
        VehicleInfo vehicleInfo;
        //AirplaneController airplaneController;
        VehicleAI vehicleAI;
        camSystem camSystem;

        public UnityEvent finishTheRace;

        private void Start()
        {
          
        }

        //-> Initialisation
        public bool bInitActionsWhenRaceEnded()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator InitRoutine()
        {
            LapCounterAndPosition.instance.AVechicleFinishTheRace += VechicleFinishTheRace;
            vehicleInfo = GetComponent<VehicleInfo>();
            //airplaneController = GetComponent<AirplaneController>();
            vehicleAI = GetComponent<VehicleAI>();
            camSystem = GetComponent<camSystem>();

            b_InitDone = true;
            //Debug.Log("Init: VehicleDamage -> Done");
            yield return null;
        }

        public void OnDestroy()
        {
            if(LapCounterAndPosition.instance && LapCounterAndPosition.instance.AVechicleFinishTheRace != null)
                LapCounterAndPosition.instance.AVechicleFinishTheRace -= VechicleFinishTheRace;
        }


        public void VechicleFinishTheRace(int vehicleID)
        {
            if(vehicleID == vehicleInfo.playerNumber)
            {
                finishTheRace?.Invoke();
            }
        }

        public void EndOfTheRace()
        {
            vehicleAI.enabled = true;
            //camSystem.speedLR = 2;
            //camSystem.speedUD = 1;
        }

        public void CongratulationSeq()
        {
            int playerID = vehicleInfo.playerNumber;

            Congratulation[] congratulations = FindObjectsOfType<Congratulation>();

            foreach (Congratulation obj in congratulations)
                if (obj.playerID == playerID)
                    obj.CongratulationSeq();

        }

        public void PlayMusic(int ID = 0)
        {
            if (ID == -1)
                MusicManager.instance.MFadeOut(1);
            else
            {
                if (MusicList.instance.ListAudioClip.Count > ID)
                {
                    MusicManager.instance.MCrossFade(1, MusicList.instance.ListAudioClip[ID]);
                }
            }
        }

        public void DisablePlayerInputs()
        {
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = false;
        }

        public void DisablePowerUps()
        {
           GetComponent<PowerUpsSystem>().NewPowerUp();
        }

        public void EnableCamViewAfterTheRace(Transform camPos)
        {
            Cam_Follow[] cam_Follows = FindObjectsOfType<Cam_Follow>();

            foreach (Cam_Follow cam in cam_Follows)
                if (cam.playerID == vehicleInfo.playerNumber)
                    cam.InitCamViewAfterRace(camPos); // Init CamViewAfterTheRace Mode
        }
    }

}
