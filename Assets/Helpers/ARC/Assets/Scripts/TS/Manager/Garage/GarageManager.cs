//Description: GarageManager. Attached to GarageManager in the Main Menu scene
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Globalization;


namespace TS.Generics
{
    public class GarageManager : MonoBehaviour
    {
        public static GarageManager instance = null;                    //Static instance allows to be accessed by any other script.
        public bool                 b_IsInitGarageInProcess = false;
        public int                  garagePageID = 13;

        public bool                 bActionAvailable;

        public UnityEvent           newVechicleEvent;                   // List of events use when a new vehicle is loaded
        public UnityEvent           instantiateNewVehicleEvent;         // Manage the vehicle instantiation

        public CurrentText          HowManyVehicleAvailable;
        public CurrentText          VehicleName;
        public CurrentText          VehiclePrice;
        public CurrentText          txtOwn;
        public int                  listTextBuy = 0;
        public int                  idTextBuy = 59;
        public int                  listTextOwn = 0;
        public int                  idTextOwn = 113;

        public GameObject           grpBuyInfo;
        public CurrentText          txtBuyInfo;
        public int                  listTextNotEnoughCredits = 0;
        public int                  idTextBuyNotEnoughCredits = 79;
        public int                  listTextAlredyBought = 0;
        public int                  idTextAlredyBought = 114;

        public GameObject           objLock;
        public GameObject           grpPrice;

        //-> Use to set up the transition (duration and curve)
        public float                duration = 1;                       // the transition duration
        public AnimationCurve       animSpeedCurve;
        [HideInInspector]
        public GarageTagPivot      garageTagPivot;
        [HideInInspector]
        public Transform            vehicleInstantiatePosition;

        public GameObject           objLoadVehicle;

        public int direction;
        
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

        //-> Init the garage (Call by the GarageManagerAssistant)
        public IEnumerator OpenGarageRoutine()
        {
            #region
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = false;

            //-> Open the garage page
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[garagePageID].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(garagePageID);
            InfoPlayerTS.instance.b_IsAvailableToDoSomething = true;
            bActionAvailable = true;
           
            yield return null;
            #endregion
        }

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

            //-> Find the vehicle to display. Check if bShow in vehicle parameters is set to True.
            bool bNewEntry = false;
            while (bNewEntry != true)
            {
                //-> Use list Order
                if (!vehicleData.OrderUsingCustomList)
                {
                    infoVehicle.currentVehicleDisplayedInTheGarage += direction + InfoVehicle.instance.vehicleParametersInGameList.Count + InfoVehicle.instance.vehicleParametersInGameList.Count;
                    infoVehicle.currentVehicleDisplayedInTheGarage %= InfoVehicle.instance.vehicleParametersInGameList.Count;
                    if (InfoVehicle.instance.vehicleParametersInGameList[infoVehicle.currentVehicleDisplayedInTheGarage].bShow) bNewEntry = true;

                }
                //-> Use Custom Order
                else
                {
                    infoVehicle.currentVehicleDisplayedInTheGarage += direction + vehicleData.customList.Count + vehicleData.customList.Count;
                    infoVehicle.currentVehicleDisplayedInTheGarage %= vehicleData.customList.Count;

                    if (InfoVehicle.instance.vehicleParametersInGameList[infoVehicle.currentVehicleDisplayedInTheGarage].bShow) bNewEntry = true;
                }
            }

            //-> Use list Order
            int currentVehicle = infoVehicle.currentVehicleDisplayedInTheGarage;
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.currentVehicleDisplayedInTheGarage];


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

            int howManyVehicle = 0;
            int posCurrentCarInListWithOffset = 0;
            //-> Use list Order
            if (!vehicleData.OrderUsingCustomList)
            {
                for (var i = 0; i < InfoVehicle.instance.vehicleParametersInGameList.Count; i++)
                    if (InfoVehicle.instance.vehicleParametersInGameList[i].bShow)
                        howManyVehicle++;

                for (var i = 0; i < InfoVehicle.instance.vehicleParametersInGameList.Count; i++)
                {
                    if (i == currentVehicle)
                        break;

                    if (InfoVehicle.instance.vehicleParametersInGameList[i].bShow)
                        posCurrentCarInListWithOffset++;
                }
            }
            //-> Use Custom Order
            else
            {
                for (var i = 0; i < vehicleData.customList.Count; i++)
                    if (InfoVehicle.instance.vehicleParametersInGameList[vehicleData.customList[i]].bShow)
                        howManyVehicle++;

                for (var i = 0; i < vehicleData.customList.Count; i++)
                {
                    if (i == currentVehicle)
                        break;

                    if (InfoVehicle.instance.vehicleParametersInGameList[vehicleData.customList[i]].bShow)
                        posCurrentCarInListWithOffset++;
                }
            }

            //-> Display the number of vehicle available
            string newTxt = (posCurrentCarInListWithOffset + 1) + "/" + howManyVehicle;
            HowManyVehicleAvailable.DisplayTextComponent(VehicleName.gameObject, newTxt, false);

            //-> Display cost
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";     // Replace , with blank space
            nfi.NumberGroupSizes = new int[] { 3 }; // 1000

            string formatedCoins = InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].cost.ToString("#,0", nfi);
            VehiclePrice.DisplayTextComponent(VehicleName.gameObject, formatedCoins, false);


            //-> Display lock
            if (InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].isUnlocked)
            {
                grpPrice.SetActive(false);
                objLock.SetActive(false);
                txtOwn.NewTextManageByScript(new List<TextEntry>(1) {new TextEntry(listTextOwn, idTextOwn) }, false);
            }
            else
            {
                txtOwn.NewTextManageByScript(new List<TextEntry>(1) { new TextEntry(listTextBuy, idTextBuy) }, false);
                objLock.SetActive(true);
                grpPrice.SetActive(true);
            }  
        }

        //-> Feedback Not enough credit | Already bought
        public void BuyButtonFeedback(int state)
        {
            StopAllCoroutines();
            StartCoroutine(BuyButtonFeedbackRoutine(state));
        }


        IEnumerator BuyButtonFeedbackRoutine(int state)
        {
            Debug.Log("State: " + state);
            //-> Not Enough Credits
            if(state == 0)
            {txtBuyInfo.NewTextWithSpecificID(idTextBuyNotEnoughCredits, listTextNotEnoughCredits);}
            //-> ALready Bought
            else
            {txtBuyInfo.NewTextWithSpecificID(idTextAlredyBought, listTextAlredyBought);}

            grpBuyInfo.SetActive(true);

            for(var i = 0;i< 5; i++)
            {
                float t = 0;
                float durationB = .25f;

                while (t < durationB)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
                grpBuyInfo.SetActive(!grpBuyInfo.activeSelf);
            }

            yield return null;
        }


        public void UnlockVehicle()
        {
            VehicleGlobalData vehicleData = DataRef.instance.vehicleGlobalData;
            InfoVehicle infoVehicle = InfoVehicle.instance;

            //-> Use list Order
            int currentVehicle = infoVehicle.currentVehicleDisplayedInTheGarage;
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.currentVehicleDisplayedInTheGarage];

            InfoCoins.instance.UpdateCoins(-InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].cost);
            InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].isUnlocked = true;

            grpPrice.SetActive(false);
            objLock.SetActive(false);
            txtOwn.NewTextManageByScript(new List<TextEntry>(1) { new TextEntry(listTextOwn, idTextOwn) }, false);

            LoadSavePlayerProgession.instance.SavePlayerProgression();
        }


        public void InstantiateNewVehicle()
        {
            StartCoroutine(InstantiateNewVehicleRoutine());
        }

        IEnumerator InstantiateNewVehicleRoutine()
        {
            GarageManager gm = GarageManager.instance;
            VehicleGlobalData vehicleData = DataRef.instance.vehicleGlobalData;
            InfoVehicle infoVehicle = InfoVehicle.instance;

            //-> Use list Order
            int currentVehicle = infoVehicle.currentVehicleDisplayedInTheGarage;
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.currentVehicleDisplayedInTheGarage];


            //-> Check if vehicle is already displayed
            GameObject currentObjVehicle = null;
            if (gm.garageTagPivot && gm.garageTagPivot.transform.childCount > 0)
            { currentObjVehicle = gm.garageTagPivot.transform.GetChild(0).gameObject; }

            if (!gm.garageTagPivot)
            {
                GarageTagPivot[] objs = FindObjectsOfType<GarageTagPivot>();

                for (var i = 0; i < objs.Length; i++)
                {
                    if (objs[i].ID == 0)
                    {
                        gm.garageTagPivot = objs[i];
                    }

                    if (objs[i].ID == 3)
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
                !currentObjVehicle && gm.direction == 0)
            {

                if (currentObjVehicle)
                    currentObjVehicle.transform.position = endParent.position;

                if (objLoadVehicle) objLoadVehicle.SetActive(true);

                //-> Instantiate the new vehicle
                GameObject newVehicle = Instantiate(InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].Prefab, gm.vehicleInstantiatePosition);      // garageTagPivot.transform);
                newVehicle.transform.localScale *= InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].prefabScaleMultiplierInMenu;
             
                float currentGameTime = Time.time;

                float t = 0;
                while (t < gm.duration)
                {
                    t += Time.deltaTime;
                    yield return null;
                }

                yield return new WaitUntil(() => newVehicle.GetComponent<VehiclePrefabInit>().bInitVehicleInfo(1));  // Init for Main menu
                newVehicle.transform.SetParent(startParent);
                newVehicle.transform.position = startParent.position;
                newVehicle.transform.localRotation = Quaternion.identity;


                //yield return new WaitUntil(() => currentGameTime + .25f < Time.time);  // Init for Main menu
                if (objLoadVehicle) objLoadVehicle.SetActive(false);
                
                newVehicle.transform.position = gm.garageTagPivot.transform.position;


                yield return new WaitUntil(() => newVehicle.GetComponent<VehiclePrefabInit>().b_InitDone);


                //-> Delete the old vehicle 
                if (gm.garageTagPivot.transform.childCount > 0)
                { if (currentObjVehicle) Destroy(currentObjVehicle); }

                newVehicle.transform.SetParent(gm.garageTagPivot.transform);
                newVehicle.transform.localRotation = Quaternion.identity;
            }


            //-> IMPORTANT: Next 2 lines closed the process. Always add those 2 lines.
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            gm.bActionAvailable = true;
            yield return null;
        }
    }
}
