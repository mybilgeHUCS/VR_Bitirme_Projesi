//Description: InputRemapperUI. This script is used to display inputs, bool, float that can be remap into the Input Menu Page
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TS.Generics
{
    public class InputRemapperUI : MonoBehaviour
    {
        public static InputRemapperUI   instance = null;

        public bool                     SeeInspector;
        public bool                     helpBox = true;


        public GameObject               objContent;
        public bool                     b_UpdateText;

        public GameObject objTxtFeedback;
        private CurrentText currentTextObjTxtFeedback;

        public List<GameObject>         refPosList = new List<GameObject>();
        public List<GameObject>         refInputTypeList = new List<GameObject>();
        public List<GameObject>         objStrokesPlayer = new List<GameObject>();
        public List<GameObject>         objStrokesDeviceType = new List<GameObject>();

        [HideInInspector]
        public int                      whichInputModeEditor;    // 0: Keyboard, 1: Gamepad, 2: Mobile, 3: Other
        [HideInInspector]
        public int                      whichInputTypeEditor;    // 0: Keycode, 1: String, 2: bool, 3: float
        [HideInInspector]
        public int                      whichInputToCreateEditor;// Number

        private List<Transform>         navigatIonList = new List<Transform>();

        public CurrentText              connectedGamepad;
        private int                     currentDeviceSelected;

        public bool                     bIsRemapperEnabled;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        //-> Page Initialization
        public bool InitPage()
        {
            currentTextObjTxtFeedback = objTxtFeedback.GetComponent<CurrentText>();
            currentTextObjTxtFeedback.NewTextWithSpecificID(2,0); //""

            SelectPlayer(InputRemapper.instance.currentSelectedPlayer);
            return true;    
        }

        //-> Select a player. Call by Btn_P1 and the Btn_P2 in the hierarchy
        public void SelectPlayer(int whichPlayer)
        {
            #region
            InputRemapper.instance.currentSelectedPlayer = whichPlayer;

            // Enable player stroke
            for (var i = 0;i< objStrokesPlayer.Count; i++)
            {
                if (whichPlayer == i)
                    objStrokesPlayer[i].SetActive(true);
                else
                    objStrokesPlayer[i].SetActive(false);
            }
            //Debug.Log("InputRemapper.instance.currentSelectedPlayer: " + InputRemapper.instance.currentSelectedPlayer);
            SelectKeyboardGamepadMobileOther(InfoInputs.instance.ListOfInputsForEachPlayer[InputRemapper.instance.currentSelectedPlayer].currentSelectedInputMode);
            #endregion
        }

        //-> Call when the player press a button that allows to remap an input
        public void RemapAnInput(btnRempInfo btnRempInfo)
        {
            #region
            InputRemapper.instance.currentSelectedButton        = btnRempInfo.gameObject;
            InputRemapper.instance.currentInputSelected         = btnRempInfo.whichInput;
            InputRemapper.instance.currentSelectedMustBeAButton = btnRempInfo.b_MustBeButton;

            if (!btnRempInfo.b_AllowRemap)
            {
                InfoText(5);                               // Display text info in the input remapper section "The Input can't be remap.";
                UpdateInputUIText();
            }
            else
            {
                //Debug.Log(" btnRempInfo.b_AllowsToHaveTheSameInputHasInput: " + btnRempInfo.b_AllowsToHaveTheSameInputHasInput + " : " + btnRempInfo.gameObject.name);
                StartCoroutine(IRemapAnInput(btnRempInfo.whichInput, btnRempInfo.b_AllowsToHaveTheSameInputHasInput, btnRempInfo.AllowsSameInputHasInput));
            }

            #endregion
        }

        //-> Routine to remap an input
        public IEnumerator IRemapAnInput(
            int whichInput,
            bool b_AllowsToHaveTheSameInputHasInput,
            int AllowsSameInputHasInput)
        {
            #region
            if (!InputRemapper.instance.IsRemapInProcess)
            {
                UpdateInputUIText();

                yield return new WaitUntil(() => b_UpdateText == false);

                btnRempInfo[] arr = objContent.GetComponentsInChildren<btnRempInfo>();

                Debug.Log("arr: " + arr.Length);

                foreach (btnRempInfo child in arr)
                {
                    if (child.whichInput == whichInput && (child.whichType == 0 || child.whichType == 1)) // It is a button to remap an input
                    {
                        //Debug.Log("whichInput: " + whichInput + " -> Child name: "  + child.gameObject.name);
                        child.transform.GetChild(1).GetComponent<CurrentText>().NewTextWithSpecificID(42,0); //"Choose a new Input"
                        break;
                    }
                }

                TS_EventSystem.instance.eventSystem.sendNavigationEvents = false;
                InputRemapper.instance.IsRemapInProcess = true;

                int currentSelectedInputMode = InfoInputs.instance.ListOfInputsForEachPlayer[InputRemapper.instance.currentSelectedPlayer].currentSelectedInputMode;
                if (currentSelectedInputMode == 0)  // Remap Keybaord
                    StartCoroutine(InputRemapper.instance.remapKeyboardInput());
                if (currentSelectedInputMode == 1)  // Remap Gamepad
                    StartCoroutine(InputRemapper.instance.remapGamepadInput(b_AllowsToHaveTheSameInputHasInput, AllowsSameInputHasInput));

            }
                yield return null;
            #endregion
        }


        //-> Update the inputs displayed on UI
        public void UpdateInputUIText()
        {
            #region
            b_UpdateText = true;
            btnRempInfo[] arr = objContent.GetComponentsInChildren<btnRempInfo>();

            foreach(btnRempInfo child in arr)
            {
                //-> Case Input
                if (child.transform.GetChild(1).GetComponent<Text>() || child.transform.GetChild(1).GetComponent<TextMeshProUGUI>())
                {
                    if (!child.transform.GetChild(1).GetComponent<CurrentText>())
                        child.transform.GetChild(1).gameObject.AddComponent<CurrentText>();

                    child.transform.GetChild(1).GetComponent<CurrentText>().NewTextManageByScript(new List<TextEntry>() { new TextEntry(child.returnInputUsed()) });

                }
                //-> Case Bool
                if (child.GetComponent<Slider>())
                    child.GetComponent<Slider>().value = child.returnInputFloat();
                //-> Case Float
                if (child.GetComponent<Toggle>())
                    child.GetComponent<Toggle>().isOn = child.returnInputBool();
            }
            b_UpdateText = false;
            #endregion
        }


        //-> Select a type of input for the current selected player. Call by the Btn_Keyboard and Btn_Gamepad in the Hierarchy
        public void SelectKeyboardGamepadMobileOther(int whichDeviceType)
        {
            //Debug.Log("SelectKeyboardGamepadMobileOther: " + whichDeviceType);
            StartCoroutine(SelectControllerRoutine(whichDeviceType));
            currentDeviceSelected = whichDeviceType;
        }


        //-> Routine to select a type of input for the current selected player
        IEnumerator SelectControllerRoutine(int whichDeviceType)
        {
            #region
            // Enable Device type stroke
            for (var i = 0; i < objStrokesDeviceType.Count; i++)
            {
                if (whichDeviceType == i)
                    objStrokesDeviceType[i].SetActive(true);
                else
                    objStrokesDeviceType[i].SetActive(false);
            }

            btnRempInfo[] arr = objContent.GetComponentsInChildren<btnRempInfo>(true);
            InfoInputs.instance.ListOfInputsForEachPlayer[InputRemapper.instance.currentSelectedPlayer].currentSelectedInputMode = whichDeviceType;
           
            foreach (btnRempInfo child in arr)
            {
                if (child.whichDevice == whichDeviceType) // 0: Keyboard, 1: Gamepad, 2: Mobile, 3: Other
                    child.transform.parent.parent.gameObject.SetActive(true);
                else
                    child.transform.parent.parent.gameObject.SetActive(false);
            }

            UpdateInputUIText();

            yield return new WaitUntil(() => !b_UpdateText);

            updateInputUINavigation();
            yield return null;
            #endregion
        }



        //-> Update UI navigation (button, toggle, slider) when the player change input type
        public void updateInputUINavigation()
        {
            #region
            navigatIonList.Clear();
            Transform[] arr = objContent.GetComponentsInChildren<Transform>();
            //InputRemapper.instance.currentRemapperSelectedInputTYpe = whichDeviceType;
            //btnRempInfo
            foreach (Transform child in arr)
            {
                if (child.GetComponent<Button>() || child.GetComponent<Toggle>() || child.GetComponent<Slider>())
                    navigatIonList.Add(child);
            }


            //-> Connect selectOnDown
            for(var i = 0;i< navigatIonList.Count - 1; i++)
            {
                // get the Navigation data
                Navigation navigation = new Navigation();
                if (navigatIonList[i].GetComponent<Button>()) navigation = navigatIonList[i].GetComponent<Button>().navigation;
                if (navigatIonList[i].GetComponent<Toggle>()) navigation = navigatIonList[i].GetComponent<Toggle>().navigation;
                if (navigatIonList[i].GetComponent<Slider>()) navigation = navigatIonList[i].GetComponent<Slider>().navigation;

                // switch mode to Explicit to allow for custom assigned behavior
                navigation.mode = Navigation.Mode.Explicit;

                //->
                if (navigatIonList[i+1].GetComponent<Button>()) navigation.selectOnDown = navigatIonList[i+1].GetComponent<Button>();
                if (navigatIonList[i+1].GetComponent<Toggle>()) navigation.selectOnDown = navigatIonList[i+1].GetComponent<Toggle>();
                if (navigatIonList[i+1].GetComponent<Slider>()) navigation.selectOnDown = navigatIonList[i+1].GetComponent<Slider>();

                // reassign the struct data to the button
                if (navigatIonList[i].GetComponent<Button>()) navigatIonList[i].GetComponent<Button>().navigation = navigation;
                if (navigatIonList[i].GetComponent<Toggle>()) navigatIonList[i].GetComponent<Toggle>().navigation = navigation;
                if (navigatIonList[i].GetComponent<Slider>()) navigatIonList[i].GetComponent<Slider>().navigation = navigation;
            }

            //-> Connect selectOnUp
            for (var i = navigatIonList.Count - 1; i > 0; i--)
            {
                // get the Navigation data
                Navigation navigation = new Navigation();
                if (navigatIonList[i].GetComponent<Button>()) navigation = navigatIonList[i].GetComponent<Button>().navigation;
                if (navigatIonList[i].GetComponent<Toggle>()) navigation = navigatIonList[i].GetComponent<Toggle>().navigation;
                if (navigatIonList[i].GetComponent<Slider>()) navigation = navigatIonList[i].GetComponent<Slider>().navigation;

                // switch mode to Explicit to allow for custom assigned behavior
                navigation.mode = Navigation.Mode.Explicit;

                //->
                if (navigatIonList[i - 1].GetComponent<Button>()) navigation.selectOnUp = navigatIonList[i - 1].GetComponent<Button>();
                if (navigatIonList[i - 1].GetComponent<Toggle>()) navigation.selectOnUp = navigatIonList[i - 1].GetComponent<Toggle>();
                if (navigatIonList[i - 1].GetComponent<Slider>()) navigation.selectOnUp = navigatIonList[i - 1].GetComponent<Slider>();

                // reassign the struct data to the button
                if (navigatIonList[i].GetComponent<Button>()) navigatIonList[i].GetComponent<Button>().navigation = navigation;
                if (navigatIonList[i].GetComponent<Toggle>()) navigatIonList[i].GetComponent<Toggle>().navigation = navigation;
                if (navigatIonList[i].GetComponent<Slider>()) navigatIonList[i].GetComponent<Slider>().navigation = navigation;
            }


            if(navigatIonList.Count > 0)
            { //-> Connect UP the first input in the list to Keyboard or gamepad button depending the selected input Type
                int currentSelectedInputMode = InfoInputs.instance.ListOfInputsForEachPlayer[InputRemapper.instance.currentSelectedPlayer].currentSelectedInputMode;

                // get the Navigation data
                Navigation navigation2 = new Navigation();
                if (navigatIonList[0].GetComponent<Button>()) navigation2 = navigatIonList[0].GetComponent<Button>().navigation;
                if (navigatIonList[0].GetComponent<Toggle>()) navigation2 = navigatIonList[0].GetComponent<Toggle>().navigation;
                if (navigatIonList[0].GetComponent<Slider>()) navigation2 = navigatIonList[0].GetComponent<Slider>().navigation;

                // switch mode to Explicit to allow for custom assigned behavior
                navigation2.mode = Navigation.Mode.Explicit;

                //->
                if (currentSelectedInputMode == 0) navigation2.selectOnUp = refPosList[6].GetComponent<Button>();
                if (currentSelectedInputMode == 1) navigation2.selectOnUp = refPosList[7].GetComponent<Button>();

                // reassign the struct data to the button
                if (navigatIonList[0].GetComponent<Button>()) navigatIonList[0].GetComponent<Button>().navigation = navigation2;
                if (navigatIonList[0].GetComponent<Toggle>()) navigatIonList[0].GetComponent<Toggle>().navigation = navigation2;
                if (navigatIonList[0].GetComponent<Slider>()) navigatIonList[0].GetComponent<Slider>().navigation = navigation2;



                //-> Connect Down Keyboard button
                // get the Navigation data
                Navigation navigation3 = new Navigation();
                if (currentSelectedInputMode == 0) navigation3 = refPosList[6].GetComponent<Button>().navigation;

                // switch mode to Explicit to allow for custom assigned behavior
                navigation3.mode = Navigation.Mode.Explicit;

                //->
                if (navigatIonList[0].GetComponent<Button>()) navigation3.selectOnDown = navigatIonList[0].GetComponent<Button>();
                if (navigatIonList[0].GetComponent<Toggle>()) navigation3.selectOnDown = navigatIonList[0].GetComponent<Toggle>();
                if (navigatIonList[0].GetComponent<Slider>()) navigation3.selectOnDown = navigatIonList[0].GetComponent<Slider>();


                navigation3.selectOnUp = refPosList[8].GetComponent<Button>();
                navigation3.selectOnRight = refPosList[7].GetComponent<Button>();

                // reassign the struct data to the button
                refPosList[6].GetComponent<Button>().navigation = navigation3;


                //-> Connect Down Gamepad button
                // get the Navigation data
                if (currentSelectedInputMode == 0) navigation3 = refPosList[7].GetComponent<Button>().navigation;

                // switch mode to Explicit to allow for custom assigned behavior
                navigation3.mode = Navigation.Mode.Explicit;

                //->
                if (navigatIonList[0].GetComponent<Button>()) navigation3.selectOnDown = navigatIonList[0].GetComponent<Button>();
                if (navigatIonList[0].GetComponent<Toggle>()) navigation3.selectOnDown = navigatIonList[0].GetComponent<Toggle>();
                if (navigatIonList[0].GetComponent<Slider>()) navigation3.selectOnDown = navigatIonList[0].GetComponent<Slider>();


                navigation3.selectOnUp = refPosList[9].GetComponent<Button>();
                navigation3.selectOnLeft = refPosList[6].GetComponent<Button>();

                // reassign the struct data to the button
                refPosList[7].GetComponent<Button>().navigation = navigation3;
            }
            #endregion
        }

        //-> Call by the inputRemapper script  if an input can't be replace the current one
        public void InfoText(int value,int value02 = 0) {
            #region
            StopCoroutine(InputFeedback());
            StartCoroutine(InputFeedback(value, value02));
            #endregion
        }


        //-> Feedback displayed on UI if an input can't be replace the current one
        public IEnumerator InputFeedback(int value = 0, int value02 = 0)
        {
            #region
            if (value == 0) currentTextObjTxtFeedback.NewTextWithSpecificID(38, 0); //"You must use a button.";
            if (value == 1) currentTextObjTxtFeedback.NewTextWithSpecificID(39, 0);//"The input is already used.";
            if (value == 2) currentTextObjTxtFeedback.NewTextWithSpecificID(40, 0); //"You must use keyboard input.";
            if (value == 3) currentTextObjTxtFeedback.NewTextWithSpecificID(41, 0); //"Canceled.";

            if (value == 4) currentTextObjTxtFeedback.NewTextManageByScript(new List<TextEntry>() { new TextEntry(0, 62), new TextEntry(value02.ToString()) }); //"You must use gamepad: 1 2 ....";

            if (value == 5) currentTextObjTxtFeedback.NewTextWithSpecificID(64, 0); //"The input can't be remap.";

            float t = 0;
            float duration = 2;

            while(t < duration)
            {
                t = Mathf.MoveTowards(t, duration, Time.deltaTime);
                yield return null;
            }

            //txtFeedback.text = "";
            currentTextObjTxtFeedback.NewTextWithSpecificID(2, 0); //"";
            yield return null;
            #endregion
        }

        public void ExitInputsMenu()
        {
            InfoInputs.instance.Bool_SaveAllInputs();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }



        private void Update()
        {
            displayConnectedGamepad(InputRemapper.instance.currentSelectedPlayer);
        }

        public void displayConnectedGamepad(int Player)
        {
            if (currentDeviceSelected == 1)
            {
                if (Input.GetJoystickNames().Length > Player)
                {
                    connectedGamepad.NewTextManageByScript(new List<TextEntry>() { new TextEntry(0, 174), new TextEntry(Input.GetJoystickNames()[Player]) });
                   
                }
                else
                {
                    connectedGamepad.NewTextWithSpecificID(177, 0);

                }
            }
            else
            {
                connectedGamepad.NewTextWithSpecificID(2, 0);
            }
            
        }

        public void IsRemapperEnabled(bool state)
        {
            bIsRemapperEnabled = state;
        }

        public void ResetInput()
        {
            Debug.Log("Reset Inputs");
            InfoInputs.instance.ResetAllInputs();
            UpdateInputUIText();
        }
    }
}


