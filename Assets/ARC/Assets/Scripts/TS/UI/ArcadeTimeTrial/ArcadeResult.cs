// Description: ArcadeResult: Used to display Acarde result when a race is finished.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace TS.Generics
{
    public class ArcadeResult : MonoBehaviour
    {
        public static ArcadeResult  instance = null;
        public Transform            grpContent;
        public GameObject           grpScorePrefab;
        
        public Color                oddColor = Color.white;
        public Color                evenColor = Color.white;
        public Sprite               oddSprite;
        public Sprite               evenSprite;

        public List<int>            vehicleList = new List<int>();
        private int                 vehicleUICounter = 0;

        bool                        bIsScoreProcess = false;

        public CurrentText          trackName;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }


        void Start()
        {
            if( InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
                LapCounterAndPosition.instance.AVechicleFinishTheRace += UpdateVehicleList;
        }

        private void OnDestroy()
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
                LapCounterAndPosition.instance.AVechicleFinishTheRace -= UpdateVehicleList;
        }


        public void UpdateVehicleList(int playerID)
        {
            vehicleList.Add(playerID);
            if (!bIsScoreProcess)
                StartCoroutine(DisplayScoreRoutine());
        }


        IEnumerator DisplayScoreRoutine()
        {
            bIsScoreProcess = true;

            while(vehicleUICounter < vehicleList.Count)
            {
                GameObject scorePrefab = Instantiate(grpScorePrefab, grpContent);
                scorePrefab.name += "_" + vehicleUICounter + "->" + vehicleList[vehicleUICounter];

                //-> Background color
                if (vehicleUICounter % 2 == 0)
                {
                    scorePrefab.transform.GetChild(0).GetComponent<Image>().color = oddColor;
                    if (oddSprite) scorePrefab.transform.GetChild(0).GetComponent<Image>().sprite = oddSprite;
                }

                else
                {
                    scorePrefab.transform.GetChild(0).GetComponent<Image>().color = evenColor;
                    if (evenSprite) scorePrefab.transform.GetChild(0).GetComponent<Image>().sprite = evenSprite;
                }

                vehicleUICounter++;

                //-> Position (TextLeaderboardEntry_01)
                scorePrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(vehicleUICounter.ToString()) });

                //-> Name
                int refID = 1000;
                if (vehicleList[vehicleUICounter - 1] < GameModeGlobal.instance.vehicleNames.Count)
                    refID = GameModeGlobal.instance.vehicleNames[vehicleList[vehicleUICounter - 1]];
                int listID = 0;
                int entryID = 149;

                //-> Player 1 or 2 case
                if (refID < 0)
                {
                    listID = DataRef.instance.vehicleGlobalData.playerNamesList[Mathf.Abs(refID) - 1].listID;
                    entryID = DataRef.instance.vehicleGlobalData.playerNamesList[Mathf.Abs(refID) - 1].EntryID;
                }
                //-> AIs case
                else if (refID < DataRef.instance.vehicleGlobalData.aiNamesList.Count)
                {
                    listID = DataRef.instance.vehicleGlobalData.aiNamesList[refID].listID;
                    entryID = DataRef.instance.vehicleGlobalData.aiNamesList[refID].EntryID;
                }


                scorePrefab.transform.GetChild(2).GetComponent<CurrentText>().NewTextWithSpecificID(entryID, listID);

                //-> Time
                LapCounterAndPosition lapCounter = LapCounterAndPosition.instance;
                int vScore = (int)(lapCounter.posList[vehicleList[vehicleUICounter - 1]].globalTime * 1000.0f);

                scorePrefab.transform.GetChild(3).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(FormatTimer(vScore)) });

                yield return new WaitForSeconds(.25f);

                yield return null;
            }

            bIsScoreProcess = false;
            yield return null;
        }

        string FormatTimer(int newTime)
        {
            int FormatedTimer = newTime;
            int minutes = FormatedTimer / (60000);
            int seconds = (FormatedTimer % 60000) / 1000;
            int milliseconds = FormatedTimer % 1000;
            return String.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }

        public bool BInitArcadeResultMenu()
        {
            int currentSelectedTrack = GameModeArcade.instance.currentSelection;

            TracksData.trackParams trackParams = DataRef.instance.tracksData.listTrackParams[currentSelectedTrack];
            if (!DataRef.instance.arcadeModeData.bDisplayTrackUsingTrackListOrder)
            {
                int specialOrderID = DataRef.instance.arcadeModeData.customTrackList[currentSelectedTrack];
                trackParams = DataRef.instance.tracksData.listTrackParams[specialOrderID];
            }

            //-> Find the name of the leaderboard
            int dataFolder = trackParams.selectedListMultiLanguage;
            int textID = trackParams.NameIDMultiLanguage;
            string leaderboardName = LanguageManager.instance.String_ReturnText(dataFolder, textID);

            if(trackName)trackName.NewTextManageByScript(new List<TextEntry>() { new TextEntry(leaderboardName) });

            return true;
        }

    }
}
