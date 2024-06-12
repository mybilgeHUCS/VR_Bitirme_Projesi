// Description: LeaderboardSaveCustom: Script use as an example in the Documentation Part Section Leaderboard
// How to: Load custom data in the leaderboard
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class LeaderboardDisplayCustom : MonoBehaviour
    {

        public void DisplayCustomLeaderboard()
        {

            //-> Choose the track to display
            int selectedTrack = Leaderboard.instance.currentSelectedLead;


            //-> Find the parameters of the track
            TracksData.trackParams trackParams = DataRef.instance.tracksData.listTrackParams[selectedTrack];
            if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
            {
                int specialOrderID = DataRef.instance.timeTrialModeData.customTrackList[selectedTrack];
                trackParams = DataRef.instance.tracksData.listTrackParams[specialOrderID];
            }


            //-> Find the name of the leaderboard
            int dataFolder = trackParams.selectedListMultiLanguage;
            int textID = trackParams.NameIDMultiLanguage;
            string leaderboardName = LanguageManager.instance.String_ReturnText(dataFolder, textID);


            //-> Update selected track if bDisplayTrackUsingTrackListOrder = false 
            if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
                selectedTrack = DataRef.instance.timeTrialModeData.customTrackList[selectedTrack];


            //-> Create the list of best score to displayed.
            //-> As an example you can load an online leaderboard and save te value in a List.
            List<string> scoreList = new List<string>() {
                "Player 1","01:01:354",
                "Player 2","01:11:700",
                "Player 3","01:21:315",
                "Player 4","02:04:956",
            }; 

           
            //-> Display the leaderboard
            Leaderboard.instance.NewLeaderboard(0, scoreList.ToArray(), leaderboardName,false);
        }
    }
}

