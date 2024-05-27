// Description: GrpPowerUp: Used to select a Power-Up during the race.
// Used to create a new Power-up in the editor.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class GrpPowerUp : MonoBehaviour, IPowerSelection<PowerUpsList>
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;
        [HideInInspector]
        public int currentEditorTab;

        // Use for the Power Editor creation
        public int currentPowerUp;
        public List<int> listPowerUpCreation = new List<int>();

        public int forcePowerUp = -1;

        // Ingame variable
        [System.Serializable]
        public class LookAtPowerUp
        {
            public bool b_PowerUpAvailable = true;
            public Transform powerUpToLookAT;
            public List<Transform> checkpointsRef = new List<Transform>();
        }

        public List<LookAtPowerUp> listLookAtPowerUp = new List<LookAtPowerUp>();


        public bool b_Random = true;

        public Transform objInsertAfter;
        public Transform objInsertBefore;

        public Path path;

        [System.Serializable]
        public class PowerUpObjRules
        {
            public string name;
            public GameObject objSelectionsRules;
            public int ID;
        }

        public List<PowerUpObjRules> objSelectionRulesList = new List<PowerUpObjRules>();
        private List<IPowerSelection<PowerUpsList>> powerSelections = new List<IPowerSelection<PowerUpsList>>();


        [HideInInspector]
        public int currentRandomPUSelected = 0;

        public void Start()
        {
            for(var i = 0;i< objSelectionRulesList.Count; i++)
            {
                powerSelections.Add(objSelectionRulesList[i].objSelectionsRules.GetComponent<IPowerSelection<PowerUpsList>>());
            }
        }

        // Call by VehiclePathFollow
        public Transform SelectAPowerUp(int VehicleNumber)
        {
            //-> Force a Power Up
            if (forcePowerUp != -1)
            {
                return listLookAtPowerUp[forcePowerUp].powerUpToLookAT;
            }

            //-> Create a list of power-up available
            for (var i = 0;i< listLookAtPowerUp.Count; i++)
            {
                if (listLookAtPowerUp[i].powerUpToLookAT &&
                    !listLookAtPowerUp[i].powerUpToLookAT.gameObject.activeSelf)
                    listLookAtPowerUp[i].b_PowerUpAvailable = true;
            }

            List<int> listPowerUpAvailable = new List<int>();
            for (var i = 0; i < listLookAtPowerUp.Count; i++)
            {
                if (listLookAtPowerUp[i].powerUpToLookAT &&
                    listLookAtPowerUp[i].powerUpToLookAT.gameObject.activeSelf &&
                    listLookAtPowerUp[i].b_PowerUpAvailable)
                {
                    listPowerUpAvailable.Add(i);
                }
            }

            //-> Choose Power-up using IPowerSelection<PowerUpsList> Interface
            for (var i = 0; i < powerSelections.Count; i++){
                PowerUpsList pUL = new PowerUpsList(listPowerUpAvailable, VehicleNumber, objSelectionRulesList[i].ID);
                Transform trans = powerSelections[i].PowerUpSelectionRules(pUL);
                if (trans) return trans;
            }

            return null;
        }


        bool IsPowerUpAvailable(int value)
        {
            if (listLookAtPowerUp[value].powerUpToLookAT &&
                listLookAtPowerUp[value].powerUpToLookAT.gameObject.activeSelf &&
                listLookAtPowerUp[value].b_PowerUpAvailable)
                return true;
            else
                return false;
        }

        bool IsAiInFrontOffPlayers(int powerUpID, int value,int vehicleNumber)
        {
            if (listLookAtPowerUp[value].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == powerUpID &&
                LapCounterAndPosition.instance.posList[vehicleNumber].RacePos < LapCounterAndPosition.instance.posList[0].RacePos

                ||

                listLookAtPowerUp[value].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == powerUpID &&
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2 &&
                LapCounterAndPosition.instance.posList[vehicleNumber].RacePos < LapCounterAndPosition.instance.posList[1].RacePos)
                return true;
            else
                return false;
        }

        public Transform ShieldRules(PowerUpsList powerUpsList)
        {
            //Debug.Log("Shield");
            List<int> listPowerUpAvailable = powerUpsList.powerUpsList;
            int VehicleNumber = powerUpsList.vehicleNumber;

            //-> Choose Shield: PowerType == 3
            int randomShield = UnityEngine.Random.Range(0, 100);

            for (var i = 0; i < listPowerUpAvailable.Count; i++)
            {
                if (listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == 3 &&
                    VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<PowerUpsSystem>().currentPowerUps != 3 &&
                    IsPowerUpAvailable(listPowerUpAvailable[i]))
                {
                    if (// Ai is in 1st position he has 70% to choose Shield Power-Up
                        randomShield < 70 &&
                        LapCounterAndPosition.instance.posList[VehicleNumber].RacePos == 0 &&
                        VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<VehicleDamage>().lifePoints > 5
                        ||
                        // Shield if AI life <= 5 && 10%
                        randomShield < 10 &&
                        VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<VehicleDamage>().lifePoints <= 5)
                    {
                        listLookAtPowerUp[listPowerUpAvailable[i]].b_PowerUpAvailable = false;
                        return listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT;
                    }
                }
            }
            return null;
        }

        public Transform RepairRules(PowerUpsList powerUpsList)
        {
            //Debug.Log("Repair");
            List<int> listPowerUpAvailable = powerUpsList.powerUpsList;
            int VehicleNumber = powerUpsList.vehicleNumber;

            //-> Repair PowerType == 1
            int randomRepair = UnityEngine.Random.Range(0, 100);
            //Debug.Log("randomShield: " + randomShield + " | " + "randomRepair: " + randomRepair);
            for (var i = 0; i < listPowerUpAvailable.Count; i++)
            {
                if (listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == 1 &&
                    VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<PowerUpsSystem>().currentPowerUps != 1 &&
                    IsPowerUpAvailable(listPowerUpAvailable[i]))
                {
                    if (// If Ai is in 1st position he has 70% to choose Repair Power-Up
                        randomRepair < 50 &&
                        LapCounterAndPosition.instance.posList[VehicleNumber].RacePos == 0
                        ||
                        // Repair if AI life <= 5 && 90%
                        randomRepair < 90 &&
                        VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<VehicleDamage>().lifePoints <= 5)
                    {
                        listLookAtPowerUp[listPowerUpAvailable[i]].b_PowerUpAvailable = false;
                        return listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT;
                    }
                }
            }
            return null;
        }

        public Transform MineRules(PowerUpsList powerUpsList)
        {
            //Debug.Log("Mine");
            List<int> listPowerUpAvailable = powerUpsList.powerUpsList;
            int VehicleNumber = powerUpsList.vehicleNumber;

            //-> Choose Mine PowerType == 5 (If Ai is in 1st position he has 70% to choose Mine Power-Up). 
            int randomMine = UnityEngine.Random.Range(0, 100);

            for (var i = 0; i < listPowerUpAvailable.Count; i++)
            {
                if (listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == 5 &&
                    VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<PowerUpsSystem>().currentPowerUps != 5 &&
                    IsPowerUpAvailable(listPowerUpAvailable[i]))
                {
                    // Mine: If Ai is in 1st position he has 40% to choose Repair Power-Up
                    if (randomMine < 40 &&
                        LapCounterAndPosition.instance.posList[VehicleNumber].RacePos == 0
                        ||
                        // Mine: if AI has 25%
                        randomMine < 25)
                    {
                        listLookAtPowerUp[listPowerUpAvailable[i]].b_PowerUpAvailable = false;
                        return listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT;
                    }
                }
            }
            return null;
        }

        public Transform MachineGunRules(PowerUpsList powerUpsList)
        {
            //Debug.Log("MachineGun");
            List<int> listPowerUpAvailable = powerUpsList.powerUpsList;
            int VehicleNumber = powerUpsList.vehicleNumber;

            //-> Choose Machine Gun PowerType == 2
            int randomMachinGun = UnityEngine.Random.Range(0, 100);

            for (var i = 0; i < listPowerUpAvailable.Count; i++)
            {
                if (IsPowerUpAvailable(listPowerUpAvailable[i]) &&
                    listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == 2)
                {
                    if (//-> Random value > 50
                        randomMachinGun < 50 &&
                        LapCounterAndPosition.instance.posList[VehicleNumber].RacePos != 0
                        ||
                        //-> Random value > 70 or P1 or P2 is in front of AI
                        randomMachinGun < 70 &&
                        !IsAiInFrontOffPlayers(2, listPowerUpAvailable[i], VehicleNumber))
                    {
                        listLookAtPowerUp[listPowerUpAvailable[i]].b_PowerUpAvailable = false;
                        //Debug.Log("MachineGun: " + VehicleNumber);
                        return listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT;
                    }
                }
            }

            return null;
        }

        public Transform MissileRules(PowerUpsList powerUpsList)
        {
            //Debug.Log("Missile");
            List<int> listPowerUpAvailable = powerUpsList.powerUpsList;
            int VehicleNumber = powerUpsList.vehicleNumber;

            //-> Choose Missile (Player is in front of the CPU) PowerType == 6
            int randomMissile = UnityEngine.Random.Range(0, 100);

            for (var i = 0; i < listPowerUpAvailable.Count; i++)
            {
                if (IsPowerUpAvailable(listPowerUpAvailable[i]) &&
                    listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT.GetComponent<PowerUpsItems>().PowerType == 6 &&
                    VehiclesRef.instance.listVehicles[VehicleNumber].GetComponent<PowerUpsSystem>().currentPowerUps != 6)
                {
                    if (//-> Random value > 50
                       randomMissile < 50 &&
                       LapCounterAndPosition.instance.posList[VehicleNumber].RacePos != 0
                       ||
                       //-> Random value > 30 && P1 or P2 is in front of AI
                       randomMissile < 30 &&
                       !IsAiInFrontOffPlayers(6, listPowerUpAvailable[i], VehicleNumber))
                    {
                        listLookAtPowerUp[listPowerUpAvailable[i]].b_PowerUpAvailable = false;
                        //Debug.Log("Missile: " + VehicleNumber);
                        return listLookAtPowerUp[listPowerUpAvailable[i]].powerUpToLookAT;
                    }
                }
            }
            return null;
        }


        public Transform PowerUpSelectionRules(PowerUpsList powerUpsList)
        {
            switch (powerUpsList.ID)
            {
                case 0:
                    Transform trans = ShieldRules(powerUpsList);
                    if (trans) return trans;
                    break;
                case 1:
                     trans = RepairRules(powerUpsList);
                    if (trans) return trans;
                    break;
                case 2:
                     trans = MineRules(powerUpsList);
                    if (trans) return trans;
                    break;
                case 3:
                     trans = MachineGunRules(powerUpsList);
                    if (trans) return trans;
                    break;
                case 4:
                     trans = MissileRules(powerUpsList);
                    if (trans) return trans;
                    break;
            }

            return null;
        }
    }

}
