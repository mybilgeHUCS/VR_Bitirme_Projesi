// Description: RewardAssistant: Methods used during the reward process aftr a race.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



namespace TS.Generics
{
    public class RewardAssistant : MonoBehaviour
    {
        public bool             SeeInspector;
        private bool            b_CoinDone;
        private bool            b_CoinInProgress;

        private bool            b_TrackDone;
        private bool            b_TrackInProgress;

        private bool            b_UpdateSaveDone;
        private bool            b_UpdateSaveInProgress;

        private bool            b_UnlockChampDone;
        private bool            b_UnlockChampInProgress;

       
        public Transform        grpGifts;
        public GameObject       objRewardPrefab;
        public RewardListData   rewardList;

        public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc   callMethods;                              // Access script taht allow to call public function in this script.

        public bool             bIsRewardDone;

        //-> use "OpenPageProcess" when RewardSequence is called when a page is opened.
        //-> In the other no string is needed
        public void RewardSequence(string _Case = "OpenPageProcess")
        {
            if (_Case == "OpenPageProcess")
                InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;

            bIsRewardDone = false;

            StartCoroutine(RewardSequenceRoutine());
        }

        IEnumerator RewardSequenceRoutine(string _Case = "OpenPageProcess")
        {
            for (var i = 0; i < methodsList.Count; i++)
            {
                yield return new WaitUntil(() => callMethods.Call_One_Bool_Method(methodsList, i) == true);
            }

            bIsRewardDone = true;

            if (_Case == "OpenPageProcess")
                InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            yield return null;
        }

        public bool bCoinArcadeReward()
        {
            #region
            //-> Play the coroutine Once
            if (!b_CoinInProgress)
            {
                b_CoinInProgress = true;
                b_CoinDone = false;
                StartCoroutine(CoinRewardArcadeRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_CoinDone)
                b_CoinInProgress = false;

            return b_CoinDone;
            #endregion
        }

        IEnumerator CoinRewardArcadeRoutine()
        {
            #region
            int currentSelectedTrack = GameModeArcade.instance.currentSelection;

            TracksData.trackParams trackParams = DataRef.instance.tracksData.listTrackParams[currentSelectedTrack];
            if (!DataRef.instance.arcadeModeData.bDisplayTrackUsingTrackListOrder)
            {
                int specialOrderID = DataRef.instance.arcadeModeData.customTrackList[currentSelectedTrack];
                trackParams = DataRef.instance.tracksData.listTrackParams[specialOrderID];
            }

            int howManyVehicleInTheList = ArcadeResult.instance.vehicleList.Count;
            int playerPos = 0;
            for (var i = 0;i< howManyVehicleInTheList++; i++)
            {
                if(ArcadeResult.instance.vehicleList[i] == 0)
                {
                    playerPos = i;
                    break;
                }
            }

            if(playerPos < trackParams.listArcadeCoins.Count)
            {
                int coins = trackParams.listArcadeCoins[playerPos];
                if(coins > 0)
                {
                    yield return new WaitForSeconds(.5f);
                    GameObject newRewardPrefab = Instantiate(rewardList.rewardParamsList[0].giftPrefab, grpGifts);

                    newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry("+" + coins) });

                    newRewardPrefab.transform.SetSiblingIndex(1);

                    //-> Add coins
                    InfoCoins.instance.UpdateCoins(coins);

                    SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

                    yield return new WaitForSeconds(.5f);
                }      
            }

            b_CoinDone = true;
            yield return null;
            #endregion
        }

        public bool bTrackReward()
        {
            #region
            //-> Play the coroutine Once
            if (!b_TrackInProgress)
            {
                b_TrackInProgress = true;
                b_TrackDone = false;
                StartCoroutine(TrackRewardRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_TrackDone)
                b_TrackInProgress = false;

            return b_TrackDone;
            #endregion
        }

        IEnumerator TrackRewardRoutine()
        {
            #region
            GameObject newRewardPrefab = Instantiate(objRewardPrefab, grpGifts);

            newRewardPrefab.transform.GetChild(0).GetComponent<Image>().color = Color.red;
            newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry("New Track unlocked") });
            newRewardPrefab.transform.SetSiblingIndex(1);

            SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

            yield return new WaitForSeconds(.5f);

            b_TrackDone = true;

            yield return null;
            #endregion
        }

        public bool bUpdateSave()
        {
            #region
            //-> Play the coroutine Once
            if (!b_UpdateSaveInProgress)
            {
                b_UpdateSaveInProgress = true;
                b_UpdateSaveDone = false;
                StartCoroutine(UpdateSaveRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_UpdateSaveDone)
                b_UpdateSaveInProgress = false;

            return b_UpdateSaveDone;
            #endregion
        }

        IEnumerator UpdateSaveRoutine()
        {
            #region
            //-> Save New Player Progression
            CanvasAutoSave.instance.transform.GetChild(0).gameObject.SetActive(true);

            yield return new WaitUntil(() => SaveManager.instance.saveAndReturnTrueAFterSaveProcess(
                LoadSavePlayerProgession.instance.SaveInfoPlayer(),
                "PP_" + LoadSavePlayerProgession.instance.currentSelectedSlot));

            yield return new WaitForSeconds(.5f);
            CanvasAutoSave.instance.transform.GetChild(0).gameObject.SetActive(false);

            b_UpdateSaveDone = true;

            yield return null;
            #endregion
        }


        public bool ClearRewardSection()
        {
            List<GameObject> tmpList = new List<GameObject>();
            for (var i = 0; i < grpGifts.childCount; i++)
                tmpList.Add(grpGifts.GetChild(i).gameObject);

            for (var i = 0; i < tmpList.Count; i++)
                Destroy(tmpList[i]);

            return true;
        }

        public bool bCoinTrackChampReward()
        {
            #region
            //-> Play the coroutine Once
            if (!b_CoinInProgress)
            {
                b_CoinInProgress = true;
                b_CoinDone = false;
                StartCoroutine(CoinTrackChampRewardRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_CoinDone)
                b_CoinInProgress = false;

            return b_CoinDone;
            #endregion
        }

        IEnumerator CoinTrackChampRewardRoutine()
        {
            #region
            int currentChampionship = GameModeChampionship.instance.currentSelection; 
            int currentTrackInTheList = GameModeChampionship.instance.currentTrackInTheList;
            ChampionshipModeData championshipModeData = DataRef.instance.championshipModeData;

            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                int specialOrderID = championshipModeData.customChampionshipList[currentChampionship];
                currentChampionship = specialOrderID;
            }

            int coins = championshipModeData.listOfChampionship[currentChampionship].listTrackParams[currentTrackInTheList].winCoinsTrack;
            if (coins > 0)
            {
                yield return new WaitForSeconds(.5f);
                GameObject newRewardPrefab = Instantiate(rewardList.rewardParamsList[0].giftPrefab, grpGifts);

                newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry("+" + coins) });

                newRewardPrefab.transform.SetSiblingIndex(1);

                //-> Add coins
                InfoCoins.instance.UpdateCoins(coins);

                SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

                yield return new WaitForSeconds(.5f);
            }

            b_CoinDone = true;
            yield return null;
            #endregion
        }

        public bool bUnlockTrackChampReward()
        {
            #region
            //-> Play the coroutine Once
            if (!b_TrackInProgress)
            {
                b_TrackInProgress = true;
                b_TrackDone = false;
                StartCoroutine(UnlockTrackChampRewardRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_TrackDone)
                b_TrackInProgress = false;

            return b_TrackDone;
            #endregion
        }

        IEnumerator UnlockTrackChampRewardRoutine()
        {
            #region
            int currentChampionship = GameModeChampionship.instance.currentSelection;
            int currentTrackInTheList = GameModeChampionship.instance.currentTrackInTheList;

            ChampionshipModeData championshipModeData = DataRef.instance.championshipModeData;

            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                int specialOrderID = championshipModeData.customChampionshipList[currentChampionship];
                currentChampionship = specialOrderID;
            }

            int currentSelectedTrack = championshipModeData.listOfChampionship[currentChampionship].listTrackParams[GameModeChampionship.instance.currentTrackInTheList].TrackID;

            bool bunlockInArcadeMode = championshipModeData.listOfChampionship[currentChampionship].listTrackParams[currentTrackInTheList].UnlockTrackOnArcadeMode;
            bool TrackCurrentArcadeState = GameModeArcade.instance.listArcadeTrackState[currentSelectedTrack];

            if (bunlockInArcadeMode == true && TrackCurrentArcadeState == false)
            {
                GameObject newRewardPrefab = Instantiate(rewardList.rewardParamsList[1].giftPrefab, grpGifts);

                newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextWithSpecificID(152,0);

                newRewardPrefab.transform.SetSiblingIndex(1);

                GameModeArcade.instance.listArcadeTrackState[currentSelectedTrack] = true;

                SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

                yield return new WaitForSeconds(.5f);
            }


            bool bunlockInTTMode = championshipModeData.listOfChampionship[currentChampionship].listTrackParams[currentTrackInTheList].UnlockTrackOnTimeTrialMode;
            bool TrackCurrentTTState = GameModeTimeTrial.instance.listTimeTrialTrackState[currentSelectedTrack];


            if (bunlockInTTMode == true && TrackCurrentTTState == false)
            {
                GameObject newRewardPrefab = Instantiate(rewardList.rewardParamsList[1].giftPrefab, grpGifts);

                newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextWithSpecificID(153, 0);

                newRewardPrefab.transform.SetSiblingIndex(1);

                GameModeTimeTrial.instance.listTimeTrialTrackState[currentSelectedTrack] = true;

                SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

                yield return new WaitForSeconds(.5f);
            }

            b_TrackDone = true;

            yield return null;
            #endregion
        }





        public bool bCoinEndChampReward()
        {
            #region
            //-> Play the coroutine Once
            if (!b_CoinInProgress)
            {
                b_CoinInProgress = true;
                b_CoinDone = false;
                StartCoroutine(CoinEndChampRewardRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_CoinDone)
                b_CoinInProgress = false;

            return b_CoinDone;
            #endregion
        }

        IEnumerator CoinEndChampRewardRoutine()
        {
            #region
            int currentChampionship = GameModeChampionship.instance.currentSelection;
            int currentTrackInTheList = GameModeChampionship.instance.currentTrackInTheList;
            ChampionshipModeData championshipModeData = DataRef.instance.championshipModeData;

            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                int specialOrderID = championshipModeData.customChampionshipList[currentChampionship];
                currentChampionship = specialOrderID;
            }

            //-> Check if the Championship is finished
            if(!ChampionshipResult.instance.isChampionshipFinished)
            {
                
            }
            else
            {
                //-> Check the best player between P1 and 2
                //Debug.Log("Championship is finished");

                //-> Check the position of the best player (P1|P2)
                int totalPoints = GameModeChampionship.instance.listScore[0];
                int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
                int bestPlayer = 0;
                for (var i = 1; i < howManyPlayer; i++)
                {
                    if (totalPoints < GameModeChampionship.instance.listScore[i])
                    {
                        totalPoints = GameModeChampionship.instance.listScore[i];
                        bestPlayer = i;
                    }
                }

                //-> Add coins depending the position at the end of the championship
                int pos = 0;
                for(var i = 0;i< GameModeChampionship.instance.listScore.Count; i++)
                {
                    if(totalPoints < GameModeChampionship.instance.listScore[i] && i != bestPlayer)
                    {
                        pos++;
                    }
                }

                if (pos < championshipModeData.listOfChampionship[currentChampionship].listChampionshipCoins.Count)
                {
                    int coins = championshipModeData.listOfChampionship[currentChampionship].listChampionshipCoins[pos];
                    if (coins > 0)
                    {
                        yield return new WaitForSeconds(.5f);
                        GameObject newRewardPrefab = Instantiate(rewardList.rewardParamsList[0].giftPrefab, grpGifts);

                        newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry("+" + coins) });

                        newRewardPrefab.transform.SetSiblingIndex(1);

                        //-> Add coins
                        InfoCoins.instance.UpdateCoins(coins);

                        SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

                        yield return new WaitForSeconds(.5f);
                    }
                }
            }

            b_CoinDone = true;
            yield return null;
            #endregion
        }



        public bool bUnlockNewChampReward()
        {
            #region
            //-> Play the coroutine Once
            if (!b_UnlockChampInProgress)
            {
                b_UnlockChampInProgress = true;
                b_UnlockChampDone = false;
                StartCoroutine(UnlockNewChampRewardRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_UnlockChampDone)
                b_UnlockChampInProgress = false;

            return b_UnlockChampDone;
            #endregion
        }

        IEnumerator UnlockNewChampRewardRoutine()
        {
            #region
            int currentChampionship = GameModeChampionship.instance.currentSelection;
            ChampionshipModeData championshipModeData = DataRef.instance.championshipModeData;

            if (!DataRef.instance.championshipModeData.bDisplayChampionshipUsingListOrder)
            {
                int specialOrderID = championshipModeData.customChampionshipList[currentChampionship];
                currentChampionship = specialOrderID;
            }

            //-> Check if the Championship is finished
            if (!ChampionshipResult.instance.isChampionshipFinished)
            {

            }
            else
            {
                //-> Check the best player between P1 and P2
                //Debug.Log("Championship is finished");

                //-> Check the position of the best player (P1|P2)
                int totalPoints = GameModeChampionship.instance.listScore[0];
                int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
                int bestPlayer = 0;
                for (var i = 1; i < howManyPlayer; i++)
                {
                    if (totalPoints < GameModeChampionship.instance.listScore[i])
                    {
                        totalPoints = GameModeChampionship.instance.listScore[i];
                        bestPlayer = i;
                    }
                }

                //-> Unlock new Championship if needed 
                int pos = 0;
                for (var i = 0; i < GameModeChampionship.instance.listScore.Count; i++)
                {
                    if (totalPoints < GameModeChampionship.instance.listScore[i] && i != bestPlayer)
                    {
                        pos++;
                    }
                }

                int whichChampionshipToUnlock = championshipModeData.listOfChampionship[currentChampionship].whichChampionshipToUnlock;
                if (pos <= championshipModeData.listOfChampionship[currentChampionship].whichPlayerPosToUnlock && !GameModeChampionship.instance.listChampionshipState[whichChampionshipToUnlock])
                {
                    GameObject newRewardPrefab = Instantiate(rewardList.rewardParamsList[2].giftPrefab, grpGifts);

                    newRewardPrefab.transform.GetChild(1).GetComponent<CurrentText>().NewTextWithSpecificID(154, 0);

                    newRewardPrefab.transform.SetSiblingIndex(1);

                    GameModeChampionship.instance.listChampionshipState[whichChampionshipToUnlock] = true;

                    SoundFxManager.instance.Play(SfxList.instance.listAudioClip[rewardList.sfxID]);

                    yield return new WaitForSeconds(.5f);
                }
            }

            b_UnlockChampDone = true;
            yield return null;
            #endregion
        }
    }
}
