// Descition: InputRemapper. Use in association with InfoInputs and InfoRemapperUI during the input remap process.
// The scrpit contains methods that check if an input can be remapped with the new input selected by a player. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class InputRemapper : MonoBehaviour
    {
        public static InputRemapper     instance = null;                        // Only one instance of the object in the scene.

        private List<KeyCode>           ListOfKeycodeAlreadyUse = new List<KeyCode>();  // Use to create a list of inputs already used by players
        [HideInInspector]
        public int                      currentSelectedPlayer;                  // Know which player is being remapped
        [HideInInspector]
        public bool                     currentSelectedMustBeAButton;           // Use to check if the input to remap must be a button (not an axis)
        [HideInInspector]
        public int                      currentInputSelected;                   // Know which input to remap is selected
        [HideInInspector]
        public bool                     IsRemapInProcess;                       // Check if a button is being remapped
        [HideInInspector]
        public GameObject               currentSelectedButton;                  // Ref to the button selected in the Input rempa section
       
        [Header ("Max number of gamepad allowed:")]
        public int                      maxNumberOfGamepad = 2;                 // Max number of gamepad use in the game. If > 2 inputs must be create in ProjectSettings -> Inputs for each new gamepad
        [Header("Back Button ID in InfoInputs:")]
        public int                      backButtonID;                           // Reference to the back Button ID (InfoInputs)

        private bool                    b_WaitForInputs = false;                // Use to check when player press an input during the remap process

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

        //-> Routine to remap a keyboard input.
        public IEnumerator remapKeyboardInput()
        {
            #region
            Input.ResetInputAxes();															// Reset Axis values
            b_WaitForInputs = true;
            // Wait until the player press a button
            while (b_WaitForInputs)
            {
                //-> Player press a key
                if (returnKeyboardKeyCode() != KeyCode.None)
                {
                    // Check if the player cancels mapping process
                    if (IsRemapInProcess && currentSelectedButton != TS_EventSystem.instance.eventSystem.currentSelectedGameObject ||
                        Input.GetKeyDown(KeyCode.Mouse0) ||
                        Input.GetKeyDown(KeyCode.Mouse1) ||
                        Input.GetKeyDown(KeyCode.Mouse2) ||
                        Input.GetKeyDown(KeyCode.Mouse3) ||
                        Input.GetKeyDown(KeyCode.Mouse4) ||
                        Input.GetKeyDown(KeyCode.Mouse5) ||
                        Input.GetKeyDown(KeyCode.Mouse6))
                    {
                        InputRemapperUI.instance.InfoText(3);                               // Display text info in the input remapper section "Canceled.";
                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;
                        //Debug.Log("Pressed Axis: " + returnGamepadAxis());
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> Check if the key is joystick 
                    else if (returnKeyboardKeyCode().ToString().Contains("oyst"))
                    {
                        //Debug.Log("Joystick Input not allowed");
                        InputRemapperUI.instance.InfoText(2);                               // Display text info in the input remapper section "You must use keyboard input.";
                        InputRemapperUI.instance.UpdateInputUIText();                       // 
                        IsRemapInProcess = false;
                        //Debug.Log("Pressed Axis: " + returnGamepadAxis());
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;

                    }
                    //-> Check if the key is already used by a player
                    else if (returnIfNewKeyboardKeyCodeIsAlreadyUsed(returnKeyboardKeyCode()))
                    {
                        //Debug.Log("New KeyCode: Already Use");
                        InputRemapperUI.instance.InfoText(1);                               // Display text info in the input remapper section "The input is already used.";
                        InputRemapperUI.instance.UpdateInputUIText();                       // 
                        IsRemapInProcess = false;
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> This KeyCode can be use to replace the current input
                    else
                    {
                        StartCoroutine(UpdateKeyboardKeyCode(
                            currentSelectedPlayer,
                            currentInputSelected,
                            returnKeyboardKeyCode()));

                        //Debug.Log("New KeyCode: " + returnKeyboardKeyCode());
                        InputRemapperUI.instance.UpdateInputUIText();                         
                        IsRemapInProcess = false;
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;

                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                    }
                    Input.ResetInputAxes();
                }
                yield return null;
            }
            yield return null;
            #endregion
        }


        // Routine to remap a Gamepad input.
        public IEnumerator remapGamepadInput(
            bool b_AllowsToHaveTheSameInputHasInput,
            int AllowsSameInputHasInput)
        {
            #region
            Input.ResetInputAxes();																				// Reset Axis values
            b_WaitForInputs = true;
            // Wait until the player press a button or an axis
            while (b_WaitForInputs)
            {
                //-> Player press a key
                // Check if the player cancels mapping process
                if (IsRemapInProcess && currentSelectedButton != TS_EventSystem.instance.eventSystem.currentSelectedGameObject
                    || InfoInputs.instance.checkSpecificInputAllPlayers(backButtonID))
                {
                    //Debug.Log("Canceled");
                    InputRemapperUI.instance.InfoText(3);                                   // Display text info in the input remapper section "Canceled.";
                    InputRemapperUI.instance.UpdateInputUIText();
                    IsRemapInProcess = false;
                    //Debug.Log("Pressed Axis: " + returnGamepadAxis());
                    Input.ResetInputAxes();
                    yield return new WaitForSeconds(.2f);                                   // Prevents to change the selected button after the input remap
                    TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                    b_WaitForInputs = false;
                    yield return new WaitForEndOfFrame();
                }
                //-> Player press a key
                else if (returnGamepadAxis(currentSelectedPlayer) != "")
                {
                    string InputName = returnGamepadAxis(currentSelectedPlayer);
                    Debug.Log("InputName: " + InputName + " ->  " + currentSelectedPlayer);
                    //-> Check if the gamepad is allowed for the player (P1 must use gamepad 1, P2 must gamepad 2...)
                    if (!returnGamepadAxis(currentSelectedPlayer).ToString().Contains("oystick" + (currentSelectedPlayer+1).ToString()))
                    {
                        //Debug.Log("Wrong gamepad");
                        int tmpCurrentPlayer = currentSelectedPlayer + 1;
                        InputRemapperUI.instance.InfoText(4, tmpCurrentPlayer);                               // Display text info in the input remapper section "You must use a button.";
                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;

                        //Debug.Log("Pressed Axis: " + returnGamepadAxis());
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> Check if the button must be a button
                    else if (!returnGamepadAxis(currentSelectedPlayer).ToString().Contains("utton") &&
                        currentSelectedMustBeAButton)
                    {
                        //Debug.Log("Must be a button");
                        InputRemapperUI.instance.InfoText(0);                               // Display text info in the input remapper section "You must use a button.";
                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;

                        //Debug.Log("Pressed Axis: " + returnGamepadAxis());
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> Check if the Axis is already used by a player
                    else if (returnIfNewGamepadAxisIsAlreadyUsed(returnGamepadAxis(currentSelectedPlayer)) &&
                        !b_AllowsToHaveTheSameInputHasInput)    // This axis is only use for one button. 2 buttons can't use the same input.
                    {
                        //Debug.Log("New Axis: Already Use");
                        InputRemapperUI.instance.InfoText(1);                               // Display text info in the input remapper section "The input is already used.";
                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;

                        //Debug.Log("Pressed Axis: " + returnGamepadAxis());
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> Check if the Axis is already used by a player and can be used because it is paring to an another Axis (Example case: Left/Right Up/Down)
                    else if (returnIfNewGamepadAxisIsAlreadyUsed(returnGamepadAxis(currentSelectedPlayer)) &&
                        returnParedInputs(
                            returnGamepadAxis(currentSelectedPlayer),
                            currentSelectedPlayer,
                            b_AllowsToHaveTheSameInputHasInput,
                            AllowsSameInputHasInput))                                       // This axis is only use for one button. 2 buttons can't use the same input.
                    {
                        StartCoroutine(UpdateGamepadAxis(
                            currentSelectedPlayer,
                            currentInputSelected,
                            returnGamepadAxis(currentSelectedPlayer)));
                        //Debug.Log("Pressed Axis: " + returnGamepadAxis(currentSelectedPlayer));
                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                           // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> Check if the Axis is already used by a player and can be used because it is paring to an another Axis (case Left/Right Up/Down)
                    else if (returnIfNewGamepadAxisIsAlreadyUsed(returnGamepadAxis(currentSelectedPlayer)) &&
                        !returnParedInputs(
                            returnGamepadAxis(currentSelectedPlayer),
                            currentSelectedPlayer,
                            b_AllowsToHaveTheSameInputHasInput,
                            AllowsSameInputHasInput))    // This axis is only use for one button. 2 buttons can't use the same input.
                    {
                        //Debug.Log("Pairing impossible");
                        InputRemapperUI.instance.InfoText(1);                               // Display text info in the input remapper section "The input is already used.";
                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                               // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                    //-> This Axis can be use to replace the current input
                    else
                    {
                        StartCoroutine(UpdateGamepadAxis(
                            currentSelectedPlayer,
                            currentInputSelected,
                            returnGamepadAxis(currentSelectedPlayer)));

                        InputRemapperUI.instance.UpdateInputUIText();
                        IsRemapInProcess = false;
                        //Debug.Log("Pressed Axis: " + returnGamepadAxis(currentSelectedPlayer));
                        Input.ResetInputAxes();
                        yield return new WaitForSeconds(.2f);                           // Prevents to change the selected button after the input remap
                        TS_EventSystem.instance.eventSystem.sendNavigationEvents = true;
                        b_WaitForInputs = false;
                    }
                }
                yield return null;
            }
            yield return null;
            #endregion
        }


        //-> Return the Input (keycode) pressed by the player 
        KeyCode returnKeyboardKeyCode()
        {
            #region
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            { if (Input.GetKeyDown(key)) { return key; } }
            return KeyCode.None;
            #endregion
        }

        //-> Return if a keycode is already used by a player 
        bool returnIfNewKeyboardKeyCodeIsAlreadyUsed(KeyCode TestedKeyCode)
        {
            #region
            for(var i = 0;i < listKeyCodeAlreadyUse().Count; i++)
            {
                if (TestedKeyCode == listKeyCodeAlreadyUse()[i])
                    return true;
            }
            return false;
            #endregion
        }

        //-> Return the gamepad input (button/Axis) pressed by the player
        string returnGamepadAxis(int whichPlayer)
        {
            #region
            //Debug.Log("Input.GetJoystickNames().Length: " + Input.GetJoystickNames().Length);
            for (var k = 0; k < Input.GetJoystickNames().Length; k++)
            {
                // Prevent bug if more than 2 gamepads are connected.Limit to 2 gamepad.
                if (k < maxNumberOfGamepad)
                {
                    // Check joystick axis
                    for (var i = 0; i < 10; i++)
                    {
                        if (Mathf.Abs(Input.GetAxisRaw("Joystick" + (k + 1).ToString() + "Axis" + (i + 1).ToString())) == 1)
                            return "Joystick" + (k + 1).ToString() + "Axis" + (i + 1).ToString();
                    }

                    // Check Joystick Button
                    for (var i = 0; i < 20; i++)
                    {
                        if (Mathf.Abs(Input.GetAxisRaw("Joystick" + (k + 1).ToString() + "Button" + (i).ToString())) == 1)
                        {
                            //Debug.Log("k: " + k + " | " + "Joystick" + (k + 1).ToString() + "Button" + (i).ToString());
                            return "Joystick" + (k + 1).ToString() + "Button" + (i).ToString();
                        }
                           
                    }
                }
            }
            return "";
            #endregion
        }

        float ReturnTheInputValue(string inputName)
        {
            #region
            return Input.GetAxisRaw(inputName);
            #endregion
        }

        //-> Return if a gamepad axis is already used by a player
        bool returnIfNewGamepadAxisIsAlreadyUsed(string TestedAxis)
        {
            #region
            for (var i = 0; i < listAxisAlreadyUse().Count; i++)
            {
                if (TestedAxis == listAxisAlreadyUse()[i])
                    return true;
            }

            return false;
            #endregion
        }

        //-> Check if an Input is paired with an other input (Gamepad Only)
        bool returnParedInputs(
            string selectedInputName,
            int currentSelectedPlayer,
            bool b_AllowsToHaveTheSameInputHasInput,
            int AllowsSameInputHasInput)
        {
            #region
            string pairedAxis = InfoInputs.instance.ListOfInputsForEachPlayer[currentSelectedPlayer].listOfButtons[AllowsSameInputHasInput]._AxisName;
            //Debug.Log("Check Paired Input:" + pairedAxis + " : " + selectedInputName);
            if (pairedAxis == selectedInputName)
                return true;
           
            return  false;
            #endregion
        }

        //-> return a list of already used Axis used by players
        public List<string> listAxisAlreadyUse()
        {
            #region
            List<string> sAlreadyUse = new List<string>();
            for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
            {
                for (var k = 0; k < InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons.Count; k++)
                {
                    string newAxis = InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[k]._AxisName;
                    sAlreadyUse.Add(newAxis);
                }
            }
            return sAlreadyUse;
            #endregion
        }

        // Return a list of inputs already used by players
        List<KeyCode> listKeyCodeAlreadyUse()
        {
            #region
            ListOfKeycodeAlreadyUse.Clear();
            for (var i = 0; i < InfoInputs.instance.ListOfInputsForEachPlayer.Count; i++)
            {
                for (var k = 0; k < InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons.Count; k++)
                {
                    KeyCode newKey = InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[k]._Keycode;
                    ListOfKeycodeAlreadyUse.Add(newKey);
                }
            }
            return ListOfKeycodeAlreadyUse;
            #endregion
        }

        //-> Update Keyboard input. Use in InputRemapper
        public IEnumerator UpdateKeyboardKeyCode(int _Player, int _InputNumber, KeyCode _NewKeycode)
        {
            InfoInputs.instance.ListOfInputsForEachPlayer[_Player].listOfButtons[_InputNumber]._Keycode = _NewKeycode;
            yield return null;
        }

        //-> Update Gamepad input. Use in InputRemapper
        public IEnumerator UpdateGamepadAxis(int _Player, int _InputNumber, string _NewAxis)
        {
            // Save the name of the new input
            InfoInputs.instance.ListOfInputsForEachPlayer[_Player].listOfButtons[_InputNumber]._AxisName = _NewAxis;

            // Save the input value when the input is pressed. If positive 1. If negative -1.
            if(ReturnTheInputValue(_NewAxis) > 0)
                InfoInputs.instance.ListOfInputsForEachPlayer[_Player].listOfButtons[_InputNumber]._AxisPositiveOrNegative = 1;
            else
                InfoInputs.instance.ListOfInputsForEachPlayer[_Player].listOfButtons[_InputNumber]._AxisPositiveOrNegative = -1;
            yield return null;
        }

        public void ExitRemapperSaveInput()
        {
            while (!InfoInputs.instance.Bool_SaveAllInputs())
            {
                Debug.Log("Wait");
            }
           
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void EventSytemAllowAllPlayersUsingAxisNavigation()
        {
            TS_EventSystem.instance.standaloneInputModule.horizontalAxis = "TSHorizontalRemap";
            TS_EventSystem.instance.standaloneInputModule.verticalAxis = "TSVerticalRemap";
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void EventSytemAllowOnlyPlayerOneUsingAxisNavigation()
        {
            TS_EventSystem.instance.standaloneInputModule.horizontalAxis = "TSHorizontal";
            TS_EventSystem.instance.standaloneInputModule.verticalAxis = "TSVertical";
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }
}
