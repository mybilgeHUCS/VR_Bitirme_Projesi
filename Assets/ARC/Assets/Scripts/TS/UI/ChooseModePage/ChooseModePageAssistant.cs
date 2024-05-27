// Description: ChooseModePageAssistant: Methods used in Main Menu scene. Called by the page Grp_Game_ChooseMode 
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class ChooseModePageAssistant : MonoBehaviour
    {
        public List<GameObject> objsList = new List<GameObject>();

        public List<GameObject> btnList = new List<GameObject>();

        public bool SoloOrVersus()
        {
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 1)
                foreach (GameObject obj in objsList) obj.SetActive(true);
            else
                foreach (GameObject obj in objsList) obj.SetActive(false);

            return true;
        }

        public void ChooseButtonDependingNumberOfPlayer()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
          
            GameObject newButton = null;
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 1)
            {
                newButton = btnList[0];
            }
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
            {
                newButton = btnList[1];
            }

            if (IntroInfo.instance.globalDatas.returnPageOutSetSelectedButtonAllowed())
                TS_EventSystem.instance.eventSystem.SetSelectedGameObject(newButton);
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }
}