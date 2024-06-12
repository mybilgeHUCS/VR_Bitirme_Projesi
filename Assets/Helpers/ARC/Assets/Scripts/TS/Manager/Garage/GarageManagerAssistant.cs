//Description: GarageManagerAssistant: Attached to buttons that open/close the garage
using UnityEngine;

namespace TS.Generics
{
    public class GarageManagerAssistant : MonoBehaviour
    {
        //-> Call when button back is pressed and page Grp_Garage is closed (call from pageOut.cs)
        public void ExitGarage()
        {
            #region
            if (Leaderboard.instance)
            {
                InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
                GameObject newSelectedButton = CanvasMainMenuManager.instance.ComeBackFromPageList[1].selectedButtonWhenBackToPage;
                Debug.Log("Exit Garage");

                StartCoroutine(CanvasMainMenuManager.instance.listMenu[GarageManager.instance.garagePageID].transform.parent.GetComponent<PageOut>().DisableCurrentPageAndSelectManualyANewPage(
                    CanvasMainMenuManager.instance.ComeBackFromPageList[1].comeBackToPage,
                    false,
                    0,
                    newSelectedButton));

                //-> Go back to car selection. Enable Cam P1 vehicle
                if (CanvasMainMenuManager.instance.ComeBackFromPageList[1].comeBackToPage == 15)
                {
                    //CarSelectionManager.instance.StateGrpCamP1(true, false);
                    CarSelectionManager.instance.StartCoroutine(CarSelectionManager.instance.EnterCarSelectionRoutine());
                }

                InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            }
            #endregion
        }


        // Use by the buttons Button_Garage
        public void OpenGarage()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
                Debug.Log("Open Garage");
                CanvasMainMenuManager.instance.ComeBackFromPageList[1].comeBackToPage = CanvasMainMenuManager.instance.currentSelectedPage;
                CanvasMainMenuManager.instance.ComeBackFromPageList[1].selectedButtonWhenBackToPage = TS_EventSystem.instance.eventSystem.currentSelectedGameObject;

                StartCoroutine(GarageManager.instance.OpenGarageRoutine());
                InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
            }
            #endregion
        }
    }

}
