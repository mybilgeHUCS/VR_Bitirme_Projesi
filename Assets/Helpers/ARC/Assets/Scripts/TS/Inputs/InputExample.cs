using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class InputExample : MonoBehaviour
    {
        void Start()
        {
            //-> Player 1 | Input Left (ID = 3)

            #region Delegate
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyReceived          += OnGetKeyReceived;
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyDownReceived      += OnGetKeyDownReceived;
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyUpReceived        += OnGetKeyUpReceived;
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyReleasedReceived  += OnGetKeyReleasedReceived;
            #endregion

            #region Parameters
            // Access the name of the input in Unity Editor
            string name = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3]._Names;

            // Access the sprite associated with the Input
            Image image = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3]._Image;

            // Access KeyCode associated with the Input
            KeyCode keyCode = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3]._Keycode;

            // Access Axis name. The name corresponds to an Input in Project Settings -> Input Manager.
            string axisName = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3]._AxisName;

            // If true: Enable the next parameters
            bool bAxisDirection = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].bUseAxisDirection;

            // Know if an input is pressed or not.
            // Input is pressed if _AxisPositiveOrNegative = -1 && _AxisCurrentValue = -1
            // or if _AxisPositiveOrNegative = 1 && _AxisCurrentValue = 1
            int dir = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3]._AxisPositiveOrNegative;

            // Retrun if the button is pressed. Button pressed if bGetKeyDown = 1.
            bool bGetKeyDown = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].b_GetKeyDown;

            // Use to determine if an axis is pressed when axis value is negative or positive (-1 or 1)
            float axisValue = InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3]._AxisCurrentValue;
            #endregion
        }

        private void OnDestroy()
        {
            #region Delegate
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyReceived          -= OnGetKeyReceived;
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyDownReceived      -= OnGetKeyDownReceived;
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyUpReceived        -= OnGetKeyUpReceived;
            InfoInputs.instance.ListOfInputsForEachPlayer[0].listOfButtons[3].OnGetKeyReleasedReceived  -= OnGetKeyReleasedReceived;
            #endregion
        }

        public void OnGetKeyReceived()
        {
            // Do something when the key is pressed and until the key is released
            Debug.Log("Left: OnGetKeyReceived");
        }

        public void OnGetKeyDownReceived()
        {
            // Do something when the key is pressed down.
            Debug.Log("Left: OnGetKeyDownReceived");
        }

        public void OnGetKeyUpReceived()
        {
            // Do something when the key is pressed up.
            Debug.Log("Left: OnGetKeyUpReceived");
        }

        public void OnGetKeyReleasedReceived()
        {
            // Do something when the key is not pressed.
            Debug.Log("Left: OnGetKeyReleasedReceived");
        }
    }
}

