// Description : buttonValidation : Used To invoke onClick() when the player press a UI button
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace TS.Generics
{
    public class buttonValidation : MonoBehaviour
    {

        public bool         SeeInspector = false;
        private EventSystem eventSystem;
        public int          validationButtonID = 1;
        public bool         b_KeyCodeLastState = false;

        // Use this for initialization
        void Start()
        {
            eventSystem = gameObject.GetComponent<EventSystem>();

            for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
            {
                if(i == 0)
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[validationButtonID].OnGetKeyDownReceived += CheckValidationButtonPlayerOne;
                else
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[validationButtonID].OnGetKeyDownReceived += CheckValidationButtonOtherPlayers;
                //Debug.Log("-> " + InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[validationButtonID]._Names);
            }
               
        }
       
        public void OnDestroy()
        {
            for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
            {
                if (i == 0)
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[validationButtonID].OnGetKeyDownReceived -= CheckValidationButtonPlayerOne;
                else
                    InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[validationButtonID].OnGetKeyDownReceived -= CheckValidationButtonOtherPlayers;
                //Debug.Log("-> " + InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[validationButtonID]._Names);
            }

            //Debug.Log("Destroy Validation Button Manager");
        }
       


        private void CheckValidationButtonPlayerOne()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0))
            {
               
                if (eventSystem.currentSelectedGameObject != null &&
                eventSystem.currentSelectedGameObject.GetComponent<Button>() &&
                 eventSystem.currentSelectedGameObject.activeInHierarchy)
                {
                    //Debug.Log("Validation Button Pressed : " + eventSystem.currentSelectedGameObject.name);
                    eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
                }
                else if (eventSystem.currentSelectedGameObject != null &&
                eventSystem.currentSelectedGameObject.GetComponent<Toggle>() &&
                 eventSystem.currentSelectedGameObject.activeInHierarchy)
                {
                    //Debug.Log("Toggle Pressed : " + eventSystem.currentSelectedGameObject.name);
                    if (eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn = false;
                    else
                        eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
                }
            }
            #endregion
        }

        private void CheckValidationButtonOtherPlayers()
        {
            #region
            if (InfoPlayerTS.instance.returnCheckState(0) &&
                InputRemapperUI.instance.bIsRemapperEnabled)
            {

                if (eventSystem.currentSelectedGameObject != null &&
                eventSystem.currentSelectedGameObject.GetComponent<Button>())
                {
                    //Debug.Log("Validation Button Pressed : " + eventSystem.currentSelectedGameObject.name);
                    eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
                }
                else if (eventSystem.currentSelectedGameObject != null &&
                eventSystem.currentSelectedGameObject.GetComponent<Toggle>())
                {
                    //Debug.Log("Toggle Pressed : " + eventSystem.currentSelectedGameObject.name);
                    if (eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn)
                        eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn = false;
                    else
                        eventSystem.currentSelectedGameObject.GetComponent<Toggle>().isOn = true;
                }
            }
            #endregion
        }
    }

}
