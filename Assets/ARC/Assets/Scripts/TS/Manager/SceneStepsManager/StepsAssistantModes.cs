// Description: StepsAssistantModes: Attached to SceneStepsManager. Methods called by SceneStepsManager.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class StepsAssistantModes : MonoBehaviour
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

            List<int> HowManyVehicleToInstantiate = new List<int>();
            int HowManyPlayer = 0;
            //-> Arcade Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                HowManyPlayer = DataRef.instance.arcadeModeData.howManyVehicleByRace;
            }
            
            //-> Time Trial Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                HowManyPlayer = 1;
            }
          
            //-> Championship Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 2)
            {
                int currentChampionship = GameModeChampionship.instance.currentSelection;
                int currentTrack = GameModeChampionship.instance.currentTrackInTheList;
                HowManyPlayer = DataRef.instance.championshipModeData.listOfChampionship[currentChampionship].listTrackParams[currentTrack].howManyVehicleByRace;
            }

            //-> Test Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 3)
            {
                HowManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.howManyVehicleInSelectedGameMode;
            }

            //-> Test 1P + No Collision
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 5)
            {
                HowManyPlayer = 1;
            }

            //-> Prevent bugs
            if (GameModeGlobal.instance.vehicleIDList.Count == 0)
            {
                for (var i = 0; i < HowManyPlayer; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(0);
                    HowManyVehicleToInstantiate.Add(0);
                }  
            }
            else
            {
                for (var i = 0; i < GameModeGlobal.instance.vehicleIDList.Count; i++)
                {
                    HowManyVehicleToInstantiate.Add(GameModeGlobal.instance.vehicleIDList[i]);
                }
            }
            

            yield return new WaitUntil(() => VehiclesRef.instance.bInstantiateVehicle(HowManyVehicleToInstantiate) == true);

            
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

           // Debug.Log("Time Trial Step 1:  -> Init Game Modules");
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

            //-> Disable Pause Mode
            PauseManager.instance.isPauseModeEnable = false;

            b_InitDone = true;
            yield return null;
            #endregion
        }

        //-> Time Trial Step 1:  -> Init Game Modules
        public bool bStep2_TT_AllowVehiclesToMoveAllAI()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(Step2_TT_AllowVehiclesToMoveAllAIRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
            {
                b_InitInProgress = false;
            }
               

            return b_InitDone;
            #endregion
        }

        IEnumerator Step2_TT_AllowVehiclesToMoveAllAIRoutine()
        {
            #region
            b_InitDone = false;
            //Debug.Log("Time Trial Step 3:  -> Allow Vehicles To Move All AI");

           int counter = 0;
            for(var i = 0;i< VehiclesRef.instance.listVehicles.Count; i++)
            {
                //-> Allow car to move
                VehiclesRef.instance.listVehicles[i].b_IsVehicleAvailableToMove = true;

                //-> Init Audio Volumes
                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInitAudio>().bInitAudioSource();

                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInfo>().b_IsPlayerInputAvailable = false; // The player can't move the plane but the plane is moving forward
                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled = false;

                counter++;
            }

            yield return new WaitUntil(() => counter == VehiclesRef.instance.listVehicles.Count);
            b_InitDone = true;

            yield return null;
            #endregion
        }

        //-> Time Trial Step 1:  -> Init Game Modules
        public bool bStep3_TT_Countdown(int CountdownID = 0)
        {
            #region
            return Countdown.instance.BCountdown(CountdownID);
            #endregion
        }
     
        //-> Time Trial Step 1:  -> Init Game Modules
        public bool bStep4_TT_EnablePlayerToMove()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(bStep4_TT_EnablePlayerToMoveRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator bStep4_TT_EnablePlayerToMoveRoutine()
        {
            #region
            b_InitDone = false;
            //Debug.Log("Step 4: Enable Player to move.");

            for (var i = 0; i < VehiclesRef.instance.listVehicles.Count; i++)
            {
                if(i < InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer)
                {
                    VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled = false;
                    VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInfo>().b_IsPlayerInputAvailable = true;               // Allow the player to move the plane
                    VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().aiSmoothStart = 0;
                    yield return new WaitUntil(() => !VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled);
                }
                else
                {
                    VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled = true;
                    VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInfo>().b_IsPlayerInputAvailable = true;
                    VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().aiSmoothStart = 0;
                   yield return new WaitUntil(() => VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled);
                }
            }
            b_InitDone = true;

            //-> Disable Pause Mode
            PauseManager.instance.isPauseModeEnable = true;
            yield return null;
            #endregion
        }

        //-> Camera system during the race cointdown
        public bool BStep5CameraPreRace()
        {
            #region
            CamDuringCoundown.instance.BVehiclePresentationCountdown();
            return true;
            #endregion
        }

        //-> Camera system during the race cointdown
        public bool BStep6EnableLapAndPositionSystem()
        {
            #region
            LapCounterAndPosition.instance.bAllowPositionToBeUpdated = true;
            return true;
            #endregion
        }


        //-> Race Complete Test
        public bool BStep7RaceComplete()
        {
            #region
            PauseManager.instance.isPauseModeEnable = false;
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[9].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(9);
            return true;
            #endregion
        }


        //-> Time Trial Step 0:  -> Instantiate Vehicle
        public bool bInitTrackFolder()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(bInitTrackFolderRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator bInitTrackFolderRoutine()
        {
            #region
            b_InitDone = false;


            GameObject objTrack = PathRef.instance.Track.gameObject;

            //-> Move AltPathTrigger objects inside Grp_AltPathTrigger obj (folder)
            TriggerAltPath[] children = objTrack.GetComponentsInChildren<TriggerAltPath>();
            int counter = 0;
            foreach(TriggerAltPath child in children)
            {
                child.transform.SetParent(PathRef.instance.Grp_AltPathTigger.transform);
                yield return new WaitUntil(() => child.transform.parent == PathRef.instance.Grp_AltPathTigger.transform);
                counter++;
            }

            yield return new WaitUntil(() => counter == children.Length);


            //-> Move AltPath objects inside Grp_AltPathTrigger obj (folder)
            AltPath[] children3 = objTrack.GetComponentsInChildren<AltPath>();
            int counter3 = 0;
            foreach (AltPath child in children3)
            {
                child.transform.SetParent(PathRef.instance.Grp_AltPathTigger.transform);
                yield return new WaitUntil(() => child.transform.parent == PathRef.instance.Grp_AltPathTigger.transform);
                counter3++;
            }

            yield return new WaitUntil(() => counter3 + counter == children.Length + children3.Length);



            //Debug.Log("PathRef.instance.Track.AltPathList.Count: " + PathRef.instance.Track.AltPathList.Count);
            for (var i = 0;i< PathRef.instance.Track.AltPathList.Count; i++)
            {
                AltPathPlayerTrigger[] children2 = PathRef.instance.Track.AltPathList[i].GetComponentsInChildren<AltPathPlayerTrigger>();
                counter = 0;
                foreach (AltPathPlayerTrigger child in children2)
                {
                    child.transform.SetParent(PathRef.instance.Grp_AltPathTigger.transform);
                    yield return new WaitUntil(() => child.transform.parent == PathRef.instance.Grp_AltPathTigger.transform);
                    counter++;
                }
                yield return new WaitUntil(() => counter == children2.Length);
            }
            b_InitDone = true;

            yield return null;
            #endregion
        }


        public bool BStep7RaceCompleteTimeTrial(float duration = 3)
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(BStep7RaceCompleteTimeTrialRoutine(duration));
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }


        IEnumerator BStep7RaceCompleteTimeTrialRoutine(float duration = 3)
        {
            #region
            b_InitDone = false;

            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }

            PauseManager.instance.isPauseModeEnable = false;
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[9].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(9);

           
            b_InitDone = true;

            yield return null;
            #endregion
        }

        public bool BStep7RaceCompleteArcade(float duration = 3)
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(BStep7RaceCompleteArcadeRoutine(duration));
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }


        IEnumerator BStep7RaceCompleteArcadeRoutine(float duration = 3)
        {
            #region
            b_InitDone = false;
            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }

            PauseManager.instance.isPauseModeEnable = false;
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[14].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(14);

            b_InitDone = true;

            yield return null;
            #endregion
        }

        public bool BStep7RaceCompleteChampionship(float duration = 3)
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
               
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(BStep7RaceCompleteChampionshipRoutine(duration));
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }


        IEnumerator BStep7RaceCompleteChampionshipRoutine(float duration = 3)
        {
            #region
            b_InitDone = false;
           
            float t = 0;

            while (t < duration)
            {
                t += Time.deltaTime;
                yield return null;
            }

            PauseManager.instance.isPauseModeEnable = false;
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[15].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(15);

            b_InitDone = true;

            yield return null;
            #endregion
        }

        public bool BFlagAllowed()
        {
            #region
            VehicleFlagManager.instance.bFlagAllowed = true;
            StartCoroutine(VehicleFlagManager.instance.FlagColorFadeRoutine());

            return true;
            #endregion
        }

        public bool BModeFiveDisableStartLine()
        {
            #region
            StartLine.instance.Grp_StartLineColliders.gameObject.SetActive(false);
            return true;
            #endregion
        }

        public bool BPlayerTwoAI()
        {
            #region
            VehiclesRef.instance.listVehicles[1].GetComponent<VehicleAI>().enabled = true;
            return true;
            #endregion
        }


        public bool BInitTuto(int ID = 0)
        {
            #region
            FindObjTutoManager(ID).InitTuto();
            return true;
            #endregion
        }

        TutoManager FindObjTutoManager(int ID)
        {
            TutoManager[] tutos = FindObjectsOfType<TutoManager>();

            foreach (TutoManager tuto in tutos)
                if (tuto.ID == ID)
                {
                    return tuto;
                }

            return null;
        }

        //-> Cursor Visibility
        public bool ShowCursor()
        {
            Cursor.visible = true;
            return true;
        }

        //->Mode 6:  -> Instantiate Vehicle
        public bool BInitModeSpectator(string sData)
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitModeSpectatorRoutine(sData));
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator InitModeSpectatorRoutine(string sData)
        {
            #region
            b_InitDone = false;

            string[] codes = sData.Split('_');

            List<int> HowManyVehicleToInstantiate = new List<int>();
            int HowManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.howManyVehicleInSelectedGameMode;

            if (GameModeGlobal.instance.vehicleIDList.Count == 0)
            {
                for (var i = 0; i < codes.Length; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(int.Parse(codes[i]));
                    HowManyVehicleToInstantiate.Add(int.Parse(codes[i]));
                }
            }
            else
            {
                for (var i = 0; i < GameModeGlobal.instance.vehicleIDList.Count; i++)
                {
                    HowManyVehicleToInstantiate.Add(GameModeGlobal.instance.vehicleIDList[i]);
                }
            }


            yield return new WaitUntil(() => VehiclesRef.instance.bInstantiateVehicle(HowManyVehicleToInstantiate) == true);


            b_InitDone = true;
            yield return null;
            #endregion
        }

        public bool BDisableInGameCanvas()
        {
            CanvasInGameTag.instance.gameObject.SetActive(false);
            return true;
        }

        //->Mode 6:  -> Instantiate Vehicle
        public bool BAllVehicleWithAI()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(AllVehicleWithAIRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator AllVehicleWithAIRoutine()
        {
            #region
            b_InitDone = false;

           
            for (var i = 0; i < VehiclesRef.instance.listVehicles.Count; i++)
            {
                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled = true;
                VehiclesRef.instance.listVehicles[i].GetComponent<VehicleInfo>().b_IsPlayerInputAvailable = true;
                yield return new WaitUntil(() => VehiclesRef.instance.listVehicles[i].GetComponent<VehicleAI>().enabled);
            }

            LapCounterAndPosition.instance.bAllowPositionToBeUpdated = true;

            b_InitDone = true;
            yield return null;
            #endregion
        }

        //-> Mode 6: -> TV System
        public bool bInitTVSystem(int targetID)
        {
            #region
            if (TVSystem.instance)
                TVSystem.instance.bInitTVSystem(targetID);
            return b_InitDone;
            #endregion
        }

       
    }
}