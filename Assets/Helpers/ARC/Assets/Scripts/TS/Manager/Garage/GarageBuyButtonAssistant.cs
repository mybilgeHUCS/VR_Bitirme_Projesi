// Description: GarageBuyButtonAssistant. Attached to Button_Simple_Buy in Page Grp_Garage  (Main Menu)
using UnityEngine;
using System.Collections;

namespace TS.Generics
{
    public class GarageBuyButtonAssistant : MonoBehaviour
    {
        
        public bool CheckIfConditions()
        {
            VehicleGlobalData vehicleData = DataRef.instance.vehicleGlobalData;
            InfoVehicle infoVehicle = InfoVehicle.instance;

            //-> Use list Order
            int currentVehicle = infoVehicle.currentVehicleDisplayedInTheGarage;
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.currentVehicleDisplayedInTheGarage];


            if (InfoCoins.instance.currentPlayerCoins >= InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].cost &&
                !InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].isUnlocked
                )
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }

        public void BuyVehicle()
        {
            GarageManager.instance.UnlockVehicle();
            // Force to reload the vehicle in the vehicle selection page to prevent issu if a vehicle has been earned by the player
            CarSelectionManager.instance.carSelectionAssistantP1.bForceReloadingVehicleInVehicleChoose = true;
        }

        public void WrongResult()
        {
            VehicleGlobalData vehicleData = DataRef.instance.vehicleGlobalData;
            InfoVehicle infoVehicle = InfoVehicle.instance;

            //-> Use list Order
            int currentVehicle = infoVehicle.currentVehicleDisplayedInTheGarage;
            //-> Use Custom Order
            if (vehicleData.OrderUsingCustomList)
                currentVehicle = vehicleData.customList[infoVehicle.currentVehicleDisplayedInTheGarage];


            if (InfoVehicle.instance.vehicleParametersInGameList[currentVehicle].isUnlocked)
            {
                //-> Feedback Already bought
                GarageManager.instance.BuyButtonFeedback(1);
            }
            else
            {
                //-> Feedback Not Enough Credit
                GarageManager.instance.BuyButtonFeedback(0);
            }
        }

       
    }

}
