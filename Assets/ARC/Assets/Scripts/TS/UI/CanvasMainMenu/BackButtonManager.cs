// Description: BackButtonManager.cs. Attached to EventSystem object.
// If the main menu is displayed on screen and if back button is pressed the previous menu page is displayed on the screen.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class BackButtonManager : MonoBehaviour
    {
        public static BackButtonManager instance = null;

        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.

        public int backButtonID;                                        // Select the ID that correspond to the back button in Object InfoInputs.


        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
            else if (instance != this)
                Destroy(gameObject);
        }
        public void Start()
        {
            for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
            {
                if (i == 0)
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[backButtonID].OnGetKeyDownReceived += CheckBackButtonPlayerOne;
                else
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[backButtonID].OnGetKeyDownReceived += CheckBackButtonOtherPlayers;
            }
                
        }

        public void OnDestroy()
        {
            for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
            {
                if (i == 0)
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[backButtonID].OnGetKeyDownReceived -= CheckBackButtonPlayerOne;
                else
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[backButtonID].OnGetKeyDownReceived -= CheckBackButtonOtherPlayers;
            }

            Debug.Log("Destroy Back Button Manager");
        }

        private void CheckBackButtonPlayerOne()
        {
            #region
            if (!returnIfSpecialConditionAvailable() &&
                    InfoPlayerTS.instance.returnCheckState(0))
            {
                Debug.Log("Submit Joystick back Pressed : ");
                int currentPage = CanvasMainMenuManager.instance.currentSelectedPage;
                StartCoroutine(CanvasMainMenuManager.instance.listMenu[currentPage].transform.parent.GetComponent<PageOut>().BackMenu());
            }
            #endregion
        }

        //-> Check if a special condition is available
        bool returnIfSpecialConditionAvailable()
        {
            #region
            for (var i = 0; i < methodsList.Count; i++)
            {
                if(callMethods.Call_One_Bool_Method(methodsList, i) == true)
                {
                    return true;
                }
            }
            return false;
            #endregion
        }



        public bool CheckIfMonetizationDisplayedOnScreen()
        {
            #region
            if(CanvasMainMenuManager.instance.currentSelectedPage == 4)
            {
                Debug.Log("Page: Do you want to buy this item");
                return true;
            }
            return false;
            #endregion
        }

        private void CheckBackButtonOtherPlayers()
        {
            #region
            if (!returnIfSpecialConditionAvailable() &&
                InfoPlayerTS.instance.returnCheckState(0) &&
                InputRemapperUI.instance.bIsRemapperEnabled)
            {
                Debug.Log("Submit Joystick back Pressed : ");
                int currentPage = CanvasMainMenuManager.instance.currentSelectedPage;
                StartCoroutine(CanvasMainMenuManager.instance.listMenu[currentPage].transform.parent.GetComponent<PageOut>().BackMenu());
            }
            #endregion
        }

    }

}
