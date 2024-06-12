// Desciption: DisplayPauseMenu. Start/Stop the pause
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class DisplayPauseMenu : MonoBehaviour
    {

        public int escapeButtonID = 0;      // refers to Input ID in object InfoInputs
        public int pauseButtonID = 2;       // refers to Input ID in object InfoInputs
        public List<int> pagesThatAllowsPause = new List<int>();


        public bool b_EnablePauseModule = true; // Pause Manager is enabled only in gameplay scenes not in the Main Menu Scene

        public void Start()
        {
            if (b_EnablePauseModule)
            {
                for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
                {
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[escapeButtonID].OnGetKeyDownReceived += OnPauseAction;
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[pauseButtonID].OnGetKeyDownReceived += OnPauseAction;
                }
            }
        }
       

        public void OnDestroy()
        {
            if (b_EnablePauseModule)
            {
                for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
                {
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[escapeButtonID].OnGetKeyDownReceived -= OnPauseAction;
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[pauseButtonID].OnGetKeyDownReceived -= OnPauseAction;
                }
            }
        }

        //-> Enable the menu page in gameplay scene
        public void EnableMenuPage()
        {
            Debug.Log("Pause Page Displayed");
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[CanvasMainMenuManager.instance.menuPageInGameplayScene].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(CanvasMainMenuManager.instance.menuPageInGameplayScene);
        }

        //-> Disable the menu page in gameplay scene
        public void DisableMenuPage()
        {
            //Debug.Log("Stop Pause Menu Page");
            PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[CanvasMainMenuManager.instance.gamePageInGameplayScene].transform.parent.GetComponent<PageIn>();
            currentMenu.DisplayNewPage(CanvasMainMenuManager.instance.gamePageInGameplayScene);
        }

        public void OnPauseAction()
        {
            if (IsPauseAllowed())
            {
                //Debug.Log("Pause");
                PauseManager.instance.Bool_IsGamePaused = !PauseManager.instance.Bool_IsGamePaused;
                //-> Pause the game
                if (PauseManager.instance.Bool_IsGamePaused)// Check if the game is paused
                { // Check if Game page is displayed in gamplay scene
                    PauseManager.instance.PauseGame(0);
                    //PauseManager.instance.OnPause?.Invoke();
                }
                //-> Unpause the game
                else
                { // Check if Menu page is displayed in gamplay scene
                    PauseManager.instance.UnpauseGame(0);
                    //PauseManager.instance.OnUnPause?.Invoke();
                }
            }
        }


        bool IsPauseAllowed()
        {
            if (MusicManager.instance.b_IsFading)
                return false;

            if (!PauseManager.instance.isPauseModeEnable)
                return false;

            if (!b_EnablePauseModule)
                return false;

            //-> 2 Players. Allows a player to enable pause even if the other player has finished the race.
            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;

            if(howManyPlayer > 1)
            {
                if(!InfoPlayerTS.instance.returnCheckState(0) ||
                    (LapCounterAndPosition.instance.posList[0].IsRaceComplete &&
                    LapCounterAndPosition.instance.posList[1].IsRaceComplete))
                    return false;
            }
            else
            {
                if (!InfoPlayerTS.instance.returnCheckState(0) ||
                    LapCounterAndPosition.instance.posList[0].IsRaceComplete)
                    return false;
            }

           

           

            foreach (int value in pagesThatAllowsPause)
                if (value == CanvasMainMenuManager.instance.currentSelectedPage) return true;

            return false;
        }


        public void OnPauseAction2()
        {
            Debug.Log("--> Pause <--");
        }

        public void OnUnPauseAction()
        {
            Debug.Log("--> UnPause <--");
        }


        public void OnPauseTimeScale()
        {
            Time.timeScale = 0;
            Debug.Log("--> Pause <--");
        }

        public void OnUnPauseTimeScale()
        {
            Time.timeScale = 1;
            Debug.Log("--> UnPause <--");
        }


        //-> Cursor Visibility
        public void CusrorVisibility(bool state)
        {
            Debug.Log("Cursor " + state);
            Cursor.visible = state;
        }

        //-> Music Pause Menu
        AudioClip rememberMusicAudioClip;
        float clipPosition;
        public void MusicMenuPause(int musicID)
        {
            rememberMusicAudioClip = MusicManager.instance.currentAudioClip;
            int currentAudioSource = MusicManager.instance.currentAudioSource;
            clipPosition = MusicManager.instance.ListAudioSource[currentAudioSource].time;
            if (musicID == -1)
                MusicManager.instance.MFadeOut(1);
            else
            {
                if (MusicList.instance.ListAudioClip.Count > musicID)
                {
                    MusicManager.instance.MCrossFade(1, MusicList.instance.ListAudioClip[musicID]);
                }
            }
        }
        public void MusicMenuUnPause()
        {
            if(rememberMusicAudioClip != null)
            {
                MusicManager.instance.MCrossFade(1, rememberMusicAudioClip,0, clipPosition);
            }
            else
            {
                MusicManager.instance.MFadeOut(1);
            }
           
        }
    }

}
