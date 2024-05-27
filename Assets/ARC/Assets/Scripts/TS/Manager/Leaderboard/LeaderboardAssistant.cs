// Description: LeaderboardAssistant. ATtached to button that open/close the leaderboard page
using UnityEngine;


namespace TS.Generics
{
    public class LeaderboardAssistant : MonoBehaviour
    {
        // Use by the buttons Button_Leaderboard
        public void OpenLeaderboardPage()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                CanvasMainMenuManager.instance.ComeBackFromPageList[2].selectedButtonWhenBackToPage = TS_EventSystem.instance.eventSystem.currentSelectedGameObject;
                CanvasMainMenuManager.instance.ComeBackFromPageList[2].comeBackToPage = CanvasMainMenuManager.instance.currentSelectedPage;
                SetupNewTheLeaderboard(0);
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[Leaderboard.instance.pageLeaderboard].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(Leaderboard.instance.pageLeaderboard);
            }
            #endregion
        }

        public void SetupNewTheLeaderboard(int direction)
        {
            SetupNewTheLeaderboardPart2(direction);
        }

        public void OpenCurrentTimeTrialLeaderboard()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                CanvasMainMenuManager.instance.ComeBackFromPageList[2].selectedButtonWhenBackToPage = TS_EventSystem.instance.eventSystem.currentSelectedGameObject;
                CanvasMainMenuManager.instance.ComeBackFromPageList[2].comeBackToPage = CanvasMainMenuManager.instance.currentSelectedPage;
                SetupNewTheLeaderboardPart2(0, GameModeTimeTrial.instance.currentSelection);
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[Leaderboard.instance.pageLeaderboard].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(Leaderboard.instance.pageLeaderboard);
            }
            #endregion
        }

        public void SetupNewTheLeaderboardPart2(int direction,int specificID = -1)
        {
            int HowManyTrack;

            if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
                HowManyTrack = DataRef.instance.timeTrialModeData.customTrackList.Count;
            else
                HowManyTrack = DataRef.instance.tracksData.listTrackParams.Count;
            

            if (specificID != -1)
                Leaderboard.instance.currentSelectedLead = specificID;
            else
                Leaderboard.instance.currentSelectedLead += direction + HowManyTrack;

            Leaderboard.instance.currentSelectedLead = Leaderboard.instance.currentSelectedLead % HowManyTrack;
            //Debug.Log("selectedTrack 1: " + currentSelectedTrack);
            Leaderboard.instance.listGrpScore[0].setupLeaderboard.Invoke();
        }

        public void DisplayTimeTrialLeaderboard() 
        {
            int selectedTrack = Leaderboard.instance.currentSelectedLead;

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
            string rawScores = "";

            //-> Create the list of best score to displayed.
            if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
                selectedTrack = DataRef.instance.timeTrialModeData.customTrackList[selectedTrack];

            for (var i = 0; i < GameModeTimeTrial.instance.listLeaderboardByTrack[selectedTrack].listLeadEntry.Count; i++)
            {
                rawScores += GameModeTimeTrial.instance.listLeaderboardByTrack[selectedTrack].listLeadEntry[i].name;
                rawScores += "|";
                rawScores += GameModeTimeTrial.instance.listLeaderboardByTrack[selectedTrack].listLeadEntry[i].time;
                if (i < GameModeTimeTrial.instance.listLeaderboardByTrack[selectedTrack].listLeadEntry.Count - 1) rawScores += "|";
            }

            Leaderboard.instance.NewLeaderboard(0, Leaderboard.instance.returnParseString(rawScores), leaderboardName);
        }
    }

}
