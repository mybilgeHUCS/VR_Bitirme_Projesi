using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    [CreateAssetMenu(fileName = "ChampionshipModeData", menuName = "TS/ChampionshipModeData")]
    public class ChampionshipModeData : ScriptableObject
    {
        public bool MoreOptions;
        public bool HelpBox;
        public int  tab;

        //-> Global Parameters
        public List<int> pointsWinDependingFinalPosition = new List<int>();


        //-> Championship parameters
        [System.Serializable]
        public class TrackParams
        {
            public int TrackID = 0;

            public int AI_Difficulty = 0;
            public int howManyVehicleByRace = 8;

            public bool UnlockTrackOnArcadeMode = true;
            public bool UnlockTrackOnTimeTrialMode = true;

            public List<EditorMethodsList_Pc.MethodsList> methodsCallWhenRaceIsFinished
           = new List<EditorMethodsList_Pc.MethodsList>();                                         // useful to unlock somthing when the championship ended


            public bool bShowTrackInfoInEditor = true;
            public int winCoinsTrack = 1000;
        }

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

        [System.Serializable]
        public class _Championship
        {
            public List<Texts> listTexts = new List<Texts>(3) { new Texts("Championship Name",0, 0), new Texts("Unlock Text",0, 0), new Texts("Comment", 0, 0) };
            public bool showInCustomEditor = true;

            public List<TrackParams> listTrackParams = new List<TrackParams>();
            
            public Sprite championshipIcon;
            public bool Unlock = true;

            public bool bUnlockANewChampionship = true;
            public int whichChampionshipToUnlock = 0;
            public int whichPlayerPosToUnlock = 0;

            public List<EditorMethodsList_Pc.MethodsList> methodsCallWhenChampionshipIsFinished      
            = new List<EditorMethodsList_Pc.MethodsList>();                                         // useful to unlock somthing when the championship ended

            public bool showCoinsInEditor = true;
            public bool winCoinsChampionship = true;
            public List<int> listChampionshipCoins = new List<int>();
        }
       
        [Header ("Championship List")]
        public List<_Championship> listOfChampionship = new List<_Championship>();

        public bool bDisplayChampionshipUsingListOrder = true;
        public List<int> customChampionshipList = new List<int>();

        public List<int> listEndChampionshipCoins = new List<int>(8) { 20000, 17500, 15000,11000,9000,7000,5000,2000 };

    }
}

