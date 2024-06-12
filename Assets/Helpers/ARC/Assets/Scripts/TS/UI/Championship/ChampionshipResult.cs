// Description: ChampionshipResult: Display the result after a race in Championship mode
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

namespace TS.Generics
{
    public class ChampionshipResult : MonoBehaviour
    {
        public static ChampionshipResult instance = null;
        public Transform            grpContent;
        public GameObject           grpScorePrefab;
        
        public Color                oddColor = Color.white;
        public Color                evenColor = Color.gray;
        public Sprite oddSprite;
        public Sprite evenSprite;

        public List<int>            vehicleList = new List<int>();
        private int                 vehicleUICounter = 0;

        bool                        bIsScoreProcess = false;

        public CurrentText          trackName;

        public List<GameObject>     btnList = new List<GameObject>();

        private int                 ChampionshipResultPart = 1;
        List<GameObject>            childrenList = new List<GameObject>();

        public GameObject objDisableTime;

        [System.Serializable]
        public class VPos
        {
            public int ID = 0;
            public int Position = 0;
            public int score = 0;

            public VPos(int _ID, int _Position)
            {
                ID = _ID;
                Position = _Position;
            }
        }

        public List<VPos>           vPosList = new List<VPos>();
        public List<VPos>           finalTrackPosList = new List<VPos>();

        [System.Serializable]
        public class VScore
        {
            public int ID = 0;
            public int score = 0;

            public VScore(int _ID, int _score)
            {
                ID = _ID;
                score = _score;
            }
        }

        public List<VScore>         vScoreList = new List<VScore>();

        public RewardAssistant      rewardAssistantTrack;
        public RewardAssistant      rewardAssistantEndChamp;

        public bool                 isChampionshipFinished;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }


        void Start()
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 2)
                LapCounterAndPosition.instance.AVechicleFinishTheRace += UpdateVehicleList;
        }

        private void OnDestroy()
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 2)
                LapCounterAndPosition.instance.AVechicleFinishTheRace -= UpdateVehicleList;
        }


        public void UpdateVehicleList(int playerID)
        {
            if(ChampionshipResultPart == 1)
            {
                vehicleList.Add(playerID);
                if (!bIsScoreProcess)
                    StartCoroutine(DisplayScoreRoutine());
            }
        }


        IEnumerator DisplayScoreRoutine()
        {
            #region
            bIsScoreProcess = true;

            while(vehicleUICounter < vehicleList.Count)
            {
                if (ChampionshipResultPart == 2)
                    break;

                // Race Results
                if (vehicleUICounter == 0)
                    trackName.NewTextWithSpecificID(155, 0);

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
                 if(vehicleList[vehicleUICounter - 1] < GameModeGlobal.instance.vehicleNames.Count)
                     refID = GameModeGlobal.instance.vehicleNames[vehicleList[vehicleUICounter - 1]];
                 int listID = 0;
                 int entryID = 149;

                 //-> Player 1 or 2 case
                 if (refID < 0)
                 {
                     listID = DataRef.instance.vehicleGlobalData.playerNamesList[Mathf.Abs(refID) - 1 ].listID;
                     entryID = DataRef.instance.vehicleGlobalData.playerNamesList[Mathf.Abs(refID) - 1].EntryID;
                 }
                 //-> AIs case
                 else if(refID < DataRef.instance.vehicleGlobalData.aiNamesList.Count)
                 {
                     listID = DataRef.instance.vehicleGlobalData.aiNamesList[refID].listID;
                     entryID = DataRef.instance.vehicleGlobalData.aiNamesList[refID].EntryID;
                 }


                 scorePrefab.transform.GetChild(2).GetComponent<CurrentText>().NewTextWithSpecificID(entryID, listID);
                 
                //-> Time
                 LapCounterAndPosition lapCounter = LapCounterAndPosition.instance;
                 int vScore = (int)(lapCounter.posList[vehicleList[vehicleUICounter - 1]].globalTime * 1000.0f);

                scorePrefab.transform.GetChild(3).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(FormatTimer(vScore)) });

                //-> Track Points
                int trackPoints = 0;
                if(vehicleUICounter-1 < DataRef.instance.championshipModeData.pointsWinDependingFinalPosition.Count)
                    trackPoints = DataRef.instance.championshipModeData.pointsWinDependingFinalPosition[vehicleUICounter-1];

                if(trackPoints > 0)
                    scorePrefab.transform.GetChild(4).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry("+" + trackPoints.ToString()) });
                else
                    scorePrefab.transform.GetChild(4).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry("") });

                yield return new WaitForSeconds(.25f);

                yield return null;
            }

            bIsScoreProcess = false;
            yield return null;
            #endregion
        }

        string FormatTimer(int newTime)
        {
            int FormatedTimer = newTime;
            int minutes = FormatedTimer / (60000);
            int seconds = (FormatedTimer % 60000) / 1000;
            int milliseconds = FormatedTimer % 1000;
            return String.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }

        public bool BInitChampionshipResultMenu()
        {
            //
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


        public void ChampionshipResultPart2()
        {
            ChampionshipResultPart = 2;

            //TS_EventSystem.instance.eventSystem.SetSelectedGameObject(null);

          int currentChamp = GameModeChampionship.instance.currentSelection;

            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
                currentChamp = DataRef.instance.championshipModeData.customChampionshipList[currentChamp];

            int trackID = DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTrackParams[GameModeChampionship.instance.currentTrackInTheList].TrackID;

            GameModeGlobal.instance.currentSelectedTrack = DataRef.instance.tracksData.listTrackParams[trackID].sceneName;

            if (GameModeChampionship.instance.currentTrackInTheList + 1  < DataRef.instance.championshipModeData.listOfChampionship[currentChamp].listTrackParams.Count)
            {
                btnList[3].SetActive(true);
                isChampionshipFinished = false;
                GameModeChampionship.instance.currentTrackInTheList++;
            }
            else
            {
                btnList[2].SetActive(true);
                isChampionshipFinished = true;
                GameModeChampionship.instance.currentTrackInTheList = 0;
            }

           
            btnList[1].SetActive(false);

            ChampDisplayGlobalPoints();
        }

        public void OpenQuitIGPage()
        {
            Quit_IGAssistant.instance.OpenQuitIGPage();
        }

       
        public void ChampDisplayGlobalPoints()
        {
            //-> Clear the container
            for(var i = 0;i< grpContent.childCount; i++)
                childrenList.Add(grpContent.GetChild(i).gameObject);

            for (var i = 0; i < childrenList.Count; i++)
                Destroy(childrenList[i]);

            //-> Freeze position
            for (var i = 0; i < LapCounterAndPosition.instance.posList.Count; i++)
            {
                bool bAdd = true;
                for (var j = 0; j < vehicleList.Count; j++)
                {
                    if (vehicleList[j] == i) {
                        bAdd = false;
                        break;
                    }   
                }

                if(bAdd)
                    vPosList.Add(new VPos(i, LapCounterAndPosition.instance.posList[i].RacePos));
            }
               

            //-> Calculate the player's position in the race
            var query = vPosList.ToArray().OrderBy(id => id.Position);

            foreach (VPos id in query)
                finalTrackPosList.Add(new VPos(id.ID, id.Position));

          
            //-> Add track points to each player
            if(GameModeChampionship.instance.listScore.Count == 0)
            {
                for (var i = 0; i < GameModeGlobal.instance.vehicleIDList.Count;i++)
                    GameModeChampionship.instance.listScore.Add(0);
            }

            //-> Update score for each player
            int counter = 0;
            for (var i = 0; i < vehicleList.Count; i++)
            {
                int trackPoints = 0;
                if (vehicleList[i] < DataRef.instance.championshipModeData.pointsWinDependingFinalPosition.Count)
                    trackPoints = DataRef.instance.championshipModeData.pointsWinDependingFinalPosition[i];

                GameModeChampionship.instance.listScore[vehicleList[i]] += trackPoints;
                counter++;
            }

            for (var i = 0; i < finalTrackPosList.Count; i++)
            {
                int trackPoints = 0;
                if (finalTrackPosList[i].ID < DataRef.instance.championshipModeData.pointsWinDependingFinalPosition.Count)
                    trackPoints = DataRef.instance.championshipModeData.pointsWinDependingFinalPosition[i+ counter];

                GameModeChampionship.instance.listScore[finalTrackPosList[i].ID] += trackPoints;
            }


            //-> Create a new result list (compare)
            //-> Calculate the player's position in the race
            for (var i = 0; i < GameModeChampionship.instance.listScore.Count; i++)
                vScoreList.Add(new VScore(i, GameModeChampionship.instance.listScore[i]));

            var query2 = vScoreList.ToArray().OrderByDescending(id => id.score);

            vScoreList.Clear();
            foreach (VScore id in query2)
                vScoreList.Add(new VScore(id.ID, id.score));


            //-> Display on screen each player ranking + Points
            StartCoroutine(DisplayScorePart2Routine());
            
        }

        //-> Display Total Score Screen
        IEnumerator DisplayScorePart2Routine()
        {
            #region
            if (objDisableTime) objDisableTime.SetActive(false);

            // Championship Results
            trackName.NewTextWithSpecificID(156, 0);
            bIsScoreProcess = true;
            vehicleUICounter = 0;

            yield return new WaitUntil(() => rewardAssistantTrack.ClearRewardSection());
            
            for (var i = 0;i< vScoreList.Count; i++)
            {
                GameObject scorePrefab = Instantiate(grpScorePrefab, grpContent);
                scorePrefab.name += "_" + vScoreList[i].ID;

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
                if (vScoreList[i].ID < GameModeGlobal.instance.vehicleNames.Count)
                    refID = GameModeGlobal.instance.vehicleNames[vScoreList[i].ID];
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

                //-> Track Points
                int trackPoints = vScoreList[i].score;

                scorePrefab.transform.GetChild(4).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(trackPoints.ToString()) });
                
                yield return new WaitForSeconds(.15f);

                yield return null;
            }

            rewardAssistantEndChamp.RewardSequence("ChampionshipEndChampionshipReward");
            yield return new WaitUntil(() => rewardAssistantTrack.bIsRewardDone == true);

            //-> Next Track
            if (btnList[3].activeSelf)
            {
                TS_EventSystem.instance.eventSystem.SetSelectedGameObject(btnList[3].transform.GetChild(1).gameObject);
            }
            //-> End of the Championship
            else if (btnList[2].activeSelf)
            {
                TS_EventSystem.instance.eventSystem.SetSelectedGameObject(btnList[2].transform.GetChild(1).gameObject);
            }

            bIsScoreProcess = false;
            yield return null;
            #endregion
        }


        public void PlayAgainThisChampionship()
        {
            isChampionshipFinished = true;
            GameModeChampionship.instance.listScore.Clear();
            GameModeChampionship.instance.currentTrackInTheList = 0;

            int currentChampionship = GameModeChampionship.instance.currentSelection;
            ChampionshipModeData championshipModeData = DataRef.instance.championshipModeData;

            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                int specialOrderID = championshipModeData.customChampionshipList[currentChampionship];
                currentChampionship = specialOrderID;
            }

            int trackIDToLoad = DataRef.instance.championshipModeData.listOfChampionship[currentChampionship].listTrackParams[0].TrackID;

            GameModeGlobal.instance.currentSelectedTrack = DataRef.instance.tracksData.listTrackParams[trackIDToLoad].sceneName;

            string trackName = GameModeGlobal.instance.currentSelectedTrack;
            LoadScene.instance.LoadSceneWithSceneNameAndSpecificCustomMethodList(trackName);
        }
    }
}
