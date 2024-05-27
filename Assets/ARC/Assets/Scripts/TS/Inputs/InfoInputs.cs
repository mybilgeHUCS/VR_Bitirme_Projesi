// Description: InfoInputs. Contains Methods to load, save inputs, float, bool for each player.
// Contains methods to know if players use inputs
// Set up players inputs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace TS.Generics
{
    public class InfoInputs : MonoBehaviour
    {
        public static InfoInputs        instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.

        public bool                     SeeInspector;
        public int                      editorCurrentSelectedPlayer;
        public int                      editorCurrentInputType;

        public bool                     moreOptions;
        public bool                     helpBox = false;

       
        [System.Serializable]
        public class C_MethodLists
        {
            public List<EditorMethodsList_Pc.MethodsList> methodsList   // Create a list of Custom Methods that could be edit in the Inspector
                = new List<EditorMethodsList_Pc.MethodsList>();
        }
        public CallMethods_Pc           callMethods;                    // Access script taht allow to call public function in this script.


        [System.Serializable]
        public class ButtonKey
        {
            public string   _Names = "";
            public Image   _Image;
            public KeyCode  _Keycode;
            public string   _AxisName;
            public bool     bUseAxisDirection = true;                           // If true: Enable the next parameters
            public int      _AxisPositiveOrNegative = 1;                        // Reference use to know if an input is pressed or not. Input is pressed if _AxisPositiveOrNegative = -1 && _AxisCurrentValue = -1 | or if _AxisPositiveOrNegative = 1 && _AxisCurrentValue = 1
            public List<EditorMethodsList_Pc.MethodsList> methodsListMobile       // Create a list of Custom Methods that could be edit in the Inspector
               = new List<EditorMethodsList_Pc.MethodsList>();
            public List<EditorMethodsList_Pc.MethodsList> methodsListOther       // Create a list of Custom Methods that could be edit in the Inspector
               = new List<EditorMethodsList_Pc.MethodsList>();
            //public bool     IsPressed;
            public bool     b_GetKeyDown;                                       // Check GetKeyDown state
           
            public float    _AxisCurrentValue;                                  // Use to determine if an axis is pressed when axis value is negative or positive (-1 or 1)
            public Action   OnGetKeyReceived;                                   // Use to call methods depending the input state (Key Down, pressed, up, released)
            public Action   OnGetKeyDownReceived;
            public Action   OnGetKeyUpReceived;
            public Action   OnGetKeyReleasedReceived;
        }


        [System.Serializable]
        public class BoolPlayer
        {
            public string   _Name;
            public  bool    b_State;
        }

        [System.Serializable]
        public class floatPlayer
        {
            public string   _Name;
            public float    _Value;
        }

        [System.Serializable]
        public class ListOfInputs
        {
            public int                  currentSelectedInputMode = 0;
            public List<ButtonKey>      listOfButtons = new List<ButtonKey>();
            public List<BoolPlayer>     listOfBool = new List<BoolPlayer>();
            public List<floatPlayer>    listOfFloat = new List<floatPlayer>();
        }

        public List<ListOfInputs>   ListOfInputsForEachPlayer = new List<ListOfInputs>();

        public string               SaveName = "GP";




        [Header("Default Inputs")]
        public int defaultInput = 0; // 0: PC | 1: Mac
        // -> Default PC Values
        public List<InputInfoParams> InputPCListP1 = new List<InputInfoParams>();
        public List<InputInfoParams> InputPCListP2 = new List<InputInfoParams>();
        // -> Default PC Values
        public List<InputInfoParams> InputMacListP1 = new List<InputInfoParams>();
        public List<InputInfoParams> InputMacListP2 = new List<InputInfoParams>();
        // -> Common values
        public List<FloatInfoParams> FloatInfoList = new List<FloatInfoParams>();
        public List<BoolInfoParams> BoolInfoList = new List<BoolInfoParams>();

        // Reminds default Input when game starts 
        public List<ListOfInputs> ListOfInputsForEachPlayerReminder = new List<ListOfInputs>();


        void Awake()
        {
            #region
            //Check if instance already exists
            if (instance == null)
                //if not, set instance to this
                instance = this;

            //If instance already exists and it's not this:
            else if (instance != this)
                //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
                Destroy(gameObject);
            #endregion
        }

        void Start()
        {
            //DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            //-> Check if inputs are pressed by players
            if(!b_Pause)checkInputs();
        }

        //-> Check if inputs are pressed by players
        public void checkInputs()
        {
            #region
            for (var j = 0; j < ListOfInputsForEachPlayer.Count; j++)
            {
                for (var i = 0; i < ListOfInputsForEachPlayer[j].listOfButtons.Count; i++)
                {
                    float value = 0;

                    KeyCode nKeycode = ListOfInputsForEachPlayer[j].listOfButtons[i]._Keycode;
                    if (Input.GetKey(nKeycode))
                        value = 1;

                    if (Mathf.Abs(Input.GetAxisRaw(ListOfInputsForEachPlayer[j].listOfButtons[i]._AxisName)) > .2f)
                        value = Input.GetAxisRaw(ListOfInputsForEachPlayer[j].listOfButtons[i]._AxisName);
                        
                    ListOfInputsForEachPlayer[j].listOfButtons[i]._AxisCurrentValue = value;

                    //Debug.Log(nKeycode.ToString());

                    CheckGetKeyDownAllController(j, i);
                }
            }
            #endregion
        }

        public string returnAxisName(int playerID, int axisID)
        {
            return ListOfInputsForEachPlayer[playerID].listOfButtons[axisID]._AxisName;
        }


        //-> return axis value
        public float returnAxisValue(int playerID, int axisID)
        {
            return ListOfInputsForEachPlayer[playerID].listOfButtons[axisID]._AxisCurrentValue;
        }

        //-> return if an input is pressed
        public bool returnIsPressedValue(int playerID, int buttonID)
        {
            return ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].b_GetKeyDown;
        }


        public KeyCode ReturnInputKeyCode(int playerID, int buttonID)
        {
            #region
            if (ListOfInputsForEachPlayer.Count > playerID &&
                ListOfInputsForEachPlayer[playerID].listOfButtons.Count > buttonID)
            {
                return ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._Keycode;
            }
            else
            {
                Debug.Log("Keycode doesn't exist");
                return KeyCode.None;
            }
            #endregion
        }

        //-> Load the inputs, float and bool use for all players
        public bool Bool_LoadAllInputs()
        {
            #region
            // Reminds default inputs
            for (var i = 0; i < ListOfInputsForEachPlayer.Count; i++)
            {
                ListOfInputsForEachPlayerReminder.Add(new ListOfInputs());

                //-> Load Input Axis Name
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons.Add(new ButtonKey());
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._Names = ListOfInputsForEachPlayer[i].listOfButtons[j]._Names;
                }

                //-> Sprite
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._Image = ListOfInputsForEachPlayer[i].listOfButtons[j]._Image;
                }

                //-> KeyCode
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._Keycode = ListOfInputsForEachPlayer[i].listOfButtons[j]._Keycode;
                }

                //-> Load Input Axis Name
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._AxisName = ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisName;
                }

                //-> bUseAxisDirection
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j].bUseAxisDirection = ListOfInputsForEachPlayer[i].listOfButtons[j].bUseAxisDirection;
                }

                //-> Load Input Negative or positive value when player press the button
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._AxisPositiveOrNegative = ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisPositiveOrNegative;
                }

                //-> b_GetKeyDown
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j].b_GetKeyDown = ListOfInputsForEachPlayer[i].listOfButtons[j].b_GetKeyDown;
                }

                //-> _AxisCurrentValue
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._AxisCurrentValue = ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisCurrentValue;
                }

                //-> Load Float
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfFloat.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfFloat.Add(new floatPlayer());
                    ListOfInputsForEachPlayerReminder[i].listOfFloat[j]._Value = ListOfInputsForEachPlayer[i].listOfFloat[j]._Value;
                }
                //-> Load Bool
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfBool.Count; j++)
                {
                    ListOfInputsForEachPlayerReminder[i].listOfBool.Add(new BoolPlayer());
                    ListOfInputsForEachPlayerReminder[i].listOfBool[j].b_State = ListOfInputsForEachPlayer[i].listOfBool[j].b_State;
                }
            }

                string sParams = SaveManager.instance.LoadDAT(SaveName);
             if(sParams != "")
             {
                 string[] codes = sParams.Split(':');
                //Debug.Log("sParams: " + sParams);
                 int positionOnList = 0;
                 for (var i = 0; i < ListOfInputsForEachPlayer.Count; i++)
                 {
                    //-> Load the current selected Input mode (Keyboard|Gamepad|Mobile|Other)
                    ListOfInputsForEachPlayer[i].currentSelectedInputMode = int.Parse(codes[positionOnList]);
                    positionOnList++;
                    //-> Load Keycode
                    for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    {
                         ListOfInputsForEachPlayer[i].listOfButtons[j]._Keycode = (KeyCode)System.Enum.Parse(typeof(KeyCode), codes[positionOnList]);
                         positionOnList++;
                    }
                    //-> Load Input Axis Name
                    for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    {
                         ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisName = codes[positionOnList];
                         positionOnList++;
                    }
                    //-> Load Input Negative or positive value when player press the button
                    for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    {
                        ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisPositiveOrNegative = int.Parse(codes[positionOnList]);
                        positionOnList++;
                    }

                    //-> Load Float
                    for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfFloat.Count; j++)
                    {
                        ListOfInputsForEachPlayer[i].listOfFloat[j]._Value = float.Parse(codes[positionOnList]);
                        positionOnList++;
                    }
                    //-> Load Bool
                    for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfBool.Count; j++)
                    {
                        ListOfInputsForEachPlayer[i].listOfBool[j].b_State = bool.Parse(codes[positionOnList]);
                        positionOnList++;
                    }
                }
             }
             // Save Inputs the first time the game is launched
            else
            {

            }
            //Debug.Log("Inputs Loaded");
            return true;
            #endregion
        }

        //-> Save the inputs, float and bool use for all the players
        public bool Bool_SaveAllInputs()
        {
            #region
            string sParams = "";
            for (var i = 0; i < ListOfInputsForEachPlayer.Count; i++)
            {
                //-> Save the current selected Input mode (Keyboard|Gamepad|Mobile|Other)
                sParams += ListOfInputsForEachPlayer[i].currentSelectedInputMode + ":";
                //-> Save Keycode
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {sParams += ListOfInputsForEachPlayer[i].listOfButtons[j]._Keycode.ToString() + ":";}
                //-> Save Input Axis Name
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                {sParams += ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisName.ToString() + ":";}
                //-> Save Input Negative or positive value when player press the button
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                { sParams += ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisPositiveOrNegative.ToString() + ":"; }
                //-> Save Float
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfFloat.Count; j++)
                {sParams += ListOfInputsForEachPlayer[i].listOfFloat[j]._Value.ToString() + ":";}
                //-> Save Bool
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfBool.Count; j++)
                {sParams += ListOfInputsForEachPlayer[i].listOfBool[j].b_State.ToString() + ":";}
            }
            while (!SaveManager.instance.saveAndReturnTrueAFterSaveProcess(sParams, SaveName))
            {
                return false;
            }
            //Debug.Log("Inputs Saved: " + sParams);
            return true;
            #endregion
        }

        //-> Check a specific Input GetKey for all Players. Use during remap Process
        public bool checkSpecificInputAllPlayers(int buttonID)
        {
            for (var i = 0; i < ListOfInputsForEachPlayer.Count; i++)
            {
                if (checkSpecificInput(i, buttonID))return true;
            }
            return false;
        }

        //-> Check a specific Input GetKey for a specific Player (check if the button is pressed Input.GetKey or Input.GetAxisRaw)
        public bool checkSpecificInput(int playerID, int buttonID)
        {
            #region

            float value = 0;
            KeyCode nKeycode = ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._Keycode;

            if (Input.GetKey(nKeycode))
            {
                
                value = 1;
            }
            //-> Check Gamepad
            if (value <= .2f && Input.GetAxisRaw(ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._AxisName) != 0)
            {
                //Debug.Log("_AxisName: " + ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._AxisName);
                value = Input.GetAxisRaw(ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._AxisName);

                // Return true if:
                // The value is positive and the axis correspond to a button pressed when value is positive (1) (vehicle directions)
                // The value is negative and the axis correspond to a button pressed when value is negative (-1)(vehicle directions)
                // It is not important to know if value negative or positive (_AxisPositiveOrNegative == 0) (vehicle buttons)
                if (value > .2f && ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._AxisPositiveOrNegative == 1 ||
                    value < -.2f && ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._AxisPositiveOrNegative == -1 ||
                    ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._AxisPositiveOrNegative == 0)
                {
                    ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].OnGetKeyReceived?.Invoke();
                    return true;
                }
                else
                {
                    ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].OnGetKeyReleasedReceived?.Invoke();
                    return false;
                }
            }
            //-> Check Mobile
            if (value <=.2f) // Mobile
            {
                if (ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].methodsListMobile[0].obj)
                    value = callMethods.Call_A_Method_Only_Float(ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].methodsListMobile);
            }
            //-> Check Other
            if (value <= .2f) // Other
            {
                if (ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].methodsListOther[0].obj)
                    value = callMethods.Call_A_Method_Only_Float(ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].methodsListOther);
            }

            //Debug.Log("playerID: " + playerID + " | buttonID: " + buttonID);
            if (value > .2f)
            {
                //Debug.Log("nKeycode: " + nKeycode);
                ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].OnGetKeyReceived?.Invoke();
                return true;
            }
            else
            {
                ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].OnGetKeyReleasedReceived?.Invoke();
               
                return false;
            }
            #endregion
        }


        //-> Check Input GetKeyDown for a specific Player for all controller types (keyboard,gamepad, mobile, other)
        public bool CheckGetKeyDownAllController(int playerID, int buttonID)
        {
            //Debug.Log("buttonID: " + buttonID);
            #region
            //-> Check Button Down
            bool b_GetKeyDown = ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].b_GetKeyDown;
            bool b_inputIsPressed = checkSpecificInput(playerID,buttonID);

            if (!b_GetKeyDown && b_inputIsPressed)
            {
                if(playerID > 0 && CheckIfAnOtherPlayerUseTheSameKeyboardButton(playerID, buttonID))
                {

                }
                else
                {
                    ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].b_GetKeyDown = true;
                    ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].OnGetKeyDownReceived?.Invoke();
                }
               //Debug.Log("playerID : " + playerID + " : buttonID: " + buttonID);
               return true;
            }

            //-> Check Button Up
            if (!b_inputIsPressed && ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].b_GetKeyDown)
            {
                ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].b_GetKeyDown = false;
                ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID].OnGetKeyUpReceived?.Invoke();
            }
            return false;

            #endregion
        }

        bool CheckIfAnOtherPlayerUseTheSameKeyboardButton(int playerID, int buttonID)
        {
            //-> Player uses the keyboard
            if(ListOfInputsForEachPlayer[playerID].currentSelectedInputMode == 0)
            {
                KeyCode refKeyCode = ListOfInputsForEachPlayer[playerID].listOfButtons[buttonID]._Keycode;
                for (var i = 0; i < ListOfInputsForEachPlayer.Count; i++)
                {
                    if (i != playerID &&
                        refKeyCode == ListOfInputsForEachPlayer[i].listOfButtons[buttonID]._Keycode)
                    {
                        return true;
                    }
                }
            }
            
            return false;
        }

        bool b_Pause = false;
        public void ResetAllInputs()
        {
            b_Pause = true;
            StartCoroutine(WaitRoutine());
            for (var i = 0; i < ListOfInputsForEachPlayer.Count; i++)
            {

                //-> Load Input Axis Name
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j]._Names = ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._Names;

                //-> Sprite
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j]._Image = ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._Image;

                //-> KeyCode
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j]._Keycode = ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._Keycode;

                //-> Load Input Axis Name
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisName = ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._AxisName;

                //-> bUseAxisDirection
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j].bUseAxisDirection = ListOfInputsForEachPlayerReminder[i].listOfButtons[j].bUseAxisDirection;

                //-> Load Input Negative or positive value when player press the button
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisPositiveOrNegative = ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._AxisPositiveOrNegative;

                //-> b_GetKeyDown
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j].b_GetKeyDown = ListOfInputsForEachPlayerReminder[i].listOfButtons[j].b_GetKeyDown;

                //-> _AxisCurrentValue
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfButtons.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfButtons[j]._AxisCurrentValue = ListOfInputsForEachPlayerReminder[i].listOfButtons[j]._AxisCurrentValue;

                //-> Load Float
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfFloat.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfFloat[j]._Value = ListOfInputsForEachPlayerReminder[i].listOfFloat[j]._Value;

                //-> Load Bool
                for (var j = 0; j < ListOfInputsForEachPlayer[i].listOfBool.Count; j++)
                    ListOfInputsForEachPlayer[i].listOfBool[j].b_State = ListOfInputsForEachPlayerReminder[i].listOfBool[j].b_State;

            }

        }

        IEnumerator WaitRoutine()
        {
            b_Pause = true;
            yield return new WaitForSeconds(.25f);
            b_Pause = false;
        }

    }

    [System.Serializable]
    public class InputInfoParams
    {
        public string Name;
        public KeyCode keyCode;
        public string AxisName;
        public bool bUseAxisDirection;
        public int AxisPositiveOrNegative;

        public InputInfoParams(string _Name, KeyCode _keyCode, string _AxisName, bool _bUseAxisDirection, int _AxisPositiveOrNegative)
        {
            Name = _Name;
            keyCode = _keyCode;
            AxisName = _AxisName;
            AxisPositiveOrNegative = _AxisPositiveOrNegative;
            bUseAxisDirection = _bUseAxisDirection;
        }
    }
    [System.Serializable]
    public class FloatInfoParams
    {
        public string Name;
        public float value;

        public FloatInfoParams(string _Name, float _value)
        {
            Name = _Name;
            value = _value;
        }
    }
    [System.Serializable]
    public class BoolInfoParams
    {
        public string Name;
        public bool bState;

        public BoolInfoParams(string _Name, bool _bState)
        {
            Name = _Name;
            bState = _bState;
        }
    }
}
