// Description: TSAssistantSlideShowArcadeTT: Methods to manage slideshow behavior in Arcade and TT mode
using UnityEngine;

namespace TS.Generics
{
    public class TSAssistantSlideShowArcadeTT : MonoBehaviour
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
            //-> Arcade Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                //-> Display tracks using the data order
                if (DataRef.instance.arcadeModeData.bDisplayTrackUsingTrackListOrder)
                {
                    return DataRef.instance.tracksData.listTrackParams.Count;
                }
                //-> Display tracks using a specific order
                else
                {
                    return DataRef.instance.arcadeModeData.customTrackList.Count;
                }
            }
            //-> Time Trial
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                //-> Display tracks using the data order
                if (DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
                {
                    return DataRef.instance.tracksData.listTrackParams.Count;
                }
                //-> Display tracks using a specific order
                else
                {
                    return DataRef.instance.timeTrialModeData.customTrackList.Count;
                }
            }

            return 0;
        }

        public int GetCurrentSelection()
        {
            //-> Arcade Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                return GameModeArcade.instance.currentSelection;
            }
            //-> Time Trial
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                return GameModeTimeTrial.instance.currentSelection;
            }

            return 0;
        }

        public void SetCurrentSelection()
        {
            //-> Arcade Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                GameModeArcade.instance.currentSelection = slideShow.currentSelection;
                TracksData.trackParams tracksData = ReturnTrackParams(GameModeArcade.instance.currentSelection);

                //Debug.Log("test -> " + GameModeArcade.instance.currentSelection);

                GameModeGlobal.instance.currentSelectedTrack = tracksData.sceneName;

            }
            //-> Time Trial
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                GameModeTimeTrial.instance.currentSelection = slideShow.currentSelection;
                TracksData.trackParams tracksData = ReturnTrackParams(GameModeTimeTrial.instance.currentSelection);
                GameModeGlobal.instance.currentSelectedTrack = tracksData.sceneName;
            }
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

        TracksData.trackParams ReturnTrackParams(int diff)
        {
            //TracksData.trackParams tracksData = null;
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                if (!DataRef.instance.arcadeModeData.bDisplayTrackUsingTrackListOrder)
                {
                    int specialOrderID = DataRef.instance.arcadeModeData.customTrackList[diff];
                    return DataRef.instance.tracksData.listTrackParams[specialOrderID];
                }
                else
                {
                    return DataRef.instance.tracksData.listTrackParams[diff];
                }
            }
            else
            {
                if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
                {
                    int specialOrderID = DataRef.instance.timeTrialModeData.customTrackList[diff];
                    return DataRef.instance.tracksData.listTrackParams[specialOrderID];
                }
                else
                {
                    return DataRef.instance.tracksData.listTrackParams[diff];
                }
            }
        }

        //-> Get Sprite for each track
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
                            TracksData.trackParams tracksData = ReturnTrackParams(diff);
                            slideShow.objsInSquareList[k].imagesList[0].sprite = tracksData.trackSprite;
                        }
                    }
                }
            }
        }

        //-> Get Track Name
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
                            TracksData.trackParams tracksData = ReturnTrackParams(diff);
                            slideShow.objsInSquareList[k].txtsList[1].NewTextWithSpecificID(tracksData.NameIDMultiLanguage, tracksData.selectedListMultiLanguage);
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
                            TracksData.trackParams tracksData = ReturnTrackParams(diff);
                           
                            SlideShowLockInfo objSlideLock = slideShow.listSquare[j].gameObject.GetComponentInChildren<SlideShowLockInfo>(true);
                            if (objSlideLock)
                            {
                                //-> Arcade Mode
                                if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
                                {
                                    GameModeArcade gameMode = GameModeArcade.instance;
                                    //Debug.Log(("lock: " + gameMode.listArcadeTrackState[ReturnStateArcadeID(diff)]));
                                    if (gameMode.listArcadeTrackState[ReturnStateArcadeID(diff)])
                                    {
                                        objSlideLock.Im_Lock.gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        objSlideLock.Im_Lock.gameObject.SetActive(true);
                                        objSlideLock.txtSlideShowLock.NewTextWithSpecificID(tracksData.listTexts[1].EntryID, tracksData.listTexts[1].listID);
                                    }
                                }
                                //-> Time Trial Mode
                                else
                                {
                                    GameModeTimeTrial gameMode = GameModeTimeTrial.instance;                                      
                                    // Check if the track is unlock
                                    if (gameMode.listTimeTrialTrackState[ReturnStateTimeTrialID(diff)])
                                    {
                                        objSlideLock.Im_Lock.gameObject.SetActive(false);
                                    }
                                    else
                                    {
                                        objSlideLock.Im_Lock.gameObject.SetActive(true);
                                        objSlideLock.txtSlideShowLock.NewTextWithSpecificID(tracksData.listTexts[1].EntryID, tracksData.listTexts[1].listID);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        int ReturnStateArcadeID(int diff)
        {
            if (!DataRef.instance.arcadeModeData.bDisplayTrackUsingTrackListOrder)
            {
                return DataRef.instance.arcadeModeData.customTrackList[diff];
            }
            else
            {
                return diff;
            }
        }

        int ReturnStateTimeTrialID(int diff)
        {
            if (!DataRef.instance.timeTrialModeData.bDisplayTrackUsingTrackListOrder)
            {
                return DataRef.instance.timeTrialModeData.customTrackList[diff];
            }
            else
            {
                return diff;
            }
        }

        //-> Call when button next is pressed to start a track
        public bool IsTrackAvailable()
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                GameModeArcade gameMode = GameModeArcade.instance;                                      // Check if the track is unlock 
                return gameMode.listArcadeTrackState[ReturnStateArcadeID(gameMode.currentSelection)];
            }
            else
            {
                GameModeTimeTrial gameMode = GameModeTimeTrial.instance;                                      // Check if the track is unlock 
                return gameMode.listTimeTrialTrackState[ReturnStateTimeTrialID(gameMode.currentSelection)];
            }
        }

        public void InitScenName()
        {
            //-> Arcade Mode
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                GameModeArcade.instance.currentSelection = slideShow.currentSelection;
                TracksData.trackParams tracksData = ReturnTrackParams(GameModeArcade.instance.currentSelection);
                GameModeGlobal.instance.currentSelectedTrack = tracksData.sceneName;
            }
            //-> Time Trial
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                GameModeTimeTrial.instance.currentSelection = slideShow.currentSelection;
                TracksData.trackParams tracksData = ReturnTrackParams(GameModeTimeTrial.instance.currentSelection);
                GameModeGlobal.instance.currentSelectedTrack = tracksData.sceneName;
            }
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
                            TracksData.trackParams tracksData = ReturnTrackParams(diff);

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
