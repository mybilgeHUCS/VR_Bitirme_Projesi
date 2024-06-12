using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace TS.Generics
{
    public class SelectVehicleList : MonoBehaviour
    {
       public void SelectVehicleArcadeTimeTrial()
       {
            //-> Arcade Mode
            if(InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                GameModeGlobal.instance.vehicleIDList.Clear();
                int howManyVehicle = DataRef.instance.arcadeModeData.howManyVehicleByRace;
                int HowManyPlayers = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

                //-> Add Player vehicle Prefab ID to cars list
                for(var i = 0;i< HowManyPlayers; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(InfoVehicle.instance.listSelectedVehicles[i]);
                }

                //-> Add AI vehicle Prefab ID to cars list
                List<int> tmpListVehicleAI = listVehicleAI(howManyVehicle);
                for (var i = 0; i < tmpListVehicleAI.Count; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(tmpListVehicleAI[i]);
                }
            }
            //-> Time Trial
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                GameModeGlobal.instance.vehicleIDList.Clear();
                int HowManyPlayers = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
                //-> Add Player vehicle Prefab ID to cars list
                for (var i = 0; i < HowManyPlayers; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(InfoVehicle.instance.listSelectedVehicles[i]);
                }
            }
       }

        public void SelectVehicleChampionship()
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 2)
            {
                GameModeGlobal.instance.vehicleIDList.Clear();
                int howManyVehicle = ReturnMaxChampionshipPlayer();
                int HowManyPlayers = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

                //-> Add Player vehicle Prefab ID to cars list
                for (var i = 0; i < HowManyPlayers; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(InfoVehicle.instance.listSelectedVehicles[i]);
                }

                //-> Add AI vehicle Prefab ID to cars list
                List<int> tmpListVehicleAI = listVehicleAI(howManyVehicle);
                for (var i = 0; i < tmpListVehicleAI.Count; i++)
                {
                    GameModeGlobal.instance.vehicleIDList.Add(tmpListVehicleAI[i]);
                }
            }
       }


        List<int> listVehicleAI(int _howManyVehicle)
        {
            List<int> tmpListVehicleAI = new List<int>();

            int howManyVehicleAvailable = DataRef.instance.vehicleGlobalData.carParametersList.Count;
            int howManyVehicleInTheRace = _howManyVehicle;
            int HowManyPlayers = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

            //-> Create the AI car list
            for (var i = 0; i < howManyVehicleAvailable; i++)
                tmpListVehicleAI.Add(i);

            //-> Remove from that list player cars
            for (var i = 0; i < HowManyPlayers; i++)
            {
                for (var j = 0; j < tmpListVehicleAI.Count; j++)
                {
                    if(tmpListVehicleAI[j] == InfoVehicle.instance.listSelectedVehicles[i])
                    {
                        tmpListVehicleAI.RemoveAt(j);
                        break;
                    }
                }
            }

            /*string s = "";
            foreach (int value in tmpListVehicleAI)
                s += value + "|";
            Debug.Log("tmpListVehicleAI: " + s);
            */

            //-> Randomize the list
            List<int> listRandomized = new List<int>();
            int listSize = tmpListVehicleAI.Count;

            while(listRandomized.Count < howManyVehicleInTheRace - HowManyPlayers)
            {
                List<int> tmpListVehicleAICopy = new List<int>(tmpListVehicleAI);
                for (var i = 0; i < listSize; i++)
                {
                    int rand = UnityEngine.Random.Range(0, tmpListVehicleAICopy.Count);

                    listRandomized.Add(tmpListVehicleAICopy[rand]);
                    tmpListVehicleAICopy.RemoveAt(rand);

                    if (listRandomized.Count == howManyVehicleInTheRace - HowManyPlayers)
                        break;
                }
            }
            
            /*s = "";
            foreach (int value in listRandomized)
                s += value + "|";

            Debug.Log("listRandomized: " + s);
            */
            return listRandomized;
        }

        int ReturnMaxChampionshipPlayer()
        {
            //-> Find max vehicles use in a race during the championship
            int currentChampionship = GameModeChampionship.instance.currentSelection;

            ChampionshipModeData championshipModeData = DataRef.instance.championshipModeData;
            int maxPlayer = 0;

            for(var i = 0;i< championshipModeData.listOfChampionship[currentChampionship].listTrackParams.Count; i++)
            {
                if (championshipModeData.listOfChampionship[currentChampionship].listTrackParams[i].howManyVehicleByRace > maxPlayer)
                    maxPlayer = championshipModeData.listOfChampionship[currentChampionship].listTrackParams[i].howManyVehicleByRace;
            }

            return maxPlayer;
        }

        public void ClearChampionshipPoints()
        {
            GameModeChampionship.instance.listScore.Clear();
        }
    }
}