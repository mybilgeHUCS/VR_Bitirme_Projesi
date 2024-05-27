using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    [CreateAssetMenu(fileName = "TracksData", menuName = "TS/TracksData")]
    public class TracksData : ScriptableObject
    {
        public bool MoreOptions;
        public bool HelpBox;
        public int tab;
        [HideInInspector]
        public int minutes = 0;
        [HideInInspector]
        public int seconds = 0;
        [HideInInspector]
        public int milliseconds = 0;

        [System.Serializable]
        public class leaderboardEntry
        {
            public string   name;
            public int      time;


            public int minutes = 0;
            public int seconds = 0;
            public int milliseconds = 0;
        }

        [System.Serializable]
        public class trackParams
        {
            public int                      selectedListMultiLanguage = 0;     // The multilanguage list use for this track of the track
            public int                      NameIDMultiLanguage = 0;           // The multilanguage ID use for this track of the track
            public string                   sceneName;                         // The scene use for this track

            public bool                     isUnlockedArcade = true;
            public bool                     isUnlockedTimeTrial = true;

            public int                      aIDifficulty = 0;

            public Sprite                   trackSprite;
            public Sprite                   fullScreenSprite;
            public List<leaderboardEntry>   listLeadeboardEntries = new List<leaderboardEntry>();

            public bool                     showInEditor = true;
            public bool                     showLeaderboardInEditor = true;

            public bool                     showCoinsInEditor = true;
            public List<int>                listArcadeCoins = new List<int>();
            public List<int>                listTimeTrialCoins = new List<int>();

            public List<Texts>              listTexts = new List<Texts>(2) { new Texts("Unlock Text", 0, 112), new Texts("Comment", 0, 2) };

        }

        public List<trackParams>            listTrackParams = new List<trackParams>();

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

        public bool                         showGlobalParamsInEditor = true;
        public List<float>                  listCoinMultiplier = new List<float>();

        //-> Use to display Timer convertissor in the editor
        public int                          whichTrackSelected;
        public int                          whichScoreSelected;


        public List<int> listArcadeCoins = new List<int>(8) {1500,1000,750,500,250,125,75,50};
        public List<int> listTimeTrialCoins = new List<int>(3) { 1500, 1000, 750};

    }
}

