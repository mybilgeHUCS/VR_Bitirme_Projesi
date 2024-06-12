//Description: Leaderboard: Open and CLose the leaderbord page in Main Menu Scene
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace TS.Generics
{
    public class Leaderboard : MonoBehaviour
    {
        //-> Editor variables
        public bool                 SeeInspector;
        public bool                 moreOptions;
        public bool                 helpBox = true;
        public string               newTemplateName;

        //-> Leaderboard variables
        public static Leaderboard   instance = null;

        public int                  pageLeaderboard = 9;            // Use by LeaderboardAssistant when the player want to open a leaderboard
        public GameObject           objContent;
        public bool                 b_IsInitLeaderboardInProcess;

        public GameObject           objTxtleaderboardName;
        private CurrentText         currentText;
        public int currentSelectedLead = 0;

        [System.Serializable]
        public class Grp_Score
        {
            public string name;
            public GameObject scorePrefab;
            public bool b_alternateBackgroundColor;
            public Color alternateBackgroundColor_Odd = Color.gray;     // 0,2,4,...
            public Color alternateBackgroundColor_Even = Color.white;    // 1,3,5,...
            public bool b_alternateBackgroundSprite;
            public Sprite alternateBackgroundSprite_Odd;     // 0,2,4,...
            public Sprite alternateBackgroundSprite_Even;    // 1,3,5,...
            public int maxEntries = 10;
            public UnityEvent setupLeaderboard = new UnityEvent();
        }

        public List<Grp_Score> listGrpScore = new List<Grp_Score>();

        

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }


        //-> Init the leaderboard with the new scores then display the leaderboard menu
        public IEnumerator OpenLeaderboardRoutine(int leaderboardStyle, string[] scores, string leadboardName = "")
        {
            #region
            yield return null;
            #endregion
        }


        //-> Display a new leaderboard
        public void NewLeaderboard(int leaderboardStyle,string[] scores, string leadboardName = "",bool bFormattedTime = true)
        {
            #region
            if (!currentText)
                currentText = objTxtleaderboardName.GetComponent<CurrentText>();

            for (var i = objContent.transform.childCount -1; i >= 0 ; i--)
                Destroy(objContent.transform.GetChild(i).gameObject);


            currentText.NewTextManageByScript(new List<TextEntry>() { new TextEntry(leadboardName) });

            int howManyObjText = 0;
            for (var i = 0; i < listGrpScore[leaderboardStyle].scorePrefab.transform.childCount; i++)
            {
                if (listGrpScore[leaderboardStyle].scorePrefab.transform.GetChild(i).GetComponent<Text>())
                    howManyObjText++;
            }

            if(scores != null)
            {
                int HowManyEntries = scores.Length / (howManyObjText - 1);

                for (var i = 0; i < HowManyEntries; i++)
                {
                    if (i < listGrpScore[leaderboardStyle].maxEntries)
                    {
                        // Create the entry
                        GameObject newScore = Instantiate(listGrpScore[leaderboardStyle].scorePrefab, objContent.transform);

                        // Change background color if needed --> background MUST be the first child
                        if (listGrpScore[leaderboardStyle].b_alternateBackgroundColor && i % 2 == 0)
                            newScore.transform.GetChild(0).GetComponent<Image>().color = listGrpScore[leaderboardStyle].alternateBackgroundColor_Odd;
                        else if (listGrpScore[leaderboardStyle].b_alternateBackgroundColor && i % 2 == 1)
                            newScore.transform.GetChild(0).GetComponent<Image>().color = listGrpScore[leaderboardStyle].alternateBackgroundColor_Even;

                        // Change background sprite if needed
                        if (listGrpScore[leaderboardStyle].b_alternateBackgroundSprite && i % 2 == 0)
                            newScore.transform.GetChild(0).GetComponent<Image>().sprite = listGrpScore[leaderboardStyle].alternateBackgroundSprite_Odd;
                        else if (listGrpScore[leaderboardStyle].b_alternateBackgroundSprite && i % 2 == 1)
                            newScore.transform.GetChild(0).GetComponent<Image>().sprite = listGrpScore[leaderboardStyle].alternateBackgroundSprite_Even;

                        // Position is always the second child
                        newScore.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry((i + 1).ToString()) });

                        for (var j = 0; j < howManyObjText - 1; j++)
                        {
                            int value = i * (howManyObjText - 1) + j;
                            string txtToDisplay = scores[value];

                            //-> Inside the Score Prefab if text object contain "Time" in his name: The value is formated to display a time 00:00:000. 
                            if (bFormattedTime && newScore.transform.GetChild(2 + j).name.Contains("Time"))
                                txtToDisplay = FormatTimer(scores[value]);

                            newScore.transform.GetChild(2 + j).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(txtToDisplay) });
                        }
                    }
                }
            }
           
            b_IsInitLeaderboardInProcess = false;
            #endregion
        }

        public void ExitLeaderboard()
        {
            #region
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            if (Leaderboard.instance)
            {
                GameObject newSelectedButton = CanvasMainMenuManager.instance.ComeBackFromPageList[2].selectedButtonWhenBackToPage;
                //Debug.Log("Exit leaderboard");

                StartCoroutine(CanvasMainMenuManager.instance.listMenu[Leaderboard.instance.pageLeaderboard].transform.parent.GetComponent<PageOut>().DisableCurrentPageAndSelectManualyANewPage(
                    CanvasMainMenuManager.instance.ComeBackFromPageList[2].comeBackToPage,
                    false,
                    .35f,
                    newSelectedButton));
            }
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            #endregion
        }



        //-> Parse a string
        public string[] returnParseString(string sParams)
        {
            #region
            if (sParams != "")
            {
                string[] codes = sParams.Split('|');
                return codes;
            }

            return null;
            #endregion
        }

        string FormatTimer(string newTime)
        {
            int FormatedTimer = int.Parse(newTime);
            int minutes = FormatedTimer / (60000);
            int seconds = (FormatedTimer % 60000) / 1000;
            int milliseconds = FormatedTimer % 1000;
            return String.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
        }
    }

}
