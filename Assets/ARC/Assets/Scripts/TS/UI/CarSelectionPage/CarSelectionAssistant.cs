// Description: CarSelectionAssistant: called when a vehicle is selected in the car selection menu (Main Menu scene)
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace TS.Generics
{
    public class CarSelectionAssistant : MonoBehaviour
    {
        public bool                 b_IsInitGarageInProcess = false;

        public bool                 bActionAvailable;

        public UnityEvent           newVechicleEvent;                   // List of events use when a new vehicle is loaded
        public UnityEvent           instantiateNewVehicleEvent;         // Manage the vehicle instantiation

        public CurrentText          HowManyVehicleAvailable;
        public CurrentText          VehicleName;

        //-> Use to set up the transition (duration and curve)
        public float                duration = 1;                       // the transition duration
        public AnimationCurve       animSpeedCurve;


        public int selectedPlayer = 0;
        [HideInInspector]
        public GarageTagPivot garageTagPivot;
        public Transform vehicleInstantiatePosition;

        public int direction;

        public int posInList;
        [HideInInspector]
        public bool bForceReloadingVehicleInVehicleChoose;

        //-> Call by Page Init (garage page) | Call when Button_PreviousVehicle or Button_NextVehicle are pressed
        public void DisplayNewVehicle(int Direction)
        {
            if (bActionAvailable) StartCoroutine(DisplayNextVehicleRoutine(Direction));
        }

        //-> direction 0: Init menu | 1: next vehicle | -1: Previous vehicle
        IEnumerator DisplayNextVehicleRoutine(int _direction)
        {
            direction = _direction;
            bActionAvailable = false;
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;

            VehicleGlobalData vehicleData = DataRef.instance.vehicleGlobalData;
            InfoVehicle infoVehicle = InfoVehicle.instance;

            List<int> vehicleAvailableList = CarSelectionManager.instance.vehicleAvailableList;

            //-> Find the vehicle to display. Check if bShow in vehicle parameters is set to True.
            bool bNewEntry = false;
            while (bNewEntry != true)
            {
                //-> Use list Order
                if (!vehicleData.OrderUsingCustomList)
                {
                    posInList += direction + vehicleAvailableList.Count + vehicleAvailableList.Count;
                    posInList %= vehicleAvailableList.Count;

                    infoVehicle.listSelectedVehicles[selectedPlayer - 1] = vehicleAvailableList[posInList];
                }
                //-> Use Custom Order
                else
                {
                    infoVehicle.listSelectedVehicles[selectedPlayer - 1] += direction + vehicleAvailableList.Count + vehicleAvailableList.Count;
                    infoVehicle.listSelectedVehicles[selectedPlayer - 1] %= vehicleAvailableList.Count;
                }

                if (InfoVehicle.instance.vehicleParametersInGameList[infoVehicle.listSelectedVehicles[selectedPlayer - 1]].bShow &&
                    InfoVehicle.instance.vehicleParametersInGameList[infoVehicle.listSelectedVehicles[selectedPlayer - 1]].isUnlocked)
                    bNewEntry = true;

                yield return null;  
            }

            //-> Use list Order
            int currentVehicle = infoVehicle.listSelectedVehicles[selectedPlayer - 1];
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.listSelectedVehicles[selectedPlayer - 1]];

            //-> Display the new vehicle info in UI
            UpdateVehicleInfo(currentVehicle, vehicleData);

            instantiateNewVehicleEvent?.Invoke();

            yield return null;
        }

        //-> Display vehicle info 
        void UpdateVehicleInfo(int currentVehicle, VehicleGlobalData vehicleData)
        {
            newVechicleEvent?.Invoke();
            //-> Display Vehicle name
            VehicleName.DisplayTextComponent(VehicleName.gameObject, InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].name, false);

            string newTxt = (posInList + 1) + "/" + CarSelectionManager.instance.vehicleAvailableList.Count;

            HowManyVehicleAvailable.DisplayTextComponent(VehicleName.gameObject, newTxt, false);
        }

        public void InstantiateNewVehicle()
        {
            StartCoroutine(InstantiateNewVehicleRoutine());
        }

        IEnumerator InstantiateNewVehicleRoutine()
        {
            CarSelectionAssistant[] objsSel = FindObjectsOfType<CarSelectionAssistant>();
            CarSelectionAssistant gm = null;
            for (var i = 0; i < objsSel.Length; i++)
            {
                if (objsSel[i].selectedPlayer == selectedPlayer)
                {
                    gm = objsSel[i];
                    break;
                }
            }

            VehicleGlobalData vehicleData = DataRef.instance.vehicleGlobalData;
            InfoVehicle infoVehicle = InfoVehicle.instance;

            //-> Use list Order
            int currentVehicle = infoVehicle.listSelectedVehicles[selectedPlayer - 1];
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.listSelectedVehicles[selectedPlayer - 1]];

            //-> Check if vehicle is already displayed
            GameObject currentObjVehicle = null;
            if (gm.garageTagPivot && gm.garageTagPivot.transform.childCount > 0)
            { currentObjVehicle = gm.garageTagPivot.transform.GetChild(0).gameObject; }

            if (!gm.garageTagPivot)
            {
                GarageTagPivot[] objs = FindObjectsOfType<GarageTagPivot>();

                for (var i = 0; i < objs.Length; i++)
                {
                    if (objs[i].ID == selectedPlayer)
                    {
                        gm.garageTagPivot = objs[i];
                    }

                    if (objs[i].ID == selectedPlayer + 3)
                    {
                        gm.vehicleInstantiatePosition = objs[i].transform;
                    }
                }
            }

            //-> Set the Ref transform depending direction
            Transform startParent = gm.garageTagPivot.backPos;
            Transform endParent = gm.garageTagPivot.frontPos;
            if (gm.direction == -1)
            {
                startParent = gm.garageTagPivot.frontPos;
                endParent = gm.garageTagPivot.backPos;
            }

            //-> instantiate new Vehicle
            if (currentObjVehicle && gm.direction != 0 ||
                !currentObjVehicle && gm.direction == 0 ||
                bForceReloadingVehicleInVehicleChoose)
            {
                //-> Instantiate the new vehicle
                GameObject newVehicle = Instantiate(InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].Prefab, gm.vehicleInstantiatePosition);      // garageTagPivot.transform);
                newVehicle.transform.localScale *= InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].prefabScaleMultiplierInMenu;
                yield return new WaitUntil(() => newVehicle.GetComponent<VehiclePrefabInit>().bInitVehicleInfo(1));  // Init for Main menu
                newVehicle.transform.SetParent(startParent);
                newVehicle.transform.position = startParent.position;
                newVehicle.transform.localRotation = Quaternion.identity;

                //-> Move the vehicles (Previous and current) to a new position
                float t = 0;
                while (t < 1)
                {
                    t += Time.deltaTime / gm.duration;
                    newVehicle.transform.position = Vector3.Lerp(startParent.position, gm.garageTagPivot.transform.position, gm.animSpeedCurve.Evaluate(t));

                    if (currentObjVehicle)
                        currentObjVehicle.transform.position = Vector3.Lerp(gm.garageTagPivot.transform.position, endParent.position, gm.animSpeedCurve.Evaluate(t));
                    yield return null;
                }
                yield return new WaitUntil(() => newVehicle.GetComponent<VehiclePrefabInit>().b_InitDone);

                //-> Delete the old vehicle 
                if (gm.garageTagPivot.transform.childCount > 0)
                { if (currentObjVehicle) Destroy(currentObjVehicle); }

                newVehicle.transform.SetParent(gm.garageTagPivot.transform);
                newVehicle.transform.localRotation = Quaternion.identity;
                bForceReloadingVehicleInVehicleChoose = false;
            }


            //-> IMPORTANT: Next 2 lines closed the process. Always add those 2 lines.
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            gm.bActionAvailable = true;
            yield return null;
        }
    }
}
