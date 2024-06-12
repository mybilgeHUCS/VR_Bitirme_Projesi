// Description: CarSelectionPageAssistant: attached to CarSelectionAssitantP1 an CarSelectionAssitantP2
// in Main Menu scene. Page Grp_Game_CarSelection
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class CarSelectionPageAssistant : MonoBehaviour
    {
        public List<int> pageIDToDisplayDependingGameMode = new List<int>();

        public List<GameObject> objPlayerParamsList;
        public RectTransform grpPlayerVehicle;
        public float pivotV = .6f;

        public List<GameObject> btnList = new List<GameObject>();

        public List<Button> btnNavigationList = new List<Button>();
        //private List<Transform> navigatIonList = new List<Transform>();


        [System.Serializable]
        public class ObjState
        {
            public GameObject Obj;
            public List<bool> listStateDependingPlayerNumber = new List<bool>(3) { true, true, true };
        }

        public List<ObjState> listObjState = new List<ObjState>();

        //-> Use on Button_Championship (page Grp_Game_ChooseMode).
        public void OpenPageDependingGameMode()
        {
            if (InfoPlayerTS.instance.returnCheckState(0))  // Check if the player can press a button
            {
                PageIn currentMenu;
                switch (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode)
                {
                    case 0:
                        currentMenu = CanvasMainMenuManager.instance.listMenu[pageIDToDisplayDependingGameMode[0]].transform.parent.GetComponent<PageIn>();
                        currentMenu.DisplayNewPage(pageIDToDisplayDependingGameMode[0]);
                        break;
                    case 1:
                        currentMenu = CanvasMainMenuManager.instance.listMenu[pageIDToDisplayDependingGameMode[1]].transform.parent.GetComponent<PageIn>();
                        currentMenu.DisplayNewPage(pageIDToDisplayDependingGameMode[1]);
                        break;
                    case 2:
                        currentMenu = CanvasMainMenuManager.instance.listMenu[pageIDToDisplayDependingGameMode[2]].transform.parent.GetComponent<PageIn>();
                        currentMenu.DisplayNewPage(pageIDToDisplayDependingGameMode[2]);
                        break;
                }
            }
        }

        public bool Init()
        {
            int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
            if (howManyPlayer == 1)
            {
                for (var i = 0; i < objPlayerParamsList.Count; i++)
                {
                    if (i == 0)
                        objPlayerParamsList[i].SetActive(true);
                    else
                        objPlayerParamsList[i].SetActive(false);

                    if (grpPlayerVehicle) grpPlayerVehicle.pivot = new Vector2(.5f, .5f);
                }


                for (var i = 0; i < listObjState.Count; i++)
                {
                    listObjState[i].Obj.SetActive(listObjState[i].listStateDependingPlayerNumber[0]);
                }
            }
            else
            {
                for (var i = 0; i < objPlayerParamsList.Count; i++)
                    objPlayerParamsList[i].SetActive(true);

                for (var i = 0; i < listObjState.Count; i++)
                {
                    listObjState[i].Obj.SetActive(listObjState[i].listStateDependingPlayerNumber[1]);
                }

                if (grpPlayerVehicle) grpPlayerVehicle.pivot = new Vector2(.5f, pivotV);
            }

            return true;
        }

        public void ChooseButtonDependingGameMode()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;

            GameObject newButton = null;
            if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 0)
            {
                newButton = btnList[0];
            }
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 1)
            {
                newButton = btnList[1];
            }
            else if (InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentGameMode == 2)
            {
                newButton = btnList[2];
            }

            if (IntroInfo.instance.globalDatas.returnPageOutSetSelectedButtonAllowed())
                TS_EventSystem.instance.eventSystem.SetSelectedGameObject(newButton);
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }
}