// Description: Quit_IGAssistant: Methods used in popup Quit in the gameplay scene.
using UnityEngine;

namespace TS.Generics
{
    public class Quit_IGAssistant : MonoBehaviour
    {
        public static Quit_IGAssistant instance;
        public int pageQuitIG = 8;


        private void Awake()
        {
            if (instance == null)
                instance = this;
        }


        public void OpenQuitIGPage()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                CanvasMainMenuManager.instance.ComeBackFromPageList[0].selectedButtonWhenBackToPage = TS_EventSystem.instance.eventSystem.currentSelectedGameObject;
                CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage = CanvasMainMenuManager.instance.currentSelectedPage;

                PageIn currentMenu = CanvasMainMenuManager.instance.listMenu[pageQuitIG].transform.parent.GetComponent<PageIn>();
                currentMenu.DisplayNewPage(pageQuitIG);
            }
            #endregion
        }

        public void CloseQuitIGPage()
        {
            #region
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            GameObject newSelectedButton = CanvasMainMenuManager.instance.ComeBackFromPageList[0].selectedButtonWhenBackToPage;
            //Debug.Log("Exit leaderboard");

            StartCoroutine(CanvasMainMenuManager.instance.listMenu[pageQuitIG].transform.parent.GetComponent<PageOut>().DisableCurrentPageAndSelectManualyANewPage(
                CanvasMainMenuManager.instance.ComeBackFromPageList[0].comeBackToPage,
                false,
                .35f,
                newSelectedButton));
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            #endregion
        }
    }
}
