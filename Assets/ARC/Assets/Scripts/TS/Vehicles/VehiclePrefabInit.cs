using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class VehiclePrefabInit : MonoBehaviour
    {
        public bool b_AutoInit;
        public bool b_InitDone;
        private bool b_InitInProgress;
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.

        public int currentSelectedList;
        public int currentDisplayedList;

        public int vehicleDataID;

        [System.Serializable]
        public class initVehiclePrefab
        {
            public string name;
            public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
                = new List<EditorMethodsList_Pc.MethodsList>();
        }

        public List<initVehiclePrefab> initVehiclePrefabList = new List<initVehiclePrefab>();


        public int playerNumber = 0;

        public VehicleInfo vehicleInfo;

        private void Start()
        {
            if(b_AutoInit)
                StartCoroutine(InitVehicleRoutine(currentSelectedList));
        }

        private void OnDestroy()
        {
            
        }

        //-> Initialisation
        public bool bInitVehicleInfo(int whichInit,int playerID = 0, int vehicleID = 0)
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitVehicleRoutine(whichInit, playerID, vehicleID));
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        //-> Call all the methods in the list 
        IEnumerator InitVehicleRoutine(int whichMethodist = 0, int playerID = 0, int vehicleID = 0)
        {
            #region
            b_InitDone = false;
            playerNumber = playerID;
            vehicleDataID = vehicleID;

            for (var i = 0; i < initVehiclePrefabList[whichMethodist].methodsList.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(initVehiclePrefabList[whichMethodist].methodsList, i) == true);
            }

            b_InitDone = true;

            yield return null;
            #endregion
        }
    }
}

