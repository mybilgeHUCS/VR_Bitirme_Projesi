using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "VehicleGlobalData", menuName = "TS/VehicleGlobalData")]
    public class VehicleGlobalData : ScriptableObject
    {
        public bool MoreOptions;
        public bool HelpBox;

        public int  tab = 0;

        [System.Serializable]
        public class CarParameters
        {
            public string       name = "TD-352";
            public bool         bShow = true;
            public GameObject   Prefab;

            public int          vehicleCategory = 0;        // 0: Easy | 1: Medium | 2: Difficult
            public float        speed;
            public int          damageResistance;
            public float        boosterPower;
            public float        boosterDuration;
            public float        boosterCooldown;
            public float        coinMultiplier = 1;

            public int          cost = 10000;

            public bool         isUnlocked = false;

            public float        speedAI;
            public int          damageResistanceAI;
            public float        boosterPowerAI;
            public float        boosterDurationAI;
            public float        boosterCooldownAI;
            public float        prefabScaleMultiplierInMenu = 1;
            public bool         bShowInEditor = true;

            public CarParameters(CarParameters carParams)
            {
                name                = carParams.name;
                bShow               = carParams.bShow;
                Prefab              = carParams.Prefab;
                speed               = carParams.speed;
                vehicleCategory     = carParams.vehicleCategory;
                damageResistance    = carParams.damageResistance;
                boosterPower        = carParams.boosterPower;
                boosterDuration     = carParams.boosterDuration;
                boosterCooldown     = carParams.boosterCooldown;
                coinMultiplier      = carParams.coinMultiplier;
                cost                = carParams.cost;
                isUnlocked          = carParams.isUnlocked;

                speedAI             = carParams.speedAI;
                damageResistanceAI  = carParams.damageResistanceAI;
                boosterPowerAI      = carParams.boosterPowerAI;
                boosterDurationAI   = carParams.boosterDurationAI;
                boosterCooldownAI   = carParams.boosterCooldownAI;
                prefabScaleMultiplierInMenu = carParams.prefabScaleMultiplierInMenu;
                bShowInEditor       = carParams.bShowInEditor;
            }
        }

        public List<CarParameters> carParametersList = new List<CarParameters>();

        public int currentVehicleDisplayedInTheGarage;
        public bool OrderUsingCustomList = false;

        public List<int> customList = new List<int>();


        [System.Serializable]
        public class Texts
        {
            public string paramName = "";
            public int listID = 0;
            public int EntryID = 0;

            public Texts(string v0, int v1, int v2)
            {
                paramName = v0;
                listID = v1;
                EntryID = v2;
            }
        }

        public List<Texts> playerNamesList = new List<Texts>();
        public List<Texts> aiNamesList = new List<Texts>();

        public bool aiNameRandom = true; 
    }
}

