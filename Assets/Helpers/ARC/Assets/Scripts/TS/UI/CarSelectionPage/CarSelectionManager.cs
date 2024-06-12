// Description: CarSelectionManager: Select a vehicle in the car selection menu (Main Menu scene)
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TS.Generics
{
    public class CarSelectionManager : MonoBehaviour
    {
        public static CarSelectionManager instance;
        public int TSInputKeyLeft = 3;
        public int TSInputKeyRight = 4;

        public CarSelectionAssistant carSelectionAssistantP1;
        public CarSelectionAssistant carSelectionAssistantP2;

        public GameObject grpP1;
        public GameObject grpP2;

        public GameObject grpCamP1;
        public GameObject grpCamP2;

        bool bInitDone = false;

        public List<int> vehicleAvailableList = new List<int>();

        void Awake()
        {
            #region
            //Check if instance already exists
            if (instance == null)
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        public void EnterCarSelection()
        {
            StartCoroutine(EnterCarSelectionRoutine());
        }


        public void ExitCarSelection()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;

            grpCamP1.SetActive(false);
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
                grpCamP2.SetActive(false);

            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }


        public void PlayerOneLeft()
        {
            if (grpP1.activeInHierarchy)
            {
                carSelectionAssistantP1.DisplayNewVehicle(-1);
            }
        }

        public void PlayerOneRight()
        {
            if (grpP1.activeInHierarchy)
            {
                carSelectionAssistantP1.DisplayNewVehicle(1);
            }
        }


        public void PlayerTwoLeft()
        {
            if (grpP2.activeInHierarchy)
            {
                carSelectionAssistantP2.DisplayNewVehicle(-1);
            }
        }

        public void PlayerTwoRight()
        {
            if (grpP2.activeInHierarchy)
            {
                carSelectionAssistantP2.DisplayNewVehicle(1);
            }
        }

        public void StateGrpCamP1(bool value,bool bDisplayCar = true)
        {
            grpCamP1.SetActive(value);
            if (bDisplayCar) carSelectionAssistantP1.DisplayNewVehicle(0);
        }
        public void StateGrpCamP2(bool value, bool bDisplayCar = true)
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
            {
                grpCamP2.SetActive(value);
                if(bDisplayCar) carSelectionAssistantP2.DisplayNewVehicle(0);

            }
        }

        public void OnDestroy()
        {
            if (bInitDone)
            {
                //-> Left (TSInputKeyLeft = 3) | Right (TSInputKeyRight = 4)
                InfoInputs.instance.ListOfInputsForEachPlayer[1].listOfButtons[TSInputKeyLeft].OnGetKeyDownReceived -= PlayerTwoLeft;
                InfoInputs.instance.ListOfInputsForEachPlayer[1].listOfButtons[TSInputKeyRight].OnGetKeyDownReceived -= PlayerTwoRight;
            } 
        }

        public IEnumerator EnterCarSelectionRoutine()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            if (!bInitDone)
            {
                //-> Left (TSInputKeyLeft = 3) | Right (TSInputKeyRight = 4)
                InfoInputs.instance.ListOfInputsForEachPlayer[1].listOfButtons[TSInputKeyLeft].OnGetKeyDownReceived += PlayerTwoLeft;
                InfoInputs.instance.ListOfInputsForEachPlayer[1].listOfButtons[TSInputKeyRight].OnGetKeyDownReceived += PlayerTwoRight;
                bInitDone = true;
            }


            // Create the list vehicle available
            vehicleAvailableList.Clear();
            int vehicleTotal = InfoVehicle.instance.vehicleParametersInGameList.Count;
            int counter = 0;
            for(var i = 0; i< vehicleTotal; i++)
            {
                if (InfoVehicle.instance.vehicleParametersInGameList[i].isUnlocked)
                {
                    vehicleAvailableList.Add(i);
                    counter++;
                } 
            }

           grpCamP1.SetActive(true);
            //carSelectionAssistantP1.bForceLoadingVehicle = true;
            carSelectionAssistantP1.DisplayNewVehicle(0);


            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
            {
                grpCamP2.SetActive(true);
                //carSelectionAssistantP2.bForceLoadingVehicle = true;
                carSelectionAssistantP2.DisplayNewVehicle(0);
            }

            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            yield return null;
        }
    }
}