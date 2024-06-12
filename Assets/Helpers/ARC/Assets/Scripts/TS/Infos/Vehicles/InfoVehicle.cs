// Description: InfoVehicle: Access from anywhere info about vehicles (unlocked vehicles, P1 P2 selected vehicle ID, Vehicles parameters).
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class InfoVehicle : MonoBehaviour
    {
        public static InfoVehicle       instance = null;
        public List<bool>               listVehicleUnlockState  = new List<bool>();     // Remember if a vehicle is unlocked is unlocked.
        public int                      currentVehicleDisplayedInTheGarage = 0;
        public List<int>                listSelectedVehicles    = new List<int>();      // Remember selected vehicles.
        public List<VehicleGlobalData.CarParameters> vehicleParametersInGameList = new List<VehicleGlobalData.CarParameters>();


        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        public void Init(string sData)
        {
            //-> Init vehicleParametersInGameList
            for (var i = 0; i < DataRef.instance.vehicleGlobalData.carParametersList.Count; i++)
                vehicleParametersInGameList.Add(new VehicleGlobalData.CarParameters(DataRef.instance.vehicleGlobalData.carParametersList[i]));

            //Debug.Log("ini: ''" + sData + "''");
            if (sData == "")
            {

            }
            else
            {
                string[] codes = sData.Split('_');
                int counter = 0;
                int howManyEntries = int.Parse(codes[counter]);
                counter++;
                //-> Update unlock State
                for (var i = 0; i < howManyEntries; i++)
                {
                    vehicleParametersInGameList[i].isUnlocked = TrueFalse(codes[counter]);
                    counter++;
                }

                howManyEntries = int.Parse(codes[counter]);
                counter++;
                //-> Update show in vehicle selection.
                for (var i = 0 + counter; i < howManyEntries; i++)
                {
                    vehicleParametersInGameList[i].bShow = TrueFalse(codes[counter]);
                    
                    counter++;
                }
            }
        }

        bool TrueFalse(string value)
        {
            if (value == "T") return true;
            else return false;
        }
    }
}

