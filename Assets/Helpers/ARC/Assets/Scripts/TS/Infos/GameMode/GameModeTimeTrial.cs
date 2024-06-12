// Description: GameModeTimeTrial: Access from anywhere info about GameModeTimeTrial.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class GameModeTimeTrial : MonoBehaviour
    {
        public static GameModeTimeTrial instance = null;
        public int                      currentSelection = 0;
        public List<bool>               listTimeTrialTrackState = new List<bool>(); // Remember if a track in Time Trial Mode is unlocked.

        [System.Serializable]
        public class leadEntry
        {
            public string name;
            public int time;

            public leadEntry(string _Name,int _Time)
            {
                name = _Name;
                time = _Time;
            }
        }

        [System.Serializable]
        public class leaderboardByTrack
        {
            public List<leadEntry> listLeadEntry = new List<leadEntry>();
        }

        public List<leaderboardByTrack> listLeaderboardByTrack = new List<leaderboardByTrack>();

        void Awake()
        {
            #region Create ony one instance of the gameObject in the Hierarchy
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
            #endregion
        }

        public void Init(string sData)
        {
            //Debug.Log("ini: ''" + sData + "''");
            if (sData == "")
            {
                //-> Update Time Trial track unlock State
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                {
                    listTimeTrialTrackState.Add(DataRef.instance.tracksData.listTrackParams[i].isUnlockedTimeTrial);
                }

                //-> Update Time Trial track unlock State
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                {
                    listLeaderboardByTrack.Add(new leaderboardByTrack());
                   
                }

              
            }
            else
            {
                string[] codes = sData.Split('_');
                int counter = 0;
                //-> Update Arcade State
                for (var i = 0; i < codes.Length; i++)
                {
                    listTimeTrialTrackState.Add(TrueFalse(codes[i]));
                    counter++;
                }
            }
        }

        public void InitLeaderboards(string sData)
        {
            //Debug.Log("ini: ''" + sData + "''");
            if (sData == "")
            {
                //-> Create new list of leaderboard info
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                    listLeaderboardByTrack.Add(new leaderboardByTrack());

                //-> Init Leaderboards
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                {
                    for (var j = 0; j < DataRef.instance.tracksData.listTrackParams[i].listLeadeboardEntries.Count; j++)
                    {
                         listLeaderboardByTrack[i].listLeadEntry.Add(
                         new leadEntry(DataRef.instance.tracksData.listTrackParams[i].listLeadeboardEntries[j].name,
                         DataRef.instance.tracksData.listTrackParams[i].listLeadeboardEntries[j].time));
                    }


                }
            }
            else
            {
                string[]    codes   = sData.Split('_');
                int         counter = 0;

                //-> Create new list of leaderboard info
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                    listLeaderboardByTrack.Add(new leaderboardByTrack());

                //-> Update each leaderboard with Saved data
                for (var i = 0; i < DataRef.instance.tracksData.listTrackParams.Count; i++)
                {
                    int howManyEntries = int.Parse(codes[counter]);
                    counter++;

                    for (var j = 0; j < howManyEntries; j++)
                    {
                        string  _Name = codes[counter];
                        int     _Time = int.Parse(codes[counter + 1]);

                        listLeaderboardByTrack[i].listLeadEntry.Add(new leadEntry(_Name, _Time));
                        counter += 2;
                    }
                }
            }
        }

        bool TrueFalse(string value)
        {
            if (value == "T") return true;
            else return false;
        }


        public string SaveLeaderboards()
        {
            #region
            string sData = "";

            //-> Save Leaderboards
            for (var j = 0; j < listLeaderboardByTrack.Count; j++)
            {

                sData += instance.listLeaderboardByTrack[j].listLeadEntry.Count;


                if (j < listLeaderboardByTrack.Count - 1
                    ||
                  j == listLeaderboardByTrack.Count - 1 &&
                 listLeaderboardByTrack[j].listLeadEntry.Count > 0)
                    sData += "_";

                //-> Save leaderboard for each Time Trial track
                for (var i = 0; i < listLeaderboardByTrack[j].listLeadEntry.Count; i++)
                {
                    sData += listLeaderboardByTrack[j].listLeadEntry[i].name;
                    sData += "_";
                    sData += listLeaderboardByTrack[j].listLeadEntry[i].time;

                    sData += "_";
                    //Debug.Log("sData -> " + i);
                }


            }

            if (sData[sData.Length - 1] == '_')
                sData = sData.Substring(0, sData.Length - 1);

            //Debug.Log("sData -> " + sData);

            return sData;
            #endregion
        }
    }
}

