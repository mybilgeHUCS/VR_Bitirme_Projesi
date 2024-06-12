// Description: IntroInfoAssistant: The script contained methods used by IntroInfo.
using System.Collections;
using UnityEngine;


namespace TS.Generics
{
    public class IntroInfoAssistant : MonoBehaviour
    {
        private int loadInProgress = 0;
        public bool bDisplayDebugLog;

        //-> Load Language Datas
        public bool B_01_LoadLanguage()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(LoadLanguageRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3) {
                loadInProgress = 0;
                return true;
            }    
            else
                return false;
            #endregion
        }

        //-> Corontine Init the language
        IEnumerator LoadLanguageRoutine()
        {
            #region
            if (bDisplayDebugLog) Debug.Log("Load Language");
            yield return new WaitUntil(() => LanguageManager.instance.Bool_InitLanguage() == true);
            loadInProgress = 2;
            yield return null;
            #endregion
        }

        //-> Load Player Progression Datas
        public bool B_02_LoadPlayerProgression()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(LoadPlayerProgressionRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }

        //-> Corontine Init the Player Progression
        IEnumerator LoadPlayerProgressionRoutine()
        {
            #region
            if (bDisplayDebugLog) Debug.Log("Load Player Progression");
            yield return new WaitUntil(() => LoadSavePlayerProgession.instance.B_LoadPlayerProgression() == true);
            loadInProgress = 2;
            yield return null;
            #endregion
        }


        //-> Load Sound parameters Datas
        public bool B_03_LoadSoundParams()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(SoundParamsRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        IEnumerator SoundParamsRoutine()
        {
            #region
            yield return new WaitUntil(() => SoundManager.instance.B_LoadGroupVolumes() == true);
            loadInProgress = 2;
            yield return null;
            #endregion
        }


        //-> Load Inputs Datas
        public bool B_04_LoadInputs()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(InputsRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        IEnumerator InputsRoutine()
        {
            #region
            yield return new WaitUntil(() => InfoInputs.instance.Bool_LoadAllInputs() == true);
            loadInProgress = 2;
            yield return null;
            #endregion
        }

        //-> Load Vehicles Datas
        public bool B_05_LoadVehicles()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(VehiclesRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        IEnumerator VehiclesRoutine()
        {
            #region
            yield return new WaitForSeconds(1);

            loadInProgress = 2;

            if(bDisplayDebugLog)Debug.Log("Vehicles Loaded");

            yield return null;
            #endregion
        }

        //-> Load Championship
        public bool B_06_LoadChampionship()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(ChampionshipRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        IEnumerator ChampionshipRoutine()
        {
            #region
            //yield return new WaitForSeconds(1);

            loadInProgress = 2;

            if (bDisplayDebugLog) Debug.Log("Championship Loaded");

            yield return null;
            #endregion
        }

        //-> Load Tracks Datas
        public bool B_07_LoadTracks()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(TracksRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        IEnumerator TracksRoutine()
        {
            #region
            loadInProgress = 2;
            yield return null;
            #endregion
        }

        //-> Load Leaderboards Datas
        public bool B_08_LoadLeaderboards()
        {
            #region
            //-> Play the coroutine Once
            if (loadInProgress == 0)
            {
                loadInProgress = 1;
                StartCoroutine(LeaderboardsRoutine());
            }
            //-> Check if the coroutine is finished
            else if (loadInProgress == 2)
                loadInProgress = 3;

            if (loadInProgress == 3)
            {
                loadInProgress = 0;
                return true;
            }
            else
                return false;
            #endregion
        }


        IEnumerator LeaderboardsRoutine()
        {
            #region
            loadInProgress = 2;

            if (bDisplayDebugLog) Debug.Log("Leaderboards Loaded");

            string sData = SaveManager.instance.LoadDAT("TL_" + LoadSavePlayerProgession.instance.currentSelectedSlot);

            GameModeTimeTrial.instance.InitLeaderboards(sData);


            yield return null;
            #endregion
        }


        //-> Example Load Custom Datas
        public bool B_09_LoadCustomData()
        {
            #region
            // Code: Load custom data
            return true;
            #endregion
        }


        //-> Patcher system
        public bool B_10_Patcher()
        {
            #region
            if (bDisplayDebugLog) Debug.Log("Check for patch");
            return true;
            #endregion
        }
    }
}
