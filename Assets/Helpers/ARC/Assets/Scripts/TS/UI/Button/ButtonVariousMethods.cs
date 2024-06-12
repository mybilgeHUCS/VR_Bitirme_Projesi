// Description: ButtonVariousMethods:
using UnityEngine;


namespace TS.Generics
{
    public class ButtonVariousMethods : MonoBehaviour
    {
        //-> Use on Button_Solo (page Grp_Page_HomePage).
        public void SetToSoloAndOpenPage(int PageNumber)
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer = 1;
                GameModeGlobal.instance.GenerateNameList();
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }
        }

        //-> Use on Button_Versus (page Grp_Page_HomePage).
        public void SetToVersusAndOpenPage(int PageNumber)
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer = 2;
                GameModeGlobal.instance.GenerateNameList();
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }
        }

        //-> Use on Button_Arcade (page Grp_Game_ChooseMode).
        public void SetToArcadeAndOpenPage(int PageNumber)
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode = 0;
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }
        }

        //-> Use on Button_TimeTrial (page Grp_Game_ChooseMode).
        public void SetToTimeTrialAndOpenPage(int PageNumber)
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode = 1;
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }
        }

        //-> Use on Button_Championship (page Grp_Game_ChooseMode).
        public void SetToChampionshipAndOpenPage(int PageNumber)
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode = 2;
                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[PageNumber].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(PageNumber);
            }
        }

       public void Test()
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                Debug.Log("Start Race Arcade | Time Trial");
            }
        }


        public void OpenQuitIGPage()
        {
            Quit_IGAssistant.instance.OpenQuitIGPage();
        }


        public void DeletePlayerPrefs(string name)
        {
            PlayerPrefs.DeleteKey(name);
        }
    }
}