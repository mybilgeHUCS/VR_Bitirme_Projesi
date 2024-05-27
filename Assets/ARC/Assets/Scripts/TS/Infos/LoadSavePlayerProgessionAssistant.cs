using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;

namespace TS.Generics
{
    public class LoadSavePlayerProgessionAssistant : MonoBehaviour
    {
        public bool b_LoadInProgress = false;
        public bool b_LoadIsFinished = false;


        //-> return the player coins value
        public string sCoinsValue()
        {
            return InfoCoins.instance.currentPlayerCoins.ToString();
        }

      

        //-> Update player coins value
        public bool bUpdateCoinsValue(string parms)
        {
            #region
            //-> Play the coroutine Once
            if (!b_LoadInProgress)
            {
                b_LoadInProgress = true;
                b_LoadIsFinished = false;
                StartCoroutine(IUpdateCoinsValue(parms));
            }
            //-> Check if the coroutine is finished
            else if (b_LoadIsFinished)
                b_LoadInProgress = false;

            return b_LoadIsFinished;
            #endregion
        }

     
        IEnumerator IUpdateCoinsValue(string parms)
        {
            #region
            b_LoadIsFinished = true;
            yield return null;
            #endregion
        }


        //->  Example: (Save) return Mouth
        public string sDayValue()
        {
            return System.DateTime.UtcNow.Month.ToString();
        }

        //-> Example: load Month
        public bool bUpdateMonthValue(string parms)
        {
            #region
            //-> Load the month.
            if(parms != "")
            {
                int month = int.Parse(parms);
                Debug.Log("Month saved: " + month);
            }
            else
                Debug.Log("Month saved: None");
           
            return true;
            #endregion
        }


        
        public string SaveVehicleInfo()
        {
            #region
            string sData = "";

            //-> Save if the vehicle is unlocked
            sData += InfoVehicle.instance.vehicleParametersInGameList.Count;
            sData += "_";
            for (var i = 0; i < InfoVehicle.instance.vehicleParametersInGameList.Count; i++)
            {
                sData += TrueFalse(InfoVehicle.instance.vehicleParametersInGameList[i].isUnlocked);
                sData += "_";
            }

            sData += GameModeChampionship.instance.listChampionshipPosition.Count;
            sData += "_";
            //-> Save if the vehicle is shown in the vehicle selection
            for (var i = 0; i < InfoVehicle.instance.vehicleParametersInGameList.Count; i++)
            {
                sData += TrueFalse(InfoVehicle.instance.vehicleParametersInGameList[i].bShow);
                if (i < GameModeChampionship.instance.listChampionshipPosition.Count - 1) sData += "_";
            }

            return sData;
            #endregion
        }


        public string SaveInfo01()
        {
            #region
            string sData = "";

            sData += "Info_01";

            return sData;
            #endregion
        }

        public bool LoadVehicleInfo(string sData)
        {
            #region
            InfoVehicle.instance.Init(sData);
            return true;
            #endregion
        }


        public bool LoadInfo01(string sData)
        {
            #region
            //Debug.Log(sData);
            return true;
            #endregion
        }



        public string SaveChampionshipModeInfo()
        {
            #region
            string sData = "";
            //-> Save if the championship is unlocked
            sData += GameModeChampionship.instance.listChampionshipState.Count;
            sData += "_";

            for (var i = 0;i< GameModeChampionship.instance.listChampionshipState.Count; i++)
            {
                sData += TrueFalse(GameModeChampionship.instance.listChampionshipState[i]);
                sData += "_";
            }

            sData += GameModeChampionship.instance.listChampionshipPosition.Count;
            sData += "_";
            //-> Save the best ranking for each championship
            for (var i = 0; i < GameModeChampionship.instance.listChampionshipPosition.Count; i++)
            {
                sData += GameModeChampionship.instance.listChampionshipPosition[i];
                if (i < GameModeChampionship.instance.listChampionshipPosition.Count - 1) sData += "_";
            }

            return sData;
            #endregion
        }

        public bool LoadChampionshipModeInfo(string sData)
        {
            #region
            GameModeChampionship.instance.Init(sData);
            return true;
            #endregion
        }

        public string SaveArcadeModeInfo()
        {
            #region
            string sData = "";

            for (var i = 0; i < GameModeArcade.instance.listArcadeTrackState.Count; i++)
            {
                sData += TrueFalse(GameModeArcade.instance.listArcadeTrackState[i]);
                if (i < GameModeArcade.instance.listArcadeTrackState.Count - 1) sData += "_";
            }

            return sData;
            #endregion
        }

        public bool LoadArcadeModeInfo(string sData)
        {
            #region
            GameModeArcade.instance.Init(sData);
            return true;
            #endregion
        }

        public string SaveTimeTrialModeInfo()
        {
            #region
            string sData = "";

            //-> Save if Time Trial Unlocked
            for (var i = 0; i < GameModeTimeTrial.instance.listTimeTrialTrackState.Count; i++)
            {
                sData += TrueFalse(GameModeTimeTrial.instance.listTimeTrialTrackState[i]);
                if (i < GameModeTimeTrial.instance.listTimeTrialTrackState.Count - 1) sData += "_";
            }

            

            return sData;
            #endregion
        }

        public bool LoadTimeTrialModeInfo(string sData)
        {
            #region
            GameModeTimeTrial.instance.Init(sData);
            return true;
            #endregion
        }

        public string SaveGlobalModeInfo()
        {
            #region
            string sData = "";

            for (var i = 0; i < GameModeGlobal.instance.vehicleIDList.Count; i++)
            {
                sData += GameModeGlobal.instance.vehicleIDList[i];
                if(i< GameModeGlobal.instance.vehicleIDList.Count-1)sData += "_";
            }

            return sData;
            #endregion
        }

        public bool LoadGlobalModeInfo(string sData)
        {
            #region
            GameModeGlobal.instance.Init(sData);
            return true;
            #endregion
        }

        public string SaveCoins()
        {
            #region
            string sData = "";

            //-> Save Coins
            sData = InfoCoins.instance.currentPlayerCoins.ToString();

            return sData;
            #endregion
        }

        public bool LoadCoins(string sData)
        {
            #region
            //-> Update the coins manager
            InfoCoins.instance.InitCoins(sData);
            return true;
            #endregion
        }

      
        string TrueFalse(bool value)
        {
            if (value) return "T";
            else return "F";
        }
    }

}
