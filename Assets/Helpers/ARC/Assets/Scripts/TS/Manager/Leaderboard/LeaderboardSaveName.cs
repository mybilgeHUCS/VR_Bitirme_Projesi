// LeaderboardSaveName: Description : This script allows to save score and name in Time Trial Mode
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Linq;
using UnityEngine.Events;

namespace TS.Generics
{
	public class LeaderboardSaveName : MonoBehaviour
	{
		public static       LeaderboardSaveName instance = null;
		public int          MaxLetter = 5;                          // Max length name
		public string[]     arrChara;                               // Characters that could be used for a name
		public Text[]       txt_Center;                             // Display character on these ui.text

		public CurrentText  txtPlayerName;
		public string       sCurrentName = "";

		public int          OffsetLetter = 4;                       // Use to display A ltter on UI.text name text_Center

		private int         cmpt = 0;                               // use calculate the good letter to display
		public int          StartChara = 0;                         // Use to display A ltter on UI.text name text_Center

		public CurrentText  txt_Leaderboard_Score;                  // Display Leaderboard on screen

		public TTOpening    tOpening;

		private int         step = 0;
        [HideInInspector]
		public int          timeScore;

		public int          maxEntries = 30;
		public GameObject	objBestScore;

		public UnityEvent   savePlayerTimeAndName;



		void Awake()
		{
			if (instance == null)instance = this;
		}

		public bool InitTimeTrialResult()
		{
			step = 0;

			objBestScore.SetActive(false);

			txt_Leaderboard_Score.NewTextManageByScript(new List<TextEntry>() { new TextEntry("00:00:00") });

			for (var i = 0; i < txt_Center.Length; i++)
			{
				cmpt = StartChara;
				if (cmpt < 0) cmpt = arrChara.Length - 1;
				if (txt_Center[i]) txt_Center[i] = txt_Center[i].gameObject.GetComponent<Text>();   // Access component
				if (txt_Center[i]) txt_Center[i].text = arrChara[(cmpt + i) % arrChara.Length];     // init letter to display
			}

			if (txtPlayerName)
			{

				if (PlayerPrefs.HasKey("LastTimeTrialPlayer"))
					sCurrentName = PlayerPrefs.GetString("LastTimeTrialPlayer");
				else
					sCurrentName = "-";

				txtPlayerName.NewTextManageByScript(new List<TextEntry>() { new TextEntry(sCurrentName) });// 
			}

			return true;
		}

		public string FormatTimer(int newTime)
		{
			int FormatedTimer = newTime;
			int minutes = FormatedTimer / (60000);
			int seconds = (FormatedTimer % 60000) / 1000;
			int milliseconds = FormatedTimer % 1000;
			return String.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
		}


		public void Display_Next_Letter()
		{                                           // --> UI Button is pressed.Display Next letter on screen.
			if (step == 0) {
				cmpt++;
				cmpt = cmpt % arrChara.Length;
				Display_Letter();
			}
		}

		public void Display_Last_Letter()
		{                                           // --> UI Button is pressed.Display Last letter on screen.
			if (step == 0) {
				cmpt--;
				if (cmpt < 0) cmpt = arrChara.Length - 1;
				Display_Letter();
			}	
		}

		public void Display_Letter()
		{                                                   // --> Display Letters on screen
			for (var i = 0; i < txt_Center.Length; i++)
			{
				txt_Center[i].text = arrChara[(cmpt + i) % arrChara.Length];
			}
		}

		public void AddLetter()
		{
			if (step == 0)
			{// --> Add letter to the name
				if (sCurrentName.Length < MaxLetter)
				{
					if (sCurrentName == "-") sCurrentName = "";

					if (arrChara[(cmpt + OffsetLetter) % arrChara.Length] == "")
						sCurrentName += " ";
					else
						sCurrentName += arrChara[(cmpt + OffsetLetter) % arrChara.Length];

					txtPlayerName.NewTextManageByScript(new List<TextEntry>() { new TextEntry(sCurrentName) });// 
				}
			}
		}

		public void SupprLetter()
		{                                                   // --> UI Button is pressed. Delete Last Letter
			if (step == 0)
            {
				string tmpString = sCurrentName;
				if (tmpString.Length > 1)                                                           // Check if name islonger than one character
					tmpString = tmpString.Substring(0, tmpString.Length - 1);
				else
					tmpString = "-";

				sCurrentName = tmpString;
				txtPlayerName.NewTextManageByScript(new List<TextEntry>() { new TextEntry(sCurrentName) });// 
			}	
		}

		public void Validate()
		{                                                       // --> UI Button is pressed. Validate name and score.
            if (step == 0)
            {
				Scene scene = SceneManager.GetActiveScene();

                // Save the Player name uses in Time Trial.
				PlayerPrefs.SetString("LastTimeTrialPlayer", sCurrentName);                  // Save the player name for the next time the board appear on screen

                //-> Save the score in the leaderboard
				int currentSelectedTrack = ReturnCurrentTrackID();
				GameModeTimeTrial.leadEntry newEntry = new GameModeTimeTrial.leadEntry(sCurrentName, timeScore);

                if (!IsThisScoreAlreadyExist(newEntry))
                {
					GameModeTimeTrial.instance.listLeaderboardByTrack[currentSelectedTrack].listLeadEntry.Add(newEntry);
					savePlayerTimeAndName?.Invoke();
				}

				//-> Display button Restart Race and Quit race
				tOpening.StepTwo();
			}
        }

		public void SavePlayerTimeAndName()
        {
			StartCoroutine(SavePlayerInfoRoutine());
            
		}


		IEnumerator SavePlayerInfoRoutine()
        {
		    yield return new WaitUntil(() => ReOrderLeaderboard() == true);

			//-> Display CanvasAutoSave
			CanvasAutoSave.instance.transform.GetChild(0).gameObject.SetActive(true);

			//-> Save Player Progression
			yield return new WaitUntil(() => SaveManager.instance.saveAndReturnTrueAFterSaveProcess(
				LoadSavePlayerProgession.instance.SaveInfoPlayer(),
				"PP_" + LoadSavePlayerProgession.instance.currentSelectedSlot));

			//-> Save Leaderboards
			yield return new WaitUntil(() => SaveManager.instance.saveAndReturnTrueAFterSaveProcess(
				GameModeTimeTrial.instance.SaveLeaderboards(),
				"TL_" + LoadSavePlayerProgession.instance.currentSelectedSlot));

			//-> Display CanvasAutoSave
			yield return new WaitForSeconds(1);
			CanvasAutoSave.instance.transform.GetChild(0).gameObject.SetActive(false);
		}


		

        public void DisplayTimeScore()
        {
			InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
			timeScore = (int)(LapCounterAndPosition.instance.posList[0].globalTime * 1000.0f);
			txt_Leaderboard_Score.NewTextManageByScript(new List<TextEntry>() { new TextEntry(FormatTimer(timeScore)) });

			if (ReturnPlayerPositionInTheLeaderboard(timeScore) == 0)
				objBestScore.SetActive(true);
			


			InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
		}



        int ReturnCurrentTrackID()
        {
			int currentSelectedTrack = GameModeTimeTrial.instance.currentSelection;

			TracksData.trackParams trackParams = DataRef.instance.tracksData.listTrackParams[currentSelectedTrack];
			if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
			{
				int specialOrderID = DataRef.instance.timeTrialModeData.customTrackList[currentSelectedTrack];
				return specialOrderID;
			}
			else
			{
				return currentSelectedTrack;
			}
		}


        bool ReOrderLeaderboard()
        {
			//-> Calculate the player's position in the race
			var query = GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry.ToArray().OrderByDescending(id => id.time).Reverse();

			List<GameModeTimeTrial.leadEntry> tmpList = new List<GameModeTimeTrial.leadEntry>();

			foreach (GameModeTimeTrial.leadEntry id in query)
				tmpList.Add(new GameModeTimeTrial.leadEntry(id.name, id.time));

			GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry.Clear();

            for(var i = 0; i < tmpList.Count; i++)
            {
				if (i < maxEntries)
					GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry.Add(tmpList[i]);
				else
					break;
			}

			return true;
		}


		bool IsThisScoreAlreadyExist(GameModeTimeTrial.leadEntry newEntry)
        {
			foreach (GameModeTimeTrial.leadEntry entry in GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry)
				if (entry.name == newEntry.name && entry.time == newEntry.time)
					return true;

			return false;
        }

		int ReturnPlayerPositionInTheLeaderboard(int timeScore)
        {
			for(var i = 0;i < GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry.Count; i++)
            {
				if (GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry[i].time > timeScore)
					return i;
            }


			return GameModeTimeTrial.instance.listLeaderboardByTrack[ReturnCurrentTrackID()].listLeadEntry.Count;


		}
	}
}
