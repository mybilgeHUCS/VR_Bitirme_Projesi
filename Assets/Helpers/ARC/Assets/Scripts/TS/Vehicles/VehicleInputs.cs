using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TS.Generics;
using TS.ARC;
using System;

namespace TS.Generics
{
    public class VehicleInputs : MonoBehaviour
    {
        public bool b_InitDone;
        private bool b_InitInProgress;
        private VehiclePrefabInit vehiclePrefabInit;

        private VehicleInfo vehicleInfo;
        private VehicleAI vehicleAI;

        public bool bUseTSInputManager = true;
        private int howManyInputsAvailable = 0; 

        [Header("Inputs")]
        public KeyCode keyUp = KeyCode.UpArrow;
        public KeyCode keyDown = KeyCode.DownArrow;
        public KeyCode keyLeft = KeyCode.LeftArrow;
        public KeyCode keyRight = KeyCode.RightArrow;
        public KeyCode keyBooster = KeyCode.Space;
        public KeyCode keyAccelerator = KeyCode.V;
        public KeyCode keyPowerUps = KeyCode.C;
        public KeyCode keyCameraView = KeyCode.N;

        public int TSInputKeyUp = 5;
        public int TSInputKeyDown = 6;
        public int TSInputKeyLeft = 3;
        public int TSInputKeyRight = 4;
        public int TSInputKeyBooster = 7;
        public int TSInputKeyAccelerator = 9;
        public int TSInputKeyPowerUps = 8;
        public int TSInputKeyCameraView = 10;

        public int TSInputBoolInvertUpDown = 0;

        public Action BoosterGetKeyPressed;
        public Action BoosterGetKeyReleased;

        public Action DirDownGetKeyPressed;
        public Action DirUpGetKeyPressed;
        public Action DirResetUpDown;
        public Action DirLeftGetKeyPressed;
        public Action DirRightGetKeyPressed;
        public Action DirResetLeftRight;
        public Action AccelerationButtonPressed;
        public Action AccelerationButtonUp;

        public Action PowerUpGetKeyDown;
        public Action PowerUpGetKeyUp;

        public Action CameraViewButtonDown;

        void Start()
        {
            #region
            vehicleInfo = GetComponent<VehicleInfo>();
            vehicleAI = GetComponent<VehicleAI>();

            #endregion
        }

        //-> Initialisation
        public bool bInitVehicleInputs()
        {
            #region
            //-> Play the coroutine Once
            if (!b_InitInProgress)
            {
                b_InitInProgress = true;
                b_InitDone = false;
                StartCoroutine(InitRoutine());
            }
            //-> Check if the coroutine is finished
            else if (b_InitDone)
                b_InitInProgress = false;

            return b_InitDone;
            #endregion
        }

        IEnumerator InitRoutine()
        {
            vehiclePrefabInit = transform.parent.GetComponent<VehiclePrefabInit>();

            howManyInputsAvailable = InfoInputs.instance.ListOfInputsForEachPlayer.Count;

            // Delegate
            for(var i = 0;i< howManyInputsAvailable; i++)
            {
                if (i != vehicleInfo.playerNumber)
                    continue;

                //-> Up (TSInputKeyUp = 5) | Down (TSInputKeyDown = 6)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyUp].OnGetKeyReceived               += OnGetKeyDirUpAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyUp].OnGetKeyReleasedReceived       += OnGetKeyReleasedDirResetUpDownAction;
                //
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyDown].OnGetKeyReceived             += OnGetKeyDirDownAction;

                //-> Left (TSInputKeyLeft = 3) | Right (TSInputKeyRight = 4)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyLeft].OnGetKeyReceived             += OnGetKeyDirLeftAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyLeft].OnGetKeyReleasedReceived     += OnGetKeyReleasedDirResetLeftRightAction;
                //
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyRight].OnGetKeyReceived            += OnGetKeyDirRightAction;

                //-> Booster (TSInputKeyBooster = 7)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyBooster].OnGetKeyReceived          += OnGetKeyPressedBoosterAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyBooster].OnGetKeyUpReceived        += OnGetKeyUpBoosterAction;

                //-> Power Up (TSInputKeyPowerUps = 8)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyPowerUps].OnGetKeyDownReceived     += OnGetKeyPressedPowerUpAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyPowerUps].OnGetKeyUpReceived       += OnGetKeyUpPowerUpAction;

                //-> Camera View (TSInputKeyCameraView = 10)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyCameraView].OnGetKeyDownReceived   += OnGetKeyPressedCameraAction;

                //-> Accelerator (TSInputKeyAccelerator = 9)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyAccelerator].OnGetKeyReceived      += OnGetKeyPressedAcceleartionAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyAccelerator].OnGetKeyReleasedReceived += OnGetKeyRelealedAcceleartionAction;
            }
            b_InitDone = true;
            //Debug.Log("Init: VehicleInputs -> Done");
            yield return null;
        }


        void OnDestroy()
        {
            for (var i = 0; i < howManyInputsAvailable; i++)
            {
                if (i != vehicleInfo.playerNumber)
                    continue;
                //-> Up (TSInputKeyUp = 5) | Down (TSInputKeyDown = 6)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyUp].OnGetKeyReceived -= OnGetKeyDirUpAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyUp].OnGetKeyReleasedReceived -= OnGetKeyReleasedDirResetUpDownAction;
                //
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyDown].OnGetKeyReceived -= OnGetKeyDirDownAction;

                //-> Left (TSInputKeyLeft = 3) | Right (TSInputKeyRight = 4)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyLeft].OnGetKeyReceived -= OnGetKeyDirLeftAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyLeft].OnGetKeyReleasedReceived -= OnGetKeyReleasedDirResetLeftRightAction;
                //
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyRight].OnGetKeyReceived -= OnGetKeyDirRightAction;

                //-> Booster (TSInputKeyBooster = 7)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyBooster].OnGetKeyReceived -= OnGetKeyPressedBoosterAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyBooster].OnGetKeyUpReceived -= OnGetKeyUpBoosterAction;

                //-> Power Up (TSInputKeyPowerUps = 8)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyPowerUps].OnGetKeyDownReceived -= OnGetKeyPressedPowerUpAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyPowerUps].OnGetKeyUpReceived -= OnGetKeyUpPowerUpAction;

                //-> Camera View (TSInputKeyCameraView = 10)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyCameraView].OnGetKeyDownReceived -= OnGetKeyPressedCameraAction;

                //-> Accelerator (TSInputKeyAccelerator = 9)
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyAccelerator].OnGetKeyReceived -= OnGetKeyPressedAcceleartionAction;
                InfoInputs.instance.ListOfInputsForEachPlayer[i].listOfButtons[TSInputKeyAccelerator].OnGetKeyReleasedReceived -= OnGetKeyRelealedAcceleartionAction;
            }
        }


        //-> Section Action (Delegate) ----> Start <----

        //-> Camera views
        void OnGetKeyPressedCameraAction()
        {
            #region
            if (ReturnActionAvailable())
            { CameraViewButtonDown?.Invoke(); }
            #endregion
        }

        //-> Booster
        void OnGetKeyPressedBoosterAction()
        {
            #region
            if (ReturnActionAvailable())
            { BoosterGetKeyPressed?.Invoke(); }
            #endregion
        }

        void OnGetKeyUpBoosterAction()
        {
            #region
            if (ReturnActionAvailable())
            { BoosterGetKeyReleased?.Invoke(); }
            #endregion
        }

        //-> Power Up
        void OnGetKeyPressedPowerUpAction()
        {
            #region
            if (ReturnActionAvailable() && !vehicleInfo.b_IsRespawn)
            { PowerUpGetKeyDown?.Invoke(); }
            #endregion
        }

        void OnGetKeyUpPowerUpAction()
        {
            #region
            if (ReturnActionAvailable())
            { PowerUpGetKeyUp?.Invoke(); }
            #endregion
        }

        //-> Up | down
        void OnGetKeyDirUpAction()
        {
            #region
            if (ReturnActionAvailable())
            {DirUpGetKeyPressed?.Invoke();}
            #endregion
        }

        void OnGetKeyDirDownAction()
        {
            #region
            if (ReturnActionAvailable())
            { DirDownGetKeyPressed?.Invoke(); }
            #endregion
        }

        void OnGetKeyReleasedDirResetUpDownAction()
        {
            #region
            if (ReturnActionAvailable())
            {
                if (!InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyDown].b_GetKeyDown &&
                    !InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyUp].b_GetKeyDown)
                {DirResetUpDown?.Invoke();}   
            }
            #endregion
        }

        public float ReturnUpInputValue()
        {
            return Mathf.Abs(InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyUp]._AxisCurrentValue);
        }
        public float ReturnDownInputValue()
        {
            return Mathf.Abs(InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyDown]._AxisCurrentValue);
        }

        //-> Left | Right
        void OnGetKeyDirLeftAction()
        {
            #region
            if (ReturnActionAvailable())
            { DirLeftGetKeyPressed?.Invoke();}
            #endregion
        }

        void OnGetKeyDirRightAction()
        {
            #region
            if (ReturnActionAvailable())
            { DirRightGetKeyPressed?.Invoke(); }
            #endregion
        }

        void OnGetKeyReleasedDirResetLeftRightAction()
        {
            #region
            if (ReturnActionAvailable())
            {
                if (!InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyLeft].b_GetKeyDown &&
                    !InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyRight].b_GetKeyDown)
                { DirResetLeftRight?.Invoke(); }
            }
            #endregion
        }

        public float ReturnLeftInputValue()
        {
            return Mathf.Abs(InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyLeft]._AxisCurrentValue);
        }
        public float ReturnRightInputValue()
        {
            return Mathf.Abs(InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfButtons[TSInputKeyRight]._AxisCurrentValue);
        }


        //-> Acceleration
        void OnGetKeyPressedAcceleartionAction()
        {
            #region
            if (ReturnActionAvailable() && !vehicleInfo.b_IsRespawn)
            {AccelerationButtonPressed?.Invoke(); }
            #endregion
        }

        void OnGetKeyRelealedAcceleartionAction()
        {
            #region
            if (ReturnActionAvailable() && !vehicleInfo.b_IsRespawn)
            { AccelerationButtonUp?.Invoke(); }
            #endregion
        }


        bool ReturnActionAvailable()
        {
            #region
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone &&
            !vehicleAI.enabled && vehicleInfo.playerNumber < howManyInputsAvailable)
                return true;

            return false;
            #endregion
        }

        //----> End <----



        void Update()
        {
            #region
            //-> TS Input system is not used. Default inputs are used. Important: Only Keyboard is available.
            if (!bUseTSInputManager) TSInputSystemisNotUse();
            #endregion
        }


        void TSInputSystemisNotUse()
        {
            #region
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone &&
                !vehicleAI.enabled && vehicleInfo.playerNumber < howManyInputsAvailable)
            {
                if (vehicleInfo.b_IsVehicleAvailableToMove)
                {
                    // Up | Down
                     if (Input.GetKey(keyDown))
                     {
                         if (DirDownGetKeyPressed != null)
                             DirDownGetKeyPressed?.Invoke();
                     }
                     else if (Input.GetKey(keyUp))
                     {
                         if (DirUpGetKeyPressed != null)
                             DirUpGetKeyPressed?.Invoke();
                     }
                     else if (!Input.GetKey(keyDown) && !Input.GetKey(keyUp))
                     {
                         if (DirResetUpDown != null)
                             DirResetUpDown?.Invoke();
                     }

                     // Left | Right
                     if (Input.GetKey(keyLeft))
                     {
                         if (DirLeftGetKeyPressed != null)
                             DirLeftGetKeyPressed?.Invoke();
                     }
                     else if (Input.GetKey(keyRight))
                     {
                         if (DirRightGetKeyPressed != null)
                             DirRightGetKeyPressed?.Invoke();
                     }
                     else if (!Input.GetKey(keyLeft) && !Input.GetKey(keyRight))
                     {
                         if (DirResetLeftRight != null)
                             DirResetLeftRight?.Invoke();
                     }

                     // Acceleration
                     if (Input.GetKey(keyDown) && !vehicleInfo.b_IsRespawn)
                     {
                         if (AccelerationButtonPressed != null)
                             AccelerationButtonPressed?.Invoke();
                     }
                     else
                     {
                         if (AccelerationButtonUp != null)
                             AccelerationButtonUp?.Invoke();
                     }

                    // Booster keyBooster
                    if (Input.GetKeyDown(keyBooster))
                    {
                        if (BoosterGetKeyPressed != null)
                            BoosterGetKeyPressed?.Invoke();
                    }
                    else if (Input.GetKeyUp(keyBooster))
                    {
                        if (BoosterGetKeyReleased != null)
                            BoosterGetKeyReleased?.Invoke();
                    }

                    // Power-Ups keyPowerUps
                    if (Input.GetKeyDown(keyPowerUps))
                    {
                        if (PowerUpGetKeyDown != null)
                            PowerUpGetKeyDown?.Invoke();
                    }
                    else if (Input.GetKeyUp(keyPowerUps))
                    {
                        if (PowerUpGetKeyUp != null)
                            PowerUpGetKeyUp?.Invoke();
                    }

                    // Camera keyCameraView
                    if (Input.GetKeyDown(keyCameraView))
                    {
                        //if (CameraViewButtonDown != null)
                            CameraViewButtonDown?.Invoke();
                    }
                }
            }
            #endregion
        }
    }

}
