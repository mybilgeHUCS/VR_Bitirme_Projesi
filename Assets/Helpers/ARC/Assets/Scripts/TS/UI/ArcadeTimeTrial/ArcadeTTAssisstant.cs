// Description: ArcadeTTAssisstant: Attached to Grp_Game_ArcadeTimeTrialTrackSelection in the Main Menu.
// Used to select a difficulty in Arcade Mode
using UnityEngine;

namespace TS.Generics
{
    public class ArcadeTTAssisstant : MonoBehaviour
    {
        public GameObject   objLeaderboardButton;
        public GameObject   objDifficulty;
        public CurrentText  txtDifficulty;

        public bool InitDependingMode()
        {
            //-> 1 player and Time Trial Selected
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 1 &&
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)    
            {
                objLeaderboardButton.SetActive(true);
                objDifficulty.SetActive(false);
            }
            else
            {
                objLeaderboardButton.SetActive(false);
                objDifficulty.SetActive(true);
                SelectDifficulty(0);
            }
               
            return true;
        }

        public bool BCreateNameList()
        {
            GameModeGlobal.instance.GenerateNameList();

            return true;
        }

        public void SelectDifficulty(int dir)
        {
            //-> Access DifficultyManagerData
            int howManyDifficulty = DataRef.instance.difficultyManagerData.difficultyParamsList.Count;

            //-> Access PlayerMainMenuSelection Data (current Difficulty)
            int currentDifficulty = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentDifficulty;

            currentDifficulty += dir + howManyDifficulty;
            currentDifficulty %= howManyDifficulty;

            InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentDifficulty = currentDifficulty;

            int txtFolder = DataRef.instance.difficultyManagerData.difficultyParamsList[currentDifficulty].folderTxt;
            int txtID = DataRef.instance.difficultyManagerData.difficultyParamsList[currentDifficulty].idTxt;
            txtDifficulty.NewTextWithSpecificID(txtID, txtFolder);
        }
    }
}
