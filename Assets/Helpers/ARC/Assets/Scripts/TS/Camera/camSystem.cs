// Description: camSystem: Attached to the vehicle
// Move the object followed by the player camera depending the player inputs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.Generics;
using UnityEngine.Events;

public class camSystem : MonoBehaviour
{
    [HideInInspector]
    public bool SeeInspector;
    [HideInInspector]
    public bool moreOptions;
    [HideInInspector]
    public bool helpBox = true;

    [HideInInspector]
    public bool b_InitDone;
    private bool b_InitInProgress;
    private VehiclePrefabInit vehiclePrefabInit;
    private Rigidbody rb;

    public Transform        Vehicle;

    [Header("Left/Right")]
    public Transform        PosLeft;
    public Transform        PosRight;
    public Transform        PosCenter;
    public Transform        PosCamTarget;
    public float            speedLR = 6;

    [Header("Up/Down")]
    public Transform        PosUp;
    public Transform        PosDown;
    public Transform        PosCenterUD;
    public Transform        PosCamTargetUD2;
    public float            speedUD = 8;

    
    private VehicleBooster  vehicleBooster;
    private VehicleDamage   vehicleDamage;
    private VehicleInfo     vehicleInfo;
    private VehicleAI       vehicleAI;
    private VehicleInputs   vehicleInputs;

    [HideInInspector]
    public float            upDownRotSpeed = 0;
    public AnimationCurve   animCurvesmoothupDown;
    [HideInInspector]
    public int              UDState = 0;


    [Header("Booster")]
    public Transform        camFollowThisObj;
    public Transform        camFollowThisObjNoBoost;
    public Transform        camFollowThisObjBoost;

    [HideInInspector]
    public float            LRRotSpeed = 0;
    public AnimationCurve   animCurvesmoothLR;
    [HideInInspector]
    public int              LRState = 0;

    [HideInInspector]
    public bool             b_MoveAvailable = true;

    [HideInInspector]
    public bool             b_Left;                                 // If True -> the plane turn Left
    [HideInInspector]
    public bool             b_Right;                                // If True -> the plane turn Right
    [HideInInspector]
    public bool             b_Up;                                   // If True -> the plane turn Up
    [HideInInspector]
    public bool             b_Down;                                 // If True -> the plane turn Down

    [HideInInspector]
    public int              currentPresetEditor;
    [HideInInspector]
    public int              tabEditor;
    [HideInInspector]
    public string           presetName;

    public Transform previewCam;

    [System.Serializable]
    public class CamPreset
    {
        public string name;
        public Transform targetLookAt;                      // The camera look at this target

        public float distance = 10.0f;                      // distance to followTargetPos
        public float speedToReachDefaultDistance = 3;
        public float accelerationDistance = 2;
        public float speedToReachAccelerationDistance = 1;
        public Transform followTargetPos;                   // Camera position
        public bool bSmoothTransition = true;
        public bool b_IsCamRotationLocked;                  // Cam is locked on the same rotation as the target
    }
    [Header("Camera Views (Presets)")]
    public List<CamPreset> camPresetList = new List<CamPreset>();


    [Header("Countdown Camera Views")]
    public List<Transform> countdownRefPosCamList = new List<Transform>();

    private Cam_Follow cam_Follow;
    public UnityEvent eventExplosion;


    void Start()
    {
        rb                                      = GetComponent<Rigidbody>();

        vehicleBooster                          = Vehicle.GetComponent<VehicleBooster>();

        vehicleDamage                           = Vehicle.GetComponent<VehicleDamage>();
        vehicleDamage.VehicleExplosionAction    += VehicleExplosion;
        vehicleDamage.VehicleRespawnPart2       += ResetCamera;

        vehicleInfo                             = Vehicle.GetComponent<VehicleInfo>();

        vehicleAI                               = Vehicle.GetComponent<VehicleAI>();

        vehicleInputs                           = Vehicle.GetComponent<VehicleInputs>();
        vehicleInputs.DirDownGetKeyPressed      += DirDownGetKeyPressed;
        vehicleInputs.DirUpGetKeyPressed        += DirUpGetKeyPressed;
        vehicleInputs.DirResetUpDown            += DirResetUpDown;
        vehicleInputs.DirLeftGetKeyPressed      += DirLeftGetKeyPressed;
        vehicleInputs.DirRightGetKeyPressed     += DirRightGetKeyPressed;
        vehicleInputs.DirResetLeftRight         += DirResetLeftRight;


        if (previewCam) previewCam.gameObject.SetActive(false);
    }

    public void OnDestroy()
    {
        #region
        vehicleDamage.VehicleExplosionAction    -= VehicleExplosion;
        vehicleDamage.VehicleRespawnPart2       -= ResetCamera;

        vehicleInputs.DirDownGetKeyPressed      -= DirDownGetKeyPressed;
        vehicleInputs.DirUpGetKeyPressed        -= DirUpGetKeyPressed;
        vehicleInputs.DirResetUpDown            -= DirResetUpDown;
        vehicleInputs.DirLeftGetKeyPressed      -= DirLeftGetKeyPressed;
        vehicleInputs.DirRightGetKeyPressed     -= DirRightGetKeyPressed;
        vehicleInputs.DirResetLeftRight         -= DirResetLeftRight;
        #endregion
    }

    //-> Initialisation
    public bool bInitCamSystem()
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

        if (vehicleInfo.playerNumber == 0 || vehicleInfo.playerNumber == 1)
        {
            //-> Case P1 P2: Init the Cam P1 or P2 depending the player
            Cam_Follow[] playerCameras = GameObject.FindObjectsOfType<Cam_Follow>();

            foreach (Cam_Follow cam in playerCameras)
            {
                if (cam.playerID == vehicleInfo.playerNumber)
                {
                    cam.vehicleInfo = gameObject.GetComponent<VehicleInfo>();
                    cam.vehiclePrefabInit = vehiclePrefabInit;
                    StartCoroutine(cam.InitRoutine());
                    cam_Follow = cam;
                }
            }

            //-> Init the camera target positions.
            PosCamTargetUD2.position = PosCenterUD.position;
            PosCamTarget.position = PosCenter.position;
        }

        b_InitDone = true;
        //Debug.Log("Init: CamSystem -> Done");
        yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vehicle &&
            b_MoveAvailable &&
            vehicleInfo.b_IsVehicleAvailableToMove &&
             vehiclePrefabInit && vehiclePrefabInit.b_InitDone &&
             vehicleInfo.b_IsPlayerInputAvailable &&
             !PauseManager.instance.Bool_IsGamePaused &&
             !vehicleInfo.b_IsRespawn) 
        {
            CamMovementLR();
            CamMovementUD();
            Booster();
        } 
    }

    public void VehicleExplosion()
    {
        b_MoveAvailable = false;
        eventExplosion.Invoke();
    }

    public void ResetCamera()
    {
        if(!vehicleAI.enabled)
            StartCoroutine(ResetCameraRoutine());
    }


    IEnumerator ResetCameraRoutine()
    {
        b_MoveAvailable = false;

        while (!vehicleInfo.b_IsVehicleAvailableToMove)
            yield return null;

        //-> Wait until the vehicle is on the respawned poisiton
        while (rb.isKinematic)
            yield return null;


        LRState = 0;
        UDState = 0;
        
        PosCamTarget.position = PosCenter.position;

        PosCamTargetUD2.position = PosCenterUD.position;

        camFollowThisObj.localPosition = camFollowThisObjNoBoost.localPosition;

        //Debug.Log("Reset Camera");
        
        //yield return new WaitForEndOfFrame();
        b_MoveAvailable = true;

        //Debug.Break();
        yield return null;
    }

    void Booster()
    {
        if(camFollowThisObj && camFollowThisObjNoBoost && camFollowThisObjBoost)
        {
            if (vehicleBooster.b_Booster)
            {
                camFollowThisObj.localPosition = Vector3.MoveTowards(camFollowThisObj.localPosition, camFollowThisObjBoost.localPosition, Time.deltaTime * 1.5f);
            }
            else
            {
                camFollowThisObj.localPosition = Vector3.MoveTowards(camFollowThisObj.localPosition, camFollowThisObjNoBoost.localPosition, Time.deltaTime * 1.5f);
            }
        } 
    }


    //-> The target followed by the camera move to the left or to the right when the player presses its left/right inputs.
    void CamMovementLR()
    {
        if (!vehicleAI.enabled)
        {
            if (b_Left)
            {
                if (LRState != 1) LRRotSpeed = 0;
                LRRotSpeed = Mathf.MoveTowards(LRRotSpeed, 1, Time.deltaTime);

                float smoothGampadMove = vehicleInputs.ReturnLeftInputValue();    // When player use Gamepad movement depend of the value of the axis. Create smotther movement.
                PosCamTarget.position = Vector3.Slerp(PosCamTarget.position, PosLeft.position, Time.deltaTime * speedLR * animCurvesmoothLR.Evaluate(LRRotSpeed) * smoothGampadMove);

                LRState = 1;
            }
            else if (b_Right)
            {
                if (LRState != 2) LRRotSpeed = 0;
                LRRotSpeed = Mathf.MoveTowards(LRRotSpeed, 1, Time.deltaTime);

                float smoothGampadMove = vehicleInputs.ReturnRightInputValue();    // When player use Gamepad movement depend of the value of the axis. Create smotther movement.
                PosCamTarget.position = Vector3.Slerp(PosCamTarget.position, PosRight.position, Time.deltaTime * speedLR * animCurvesmoothLR.Evaluate(LRRotSpeed) * smoothGampadMove);

                LRState = 2;
            }
            else if (!b_Left && !b_Right)
            {
                if (LRState != 0) LRRotSpeed = 0;
                LRRotSpeed = Mathf.MoveTowards(LRRotSpeed, 1, Time.deltaTime);

                PosCamTarget.position = Vector3.Slerp(PosCamTarget.position, PosCenter.position, Time.deltaTime * speedLR * animCurvesmoothLR.Evaluate(LRRotSpeed));

                LRState = 0;
            }
        }
    }



    //-> The target followed by the camera move Up or Down when the player presses its Up/Down inputs.
    void CamMovementUD()
    {

        if (!vehicleAI.enabled)
        {
            int invertInput = vehicleInputs.TSInputBoolInvertUpDown;
            bool invertState = InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfBool[invertInput].b_State;

            // Up | Down
            if (b_Up && !invertState || b_Down && invertState)
            {
                if (UDState != 1) upDownRotSpeed = 0;

                upDownRotSpeed = Mathf.MoveTowards(upDownRotSpeed, 1, Time.deltaTime);

                float smoothGampadMove = 1;    // When player use Gamepad movement depend of the value of the axis. Create smotther movement.
                if (invertState) smoothGampadMove = vehicleInputs.ReturnDownInputValue();
                if (!invertState) smoothGampadMove = vehicleInputs.ReturnUpInputValue();

                PosCamTargetUD2.position = Vector3.Slerp(PosCamTargetUD2.position, PosUp.position, Time.deltaTime * speedUD * animCurvesmoothupDown.Evaluate(upDownRotSpeed) * smoothGampadMove);

                UDState = 1;
            }
            if (b_Down && !invertState || b_Up && invertState)
            {
                if (UDState != 2) upDownRotSpeed = 0;

                upDownRotSpeed = Mathf.MoveTowards(upDownRotSpeed, 1, Time.deltaTime);

                float smoothGampadMove = 1;    // When player use Gamepad movement depend of the value of the axis. Create smotther movement.
                if (!invertState) smoothGampadMove = vehicleInputs.ReturnDownInputValue();
                if (invertState) smoothGampadMove = vehicleInputs.ReturnUpInputValue();

                PosCamTargetUD2.position = Vector3.Slerp(PosCamTargetUD2.position, PosDown.position, Time.deltaTime * speedUD * animCurvesmoothupDown.Evaluate(upDownRotSpeed) * smoothGampadMove);

                UDState = 2;
            }
            else if (!b_Up && !b_Down)
            {
                if (UDState != 0) upDownRotSpeed = 0;
                upDownRotSpeed = Mathf.MoveTowards(upDownRotSpeed, 1, Time.deltaTime);
                PosCamTargetUD2.position = Vector3.Slerp(PosCamTargetUD2.position, PosCenterUD.position, Time.deltaTime * speedUD * animCurvesmoothupDown.Evaluate(upDownRotSpeed));

                UDState = 0;
            }
        }
    }

    //-> Next methods are used to know which input direction is pressed by the player.
    public void DirLeftGetKeyPressed()
    {
        b_Left = true;
        b_Right = false;
    }

    public void DirLeftGetKeyUp()
    {
        b_Left = false;
    }

    public void DirResetLeftRight()
    {
        b_Left = false;
        b_Right = false;
    }

    public void DirRightGetKeyPressed()
    {
        b_Right = true;
        b_Left = false;
    }

    public void DirRightGetKeyUp()
    {
        b_Right = false;
    }

    public void DirUpGetKeyPressed()
    {
        // Player 
        /*if (!vehicleAI.enabled)
        {
            int invertInput = vehicleInputs.TSInputBoolInvertUpDown;
            bool invertState = InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfBool[invertInput].b_State;

            if (!invertState)
            {
                b_Up = true;
                b_Down = false;
            }
            else
            {
                b_Up = false;
                b_Down = true;
            }
        }
        // AI Vehicle
        else
        {
            b_Up = true;
            b_Down = false;
        }*/
        b_Up = true;
        b_Down = false;
    }
    public void DirUpGetKeyUp()
    {
        // Player 
        /*if (!vehicleAI.enabled)
        {
            int invertInput = vehicleInputs.TSInputBoolInvertUpDown;
            bool invertState = InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfBool[invertInput].b_State;

            if (!invertState)
            {
                b_Up = false;
            }
            else
            {
                b_Up = true;
            }
        }
        // AI Vehicle
        else
        {
            b_Up = false;
        }*/
        b_Up = false;
    }

    public void DirDownGetKeyPressed()
    {
        // Player 
        /*if (!vehicleAI.enabled)
        {
            int invertInput = vehicleInputs.TSInputBoolInvertUpDown;
            bool invertState = InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfBool[invertInput].b_State;

            if (!invertState)
            {
                b_Down = true;
                b_Up = false;
            }
            else
            {
                b_Down = false;
                b_Up = true;
            }
        }
        // AI Vehicle
        else
        {
            b_Down = true;
            b_Up = false;
        }*/
        b_Down = true;
        b_Up = false;
    }
    public void DirDownGetKeyUp()
    {
        // Player 
       /* if (!vehicleAI.enabled)
        {
            int invertInput = vehicleInputs.TSInputBoolInvertUpDown;
            bool invertState = InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfBool[invertInput].b_State;

            if (!invertState)
            {
                b_Down = false;
            }
            else
            {
                b_Down = true;
            }
        }
        // AI Vehicle
        else
        {
            b_Down = false;
        }*/
        b_Down = false;
    }

    public void DirResetUpDown()
    {
        b_Up = false;
        b_Down = false;
    }


    public void CameraFxExplosion()
    {
        if (cam_Follow)
        {
            CameraFx cameraFx = cam_Follow.cameraFx;

            if (!vehicleAI.enabled && cameraFx && cameraFx.listVFXSequence[0].cameraShake.b_ShakeEnable)
                cameraFx.listVFXSequence[0].cameraShake.VFXCameraShake(cameraFx, cameraFx.transform, cameraFx);

            if (!vehicleAI.enabled && cameraFx && cameraFx.listVFXSequence[0].cameraEnableObj.b_Enable)
                cameraFx.listVFXSequence[0].cameraEnableObj.VFXCameraEnableObj(cameraFx, cameraFx);
        }
    }
}

