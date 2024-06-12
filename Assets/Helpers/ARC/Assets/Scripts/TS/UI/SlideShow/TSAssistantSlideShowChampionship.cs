// Description: TSAssistantSlideShowChampionship: Methods to manage slideshow behavior in Championship mode
using UnityEngine;


namespace TS.Generics
{
    public class TSAssistantSlideShowChampionship : MonoBehaviour
    {
        public SlideShow slideShow;

        public bool Init()
        {
            Debug.Log("Init TS assistant");
            return true;
        }

        public bool NewEntry()
        {
            Debug.Log("New entry TS assistant");
            return true;
        }

        public bool GetSprite()
        {
            GetSpriteInit();
            return true;
        }

        public bool GetName()
        {
            GetNameInit();
            return true;
        }

        public int GetHowManyEntries()
        {
            //-> Display Championship using the data order
            if (DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                return DataRef.instance.championshipModeData.listOfChampionship.Count;
            }
            //-> Display Championship using a specific order
            else
            {
                return DataRef.instance.championshipModeData.customChampionshipList.Count;
            }
        }

        public int GetCurrentSelection()
        {
            return GameModeChampionship.instance.currentSelection;
        }

        public void SetCurrentSelection()
        {
            GameModeChampionship.instance.currentSelection = slideShow.currentSelection;
        }

        

        int ReturnDiff(int j)
        {
            int diff = 0;
            if (j < slideShow.whichSelectedInList)
            {
                diff = Mathf.Abs(slideShow.whichSelectedInList - j);
                diff = slideShow.currentSelection - diff;
            }
            else if (j > slideShow.whichSelectedInList)
            {
                diff = Mathf.Abs(slideShow.whichSelectedInList - j);
                diff = slideShow.currentSelection + diff;
            }
            else
            {
                diff = slideShow.currentSelection;
            }

            return diff;
        }

        ChampionshipModeData._Championship ReturnChampionshipGlobalParams(int diff)
        {
            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                int specialOrderID = DataRef.instance.championshipModeData.customChampionshipList[diff];
                return DataRef.instance.championshipModeData.listOfChampionship[specialOrderID];
            }
            else
            {
                return DataRef.instance.championshipModeData.listOfChampionship[diff];
            }
        }

        public void GetSpriteInit()
        {
            for (var j = 0; j < slideShow.listSquare.Count; j++)
            {
                for (var k = 0; k < slideShow.listSquareRef.Count; k++)
                {
                    if (slideShow.listSquare[j] == slideShow.listSquareRef[k])
                    {
                        int diff = ReturnDiff(j);

                        if (diff >= 0 && diff < slideShow.howManyEntries)
                        {
                            ChampionshipModeData._Championship championshipData = ReturnChampionshipGlobalParams(diff);
                            slideShow.objsInSquareList[k].imagesList[0].sprite = championshipData.championshipIcon;
                        }
                    }
                }
            }
        }

        public void GetNameInit()
        {
            for (var j = 0; j < slideShow.listSquare.Count; j++)
            {
                for (var k = 0; k < slideShow.listSquareRef.Count; k++)
                {
                    if (slideShow.listSquare[j] == slideShow.listSquareRef[k])
                    {
                        int diff = ReturnDiff(j);

                        if (diff >= 0 && diff < slideShow.howManyEntries)
                        {
                            ChampionshipModeData._Championship championshipData = ReturnChampionshipGlobalParams(diff);
                            slideShow.objsInSquareList[k].txtsList[1].NewTextWithSpecificID(championshipData.listTexts[0].EntryID, championshipData.listTexts[0].listID); ;
                        }
                    }
                }
            }
        }

        public bool GetSlideshowLock()
        {
            GetSlideshowLockInit();
            return true;
        }


        public void GetSlideshowLockInit()
        {
            for (var j = 0; j < slideShow.listSquare.Count; j++)
            {
                for (var k = 0; k < slideShow.listSquareRef.Count; k++)
                {
                    if (slideShow.listSquare[j] == slideShow.listSquareRef[k])
                    {
                        int diff = ReturnDiff(j);

                        if (diff >= 0 && diff < slideShow.howManyEntries)
                        {
                            ChampionshipModeData._Championship championshipData = ReturnChampionshipGlobalParams(diff);
                            GameModeChampionship gameMode = GameModeChampionship.instance;                                      // Check if the championship is unlock 

                            SlideShowLockInfo objSlideLock = slideShow.listSquare[j].gameObject.GetComponentInChildren<SlideShowLockInfo>(true);
                            if (objSlideLock)
                            {
                                if (gameMode.listChampionshipState[ReturnListChampionshipStateID(diff)])
                                {
                                    objSlideLock.Im_Lock.gameObject.SetActive(false);
                                }
                                else
                                {
                                    objSlideLock.Im_Lock.gameObject.SetActive(true);
                                    objSlideLock.txtSlideShowLock.NewTextWithSpecificID(championshipData.listTexts[1].EntryID, championshipData.listTexts[1].listID);
                                }
                            }
                            

                            //slideShow.objsInSquareList[k].txtsList[1].NewTextWithSpecificID(championshipData.listTexts[0].EntryID, championshipData.listTexts[0].listID);
                        }
                    }
                }
            }
        }

        public bool GetBestRanking()
        {
            GetBestRankingInit();
            return true;
        }


        public void GetBestRankingInit()
        {
            for (var j = 0; j < slideShow.listSquare.Count; j++)
            {
                for (var k = 0; k < slideShow.listSquareRef.Count; k++)
                {
                    if (slideShow.listSquare[j] == slideShow.listSquareRef[k])
                    {
                        int diff = ReturnDiff(j);

                        if (diff >= 0 && diff < slideShow.howManyEntries)
                        {
                            GameModeChampionship gameMode = GameModeChampionship.instance;                                      // Check if the championship is unlock 

                            SlideShowMedal objSlideMedal = slideShow.listSquare[j].gameObject.GetComponentInChildren<SlideShowMedal>(true);

                            if (objSlideMedal)
                            {
                                int iMedal = gameMode.listChampionshipPosition[ReturnListChampionshipStateID(diff)];
                                if (iMedal == -1)    // No medal
                                    objSlideMedal.im.sprite = objSlideMedal.listMedal[0];
                                else if (iMedal == 0)  // Bronze
                                    objSlideMedal.im.sprite = objSlideMedal.listMedal[1];
                                else if (iMedal == 1) // Silver
                                    objSlideMedal.im.sprite = objSlideMedal.listMedal[2];
                                else if (iMedal == 2) // Gold
                                    objSlideMedal.im.sprite = objSlideMedal.listMedal[3];
                            }
                        }
                    }
                }
            }
        }

        int ReturnListChampionshipStateID(int diff)
        {
            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                return DataRef.instance.championshipModeData.customChampionshipList[diff];
            }
            else
            {
                return diff;
            }
        }

        //-> Call when button next is pressed to start championship
        public bool IsTrackAvailable()
        {
            GameModeChampionship gameMode = GameModeChampionship.instance;                                     
            return gameMode.listChampionshipState[ReturnListChampionshipStateID(gameMode.currentSelection)];
        }

        public void InfoLockInit()
        {
            for (var j = 0; j < slideShow.listSquare.Count; j++)
            {
                for (var k = 0; k < slideShow.listSquareRef.Count; k++)
                {
                    if (slideShow.listSquare[j] == slideShow.listSquareRef[k])
                    {
                        int diff = ReturnDiff(j);

                        if (diff >= 0 && diff < slideShow.howManyEntries)
                        {
                            SlideShowLockInfo objSlideLock = slideShow.listSquare[j].gameObject.GetComponentInChildren<SlideShowLockInfo>(true);
                            if (objSlideLock)
                            {
                                objSlideLock.ReturnLockInfoStright(0);
                            }
                        }
                    }
                }
            }
        }
    } 
}
