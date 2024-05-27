// Description: btnRempInfo: Attached to buttons in Input Remapper Menu.
using UnityEngine;
using UnityEngine.UI;

namespace TS.Generics
{
    public class btnRempInfo : MonoBehaviour
    {
        public bool SeeInspector;
        public bool moreOptions;
        public bool helpBox = true;

        //[Header("0: Keyboard, 1: Gamepad, 2: Mobile, 3: Other")]
        public int whichDevice; //
        //[Header("0: Keycode, 1: String, 2: bool, 3: float")]
        public int whichType;   //
        //[Header("input number")]
        public int whichInput;  // 
        //[Header("which bool")]
        public int whichBool;   // 
        //[Header("which float")]
        public int whichFloat;  // 


        public bool b_MustBeButton;
        public bool b_AllowRemap = true;

        //[Header ("Axis Case")]
        public bool b_AllowsToHaveTheSameInputHasInput = false;
        public int  AllowsSameInputHasInput = 0;

        //-> Use into section Inputs Options to init the input name (Runtime);
        public string returnInputUsed()
        {
            int currentPlayer = InputRemapper.instance.currentSelectedPlayer;
            // Keycode
            if (whichType == 0) 
            {
                //Debug.Log("Name: " + InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfButtons[whichInput]._Keycode.ToString());
                return InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfButtons[whichInput]._Keycode.ToString();
            }
            // String
            if (whichType == 1)
            { 
                //Debug.Log("name: " + InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfButtons[whichInput]._AxisName.ToString());
                return InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfButtons[whichInput]._AxisName.ToString();
            }

            return "";
        }

        //-> Use into section Inputs Options to init a boolean parameter (Runtime);
        public bool returnInputBool()
        {
            int currentPlayer = InputRemapper.instance.currentSelectedPlayer;
            //Debug.Log("currentPlayer: " + currentPlayer + " :whichBool: " + whichBool);
            return InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfBool[whichBool].b_State;
        }

        //-> Use into section Inputs Options to init a float parameter (Runtime);
        public float returnInputFloat()
        {
            int currentPlayer = InputRemapper.instance.currentSelectedPlayer;

            return InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfFloat[whichFloat]._Value;
        }

        //-> Use into section Inputs Options to update an Input parameter (Runtime UI Button);
        public void remapButton()
        {
            InputRemapperUI.instance.RemapAnInput(GetComponent<btnRempInfo>());
        }

        //-> Use into section Inputs Options to update a boolean parameter (Runtime UI Toggle);
        public void updateBoolValue()
        {
            int currentPlayer = InputRemapper.instance.currentSelectedPlayer;
            InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfBool[whichBool].b_State = GetComponent<Toggle>().isOn;
        }

        //-> Use into section Inputs Options to init a float parameter (Runtime UI Slider);
        public void updateFloatValue()
        {
            int currentPlayer = InputRemapper.instance.currentSelectedPlayer;
            InfoInputs.instance.ListOfInputsForEachPlayer[currentPlayer].listOfFloat[whichFloat]._Value = GetComponent<Slider>().value;
        }
    }

}
