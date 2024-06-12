//Description: AirplaneController: This script manages airplane behavior.
using System.Collections;
using UnityEngine;
using TS.Generics;

namespace TS.ARC
{
    public class AirplaneController : MonoBehaviour, IVehicleStartLine<Vector3>, IGyroStartLine<Quaternion>
    {
        [HideInInspector]
        public bool                     b_InitDone;
        private bool                    b_InitInProgress;
        private VehiclePrefabInit       vehiclePrefabInit;

        [Tooltip("Access to the Rigidbody.")]
        public Rigidbody                rb;                                                 // Reference to the plane Rigidbody
        [Tooltip("Force is applied to the rigigidbody using his position")]
        public Transform                pivotAddForce;                                      // Position where the force is applied to move the plane
        //[HideInInspector]
        public float                    Force = 200;                                        // Force applied to the plane
        private VehicleInfo             vehicleInfo;
        private VehicleDamage           vehicleDamage;
        private VehiclePathFollow       vehiclePathFollow;

        [Header ("Speed parameters")]
        [Tooltip("Speed when the player press break button.")]
        public float                    defaultSpeed = 40;
        [HideInInspector]
        public float                    currentSpeed = 25;                                  // The current plane speed -> defaultSpeed + speed + currentBoost;
        [Tooltip("Speed added to the vehicle when the player doesn't press break button.")]
        public float                    speed = 30;                
        [Tooltip("Speed to reach the max acceleration. The higher the value, the faster the max speed is reached. (Default value 30).")]
        public float                    reachMaxAccSpeed = 30;                       
        [Tooltip("Speed to reach the min acceleration. The higher the value, the faster the min speed is reached. (Default value 15).")]
        public float                    reachMinAccSpeed = 15;                       


        [HideInInspector]
        public float                    refSpeed = 0;                           // Keep a reference of the max speed value
        private VehicleBooster          vehicleBooster;
       
        [HideInInspector]
        public float                    rotationSpeed = 1;                      // Allows to have smooth Left Right rotation (Fake Yaw).         
        [HideInInspector]
        public float                    newXRotation;                           // (Fake Yaw).
        [HideInInspector]
        public float curveSpeed = 0;
        [HideInInspector]
        public float                    upDownRotSpeed = 0;

        [Space(10)] // 10 pixels of spacing here.

        [Tooltip("Use to smooth the transition between Up Down Left and Right plane movement.")]
        public AnimationCurve           animCurvesmoothUDLR;                    // anim curve to smooth Left Right Up Down movement
        private int                     UDState = 0;                            // Use to know if the player press Up or Down button
        private int                     LRState = 0;                            // Use to know if the player press Left or Right button       

        private float                   curveSpeedReachTarget = 1;
        private float                   rotationSpeedNeutral = 20;


        [Header("Gyro Yaw (Plane Y Rotation (Left/Right))")]
        
        [Tooltip("Use to determine the Gyro yaw (Left/Right).")]
        public Transform                GyroYAxis;                              // Reference to the plane Gyroscope Y Axis (Yaw)
        
        [Tooltip("Choose the Gyro speed rotation (Yaw -> Left/Right). Default value .3")]
        public float                    gyroYRotSpeed = .3f;                       // Reference to Left/Right speed rotation

        [HideInInspector]
        public float                    gyroYExtra = 0;
        [Tooltip("Extra Y rotation when the player press Left or Right buttons.")]
        public float                    gyroYExtraRot = 1.3f;
        [HideInInspector]
        public float                    gyroYExtraBreak = 0;
        [Tooltip("Extra Y rotation when the player press break button.")]
        public float                    gyroYExtraBreakRot = .3f;

        [Header("Gyro Pitch (Plane X Rotation (Up/Down))")]

        [Tooltip("Use to determine the Gyro pitch (Up/Down).")]
        public Transform                GyroXAxis;                              // Reference to the plane Gyroscope X Axis (Pitch)
        [Tooltip("Choose the Gyro speed rotation (Pitch -> Up/Down). Default value .3")]
        public float                    gyroXRotSpeed = .3f;                        // Reference to Up/Down speed rotation
        [HideInInspector]
        public float                    gyroXExtra = 0;
        [Tooltip("Extra Y rotation when the player press Left or Right buttons.")]
        public float                    gyroXExtraRot = .5f;
        [HideInInspector]
        public float                    gyroXExtraBreak = 0;
        [Tooltip("Extra Y rotation when the player press break button.")]
        public float                    gyroXExtraBreakRot = .1f;

        [HideInInspector]
        public float                    refgyroYRotSpeed;                       // Use for AI. Remember the default value.
        [HideInInspector]
        public float                    refgyroXRotSpeed;                       // Use for AI. Remember the default value.

        [HideInInspector]
        public Vector3                  pitch;

        [Header("Object that contains 3D plane models")]
        public Transform                Grp_MoveWithWings;                              // Reference to the object that contains the plane 3D models.

        [Header("Fake Plane rotation")]
        private Vector3                 FakeRot_Left = new Vector3(0,0,-45);    // Rotate the plane to -45 when AI or player press Left input
        private Vector3                 FakeRot_Right = new Vector3(0, 0, 45);  // Rotate the plane to 45 when AI or player press Right input
        private Vector3                 FakeRot_Left_Neutral = Vector3.zero;    // Rotate the plane to 0 when AI or player press no input

        [Tooltip("Simulate smooth Left/Right plane rotation (Fake Yaw -> Left/Right). Default value 5")]
        public float                    smoothRotation = 5.0f;
        private float                   refSmoothLR = 5;
        private float                   refSmoothCenter = 4;
        [Tooltip("Smooth Left/Right rotation curve (Fake Yaw). Player P1 | P2")]
        public AnimationCurve           animCurveFakeWing;

        [Tooltip("Smooth Left/Right rotation curve (Fake Yaw). AI")]
        public AnimationCurve animSmoothAI;

        public Transform                targetFakeRotation;

        [Tooltip("Max plane Pitch (Rotation on X Axis).")]
        public float                    pitchMax = 85;                          
        [Tooltip("Min plane Pitch (Rotation on X Axis).")]
        public float                    pitchMin = -85;                         
        public Transform                CubePitchMax;                           // Use to limit plane PitchMax
        public Transform                CubePitchMin;                           // Use to limit plane PitchMin

        private float                   wingRotState = 0;
       
        [HideInInspector]
        public float                    UDSPeed = 1;                            // Use to have smooth Left/Right wings rotation.

        [HideInInspector]
        public bool                     b_Left;                                 // If True -> the plane turn Left
        [HideInInspector]
        public bool                     b_Right;                                // If True -> the plane turn Right
        [HideInInspector]
        public bool                     b_Up;                                   // If True -> the plane turn Up
        [HideInInspector]
        public bool                     b_Down;                                 // If True -> the plane turn Down
        [HideInInspector]
        public bool                     b_Acceleration;                         // If True -> the plane accelerate
        [HideInInspector]
        public int                      lastLeftRight;                           // Used to play a sound when the player press down Left or Right Input.


        [Header("Audio")]
        public GameObject               Grp_Audio;
        public AudioSource              aSourceWind;
        private float                   aSourceWindVolumeRef;
        public float                    targetVolWind = .2f;
        public AudioSource              aSourceEngineSound;                     
        private float                   offsetPitch;

        public AudioSource              aSourceFallSound;                       // AudioSource to play a sound when the plane is falling

        [Tooltip("Use to calculate the pitch of the fall sound")]
        public Transform                target_01;                              // The next 2 Transform allow to calculate the pitch angle
        [Tooltip("Use to calculate the pitch of the fall sound")]
        public Transform                target_02;
        [HideInInspector]
        public float                    angle;                                  // Use when the plane check the fall sound [Tooltip("Use to calculate the pitch of the fall sound")]
        [Tooltip("Use to calculate the pitch of the fall sound")]
        public AudioSource              aSourceLeftSound;                       // AudioSource to play a sound when the plane turn left (Whoosh Sound)
        [Tooltip("Use to calculate the pitch of the fall sound")]
        public AudioSource              aSourceRightSound;                      // AudioSource to play a sound when the plane turn right (Whoosh Sound)


        [Header("Respawn")]
        [Tooltip("Use to find the plane direction after the plane has respawned")]
        public GameObject               refRespawnPoint;

        private VehicleAI               vehicleAI;
        private VehicleInputs           vehicleInputs;


        [Header("Other")]
        public GameObject               vehicleBody;
        public GameObject               vehicleGrpPropellerStop;
        public GameObject               vehicleGrpPropellerRotSim;
        [HideInInspector]
        public float                    aiReachMaxSpeedAfterStart = 0;          // After the countdown or respawn the vehicle reach its max speed with certain of time

        public AnimationCurve           aiReachMaxSpeedAfterStartCurve;


        // Start is called before the first frame update
        void Start()
        {
            #region
            vehicleInfo                             = GetComponent<VehicleInfo>();
            CubePitchMin.localEulerAngles           = new Vector3(pitchMin, 0, 0);
            CubePitchMax.localEulerAngles           = new Vector3(pitchMax, 0, 0);
            refSpeed                                = speed;
            speed                                   = 0;

            refgyroYRotSpeed                        = gyroYRotSpeed;
            refgyroXRotSpeed                        = gyroXRotSpeed;

            vehicleDamage                           = GetComponent<VehicleDamage>();
            vehicleDamage.refLifePoints             = vehicleDamage.lifePoints;
            vehicleDamage.VehicleExplosionAction    += PlaneExplosion;
            vehicleDamage.VehicleRespawnPart1       += VehicleRespawnPart1;
            vehicleDamage.VehicleRespawnPart2       += PlaneRespawn;

            if (GetComponent<VehicleAI>())vehicleAI = GetComponent<VehicleAI>();

            vehiclePathFollow                       = GetComponent<VehiclePathFollow>();

            vehicleBooster                          = GetComponent<VehicleBooster>();

            vehicleInputs                           = GetComponent<VehicleInputs>();
            vehicleInputs.DirDownGetKeyPressed      += DirDownGetKeyPressed;
            vehicleInputs.DirUpGetKeyPressed        += DirUpGetKeyPressed;
            vehicleInputs.DirResetUpDown            += DirResetUpDown;
            vehicleInputs.DirLeftGetKeyPressed      += DirLeftGetKeyPressed;
            vehicleInputs.DirRightGetKeyPressed     += DirRightGetKeyPressed;
            vehicleInputs.DirResetLeftRight         += DirResetLeftRight;
            vehicleInputs.AccelerationButtonPressed += AccelerationButtonPressed;
            vehicleInputs.AccelerationButtonUp      += AccelerationButtonUp;

            if(aSourceWind)
                aSourceWindVolumeRef                = aSourceWind.volume;

            #endregion
        }

        public void OnDestroy()
        {
            #region
            vehicleDamage.VehicleExplosionAction    -= PlaneExplosion;
            vehicleDamage.VehicleRespawnPart1       -= VehicleRespawnPart1;
            vehicleDamage.VehicleRespawnPart2       -= PlaneRespawn;

            vehicleInputs.DirDownGetKeyPressed      -= DirDownGetKeyPressed;
            vehicleInputs.DirUpGetKeyPressed        -= DirUpGetKeyPressed;
            vehicleInputs.DirResetUpDown            -= DirResetUpDown;
            vehicleInputs.DirLeftGetKeyPressed      -= DirLeftGetKeyPressed;
            vehicleInputs.DirRightGetKeyPressed     -= DirRightGetKeyPressed;
            vehicleInputs.DirResetLeftRight         -= DirResetLeftRight;
            vehicleInputs.AccelerationButtonPressed -= AccelerationButtonPressed;
            vehicleInputs.AccelerationButtonUp      -= AccelerationButtonUp;
            #endregion
        }

        //-> Initialisation
        public bool bInitAirplaneCrontroller()
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

            if (DataRef.instance.vehicleGlobalData)
            {
                int vehicleDataID = vehiclePrefabInit.vehicleDataID;

                //-> Init vehicle speed
                // P1 | P2
                if (vehicleInfo.playerNumber == 0
                    ||
                    vehicleInfo.playerNumber == 1 && InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2)
                    refSpeed = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].speed;
                else
                    refSpeed = DataRef.instance.vehicleGlobalData.carParametersList[vehicleDataID].speedAI;
            }

            b_InitDone = true;
            yield return null;
        }



        void PlaneMovement()
        {
            #region
            int playerNumber = vehicleInfo.playerNumber;
            int invertInput = 0;
            bool invertState = false;
            if (playerNumber <= 1) {
                invertInput = vehicleInputs.TSInputBoolInvertUpDown;
                invertState = InfoInputs.instance.ListOfInputsForEachPlayer[vehicleInfo.playerNumber].listOfBool[invertInput].b_State;
            }

            // Up | Down
            if (b_Down && !invertState || b_Up && invertState)
            {
                if (UDState != 1)
                {
                    upDownRotSpeed = 0;
                    gyroXExtra = 0;
                }
                upDownRotSpeed = Mathf.MoveTowards(upDownRotSpeed, 1, Time.deltaTime);
                float smoothGampadMove = 1;
                if (!invertState && playerNumber <= 1) smoothGampadMove = vehicleInputs.ReturnDownInputValue();
                if (invertState && playerNumber <= 1) smoothGampadMove = vehicleInputs.ReturnUpInputValue();

                gyroXExtra = Mathf.MoveTowards(gyroXExtra, gyroXExtraRot + gyroXExtraBreak, Time.deltaTime);

                GyroXAxis.transform.rotation = Quaternion.Slerp(GyroXAxis.transform.rotation, CubePitchMax.rotation, Time.deltaTime * (gyroXRotSpeed + gyroXExtra) * 2 * animCurvesmoothUDLR.Evaluate(upDownRotSpeed) * smoothGampadMove);

                UDState = 1;
            }
            else if (b_Up && !invertState || b_Down && invertState)
            {
                if (UDState != 2)
                {
                    upDownRotSpeed = 0;
                    gyroXExtra = 0;
                }
                upDownRotSpeed = Mathf.MoveTowards(upDownRotSpeed, 1, Time.deltaTime);
                float smoothGampadMove = 1;
                if (invertState && playerNumber <= 1) smoothGampadMove = vehicleInputs.ReturnDownInputValue();
                if (!invertState && playerNumber <= 1) smoothGampadMove = vehicleInputs.ReturnUpInputValue();


                gyroXExtra = Mathf.MoveTowards(gyroXExtra, gyroXExtraRot + gyroXExtraBreak, Time.deltaTime);

                GyroXAxis.transform.rotation = Quaternion.Slerp(GyroXAxis.transform.rotation, CubePitchMin.rotation, Time.deltaTime * (gyroXRotSpeed + gyroXExtra) * 2 * animCurvesmoothUDLR.Evaluate(upDownRotSpeed) * smoothGampadMove);

                UDState = 2;
            }
            else if (!b_Down && !b_Up)
            {
                if (UDState != 0) upDownRotSpeed = 0;

                upDownRotSpeed = Mathf.MoveTowards(upDownRotSpeed, 0, Time.deltaTime);

                gyroXExtra = 0;

                UDState = 0;
            }


            // Left | Right
            if (b_Left)
            {
                if (LRState != 1)
                {
                    curveSpeed = 0;
                    rotationSpeed = 0;
                    gyroYExtra = 0;
                }
                gyroYExtra = Mathf.MoveTowards(gyroYExtra, gyroYExtraRot + gyroYExtraBreak, Time.deltaTime);


                curveSpeed = Mathf.MoveTowards(curveSpeed, 1, Time.deltaTime * curveSpeedReachTarget);

                rotationSpeed = Mathf.MoveTowards(rotationSpeed, 300, Time.deltaTime * 300 * animCurvesmoothUDLR.Evaluate(curveSpeed));

                float smoothGampadMove = 1;
                if (!vehicleAI.enabled && playerNumber <= 1) vehicleInputs.ReturnLeftInputValue();    // When player use Gamepad movement depend of the value of the axis. Create smoother movement.
                newXRotation = Mathf.MoveTowards(newXRotation, -1 /*- offsetRotSpeed*/, Time.deltaTime * rotationSpeed * smoothGampadMove);

                LRState = 1;
            }
            else if (b_Right)
            {

                if (LRState != 2)
                {
                    curveSpeed = 0;
                    rotationSpeed = 0;
                    gyroYExtra = 0;
                }
                gyroYExtra = Mathf.MoveTowards(gyroYExtra, gyroYExtraRot + gyroYExtraBreak, Time.deltaTime);


                curveSpeed = Mathf.MoveTowards(curveSpeed, 1, Time.deltaTime * curveSpeedReachTarget);

                rotationSpeed = Mathf.MoveTowards(rotationSpeed, 300, Time.deltaTime * 300 * animCurvesmoothUDLR.Evaluate(curveSpeed));

                float smoothGampadMove = 1;
                if (!vehicleAI.enabled && playerNumber <= 1) smoothGampadMove = vehicleInputs.ReturnRightInputValue();    // When player use Gamepad movement depend of the value of the axis. Create smoother movement.
                newXRotation = Mathf.MoveTowards(newXRotation, 1, Time.deltaTime * rotationSpeed * smoothGampadMove);

                LRState = 2;
            }
            else if (!b_Left && !b_Right)
            {
                if (LRState != 0)
                {
                    curveSpeed = 0;
                    rotationSpeed = 0;
                }
                gyroYExtra = 0;

                curveSpeed = 0;
                rotationSpeed = 1;
                newXRotation = Mathf.MoveTowards(newXRotation, 0, Time.deltaTime * rotationSpeed * rotationSpeedNeutral);

                LRState = 0;
            }

            if (vehicleAI.enabled)
            {
                if (Mathf.Abs(vehicleAI.angleVehicleLookAT) > 15)
                {
                    gyroYRotSpeed = Mathf.MoveTowards(gyroYRotSpeed, vehicleAI.XTremRotCaseTarget, Time.deltaTime * vehicleAI.XTremRotCaseSpeed);
                    gyroXRotSpeed = Mathf.MoveTowards(gyroXRotSpeed, vehicleAI.XTremRotCaseTarget, Time.deltaTime * vehicleAI.XTremRotCaseSpeed);
                }
                else
                {
                    gyroYRotSpeed = vehicleAI.percentageAIRotationLR * refgyroYRotSpeed;
                    gyroXRotSpeed = vehicleAI.percentageAIRotationUD * refgyroXRotSpeed;
                }
            }

            #endregion
        }


        //-> Plane Direction
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
            b_Up = true;
            b_Down = false;
        }
        public void DirUpGetKeyUp()
        {
            b_Up = false;
        }

        public void DirDownGetKeyPressed()
        {
            b_Down = true;
            b_Up = false;
        }
        public void DirDownGetKeyUp()
        {
            b_Down = false;
        }

        public void DirResetUpDown()
        {
            b_Up = false;
            b_Down = false;
        }

        public void AccelerationButtonPressed()
        {
            b_Acceleration = true;
        }

        public void AccelerationButtonUp()
        {
            b_Acceleration = false;
        }

        public void RespawnGetKeyPressed()
        {
           
        }

        
        void Update()
        {
            #region
            if (b_InitDone &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                !PauseManager.instance.Bool_IsGamePaused)
            {
                if (!vehicleAI.enabled)
                {
                    if (vehicleInfo.b_IsVehicleAvailableToMove && vehicleInfo.b_IsPlayerInputAvailable)
                    {
                        if (lastLeftRight != 0 && b_Left && aSourceLeftSound.gameObject.activeInHierarchy)
                            aSourceLeftSound.Play();

                        if (lastLeftRight != 1 && b_Right && aSourceRightSound.gameObject.activeInHierarchy)
                            aSourceRightSound.Play();

                        if (!b_Acceleration && !vehicleInfo.b_IsRespawn)
                        {
                            speed = Mathf.MoveTowards(speed, refSpeed, Time.deltaTime * reachMaxAccSpeed);
                            offsetPitch = Mathf.MoveTowards(offsetPitch, .3f, Time.deltaTime);

                            gyroYExtraBreak = Mathf.MoveTowards(gyroYExtraBreak, 0, Time.deltaTime);
                            gyroXExtraBreak = Mathf.MoveTowards(gyroXExtraBreak, 0, Time.deltaTime);
                        }
                        else
                        {
                            speed = Mathf.MoveTowards(speed, 0, Time.deltaTime * reachMinAccSpeed);
                            offsetPitch = Mathf.MoveTowards(offsetPitch, 0, Time.deltaTime);

                            gyroYExtraBreak = Mathf.MoveTowards(gyroYExtraBreak, gyroYExtraBreakRot, Time.deltaTime);
                            gyroXExtraBreak = Mathf.MoveTowards(gyroXExtraBreak, gyroXExtraBreakRot, Time.deltaTime);
                        }

                        currentSpeed = defaultSpeed + speed * aiReachMaxSpeedAfterStartCurve.Evaluate(vehicleAI.aiSmoothStart) + vehicleBooster.currentBoost  ;
                        lastLeftRight = returnlastLeftRightValue();
                    }

                    fallSound();
                }
                else
                {
                   
                }
                if (vehicleAI.aiSmoothStart != 1) vehicleAI.aiSmoothStart = Mathf.MoveTowards(vehicleAI.aiSmoothStart, 1, .75f * Time.deltaTime);
            }
            
            #endregion
        }
        
        int returnlastLeftRightValue()
        {
            if (b_Left) return 0;
            else if (b_Right) return 1;
            else return 2;
        }


        float t;
        int current;
        // Update is called once per frame
        void FixedUpdate()
        {
            #region
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone)
            {
                if (vehicleInfo.b_IsVehicleAvailableToMove && !PauseManager.instance.Bool_IsGamePaused)
                {
                    // -> Check plane movements
                    if (vehicleInfo.b_IsPlayerInputAvailable)
                    {
                        PlaneMovement();
                        FakePlaneRotation();
                    }
                  

                    //-> Apply movement to the plane
                    //-> P1 P2 Case
                    if (!vehicleAI.enabled)
                    {
                        // if currentSpeed = 0 the Player vehicle stops
                        float ratio = Mathf.Clamp(currentSpeed, 0, 1);
                        rb.AddForceAtPosition(pivotAddForce.forward * Force * ratio, pivotAddForce.position, ForceMode.Force);

                    }
                    //-> AI case
                    else
                    {
                        if (vehicleInfo.b_IsVehicleAvailableToMove && vehiclePathFollow.b_MoveAvailable)
                        {
                            rb.velocity = Vector3.MoveTowards(rb.velocity, vehicleAI.desiredVelocity,Time.deltaTime * 300);


                            float maxSpeed = defaultSpeed +
                                (refSpeed  +
                                vehicleAI.OffsetSpeedDistanceAiToP1 +
                                vehicleBooster.currentBoost) * aiReachMaxSpeedAfterStartCurve.Evaluate(vehicleAI.aiSmoothStart);
                            if (rb.velocity.magnitude > maxSpeed)
                                rb.velocity = rb.velocity.normalized * (maxSpeed);
                        }
                    }

                    pitch = newXRotation * GyroYAxis.up * gyroYRotSpeed * Time.deltaTime;

                    // Fake Yaw
                    GyroYAxis.transform.Rotate(0, newXRotation * (gyroYRotSpeed + gyroYExtra), 0);

                    // Fake Pitch
                    Quaternion deltaRotation2 = Quaternion.Euler(pitch);

                    rb.MoveRotation(GyroXAxis.rotation * deltaRotation2);

                    //-> Limit the plane velocity
                    if (rb.velocity.magnitude > currentSpeed && !vehicleAI.enabled)
                    {
                        rb.velocity = rb.velocity.normalized * (currentSpeed);
                    }
                }
            }            
            #endregion
        }

       public float CheckAngle()
        {
            #region
            Vector3 targetDir =  target_02.transform.position - target_01.position;
            Vector3 forward = target_01.transform.forward;

            float angle = Vector3.SignedAngle(targetDir, forward, target_01.transform.right);

            return angle;
            #endregion
        }



        void FakePlaneRotation()
        {
            #region
           
            if (b_Left)
            {
                if (wingRotState != 1) UDSPeed = 0;
                UDSPeed = Mathf.MoveTowards(UDSPeed, 1, Time.deltaTime);
                targetFakeRotation.localEulerAngles = FakeRot_Left;
                smoothRotation = Mathf.MoveTowards(smoothRotation, refSmoothLR, Time.deltaTime * 5 );
                wingRotState = 1;
            }
            else if (b_Right)
            {
                if (wingRotState != 2) UDSPeed = 0;
                UDSPeed = Mathf.MoveTowards(UDSPeed, 1, Time.deltaTime);
                targetFakeRotation.localEulerAngles = FakeRot_Right;
                smoothRotation = Mathf.MoveTowards(smoothRotation, refSmoothLR, Time.deltaTime * 5);
                wingRotState = 2;
            }
            else if(!b_Left && !b_Right )
            {
                if (wingRotState != 0) UDSPeed = 0;
                UDSPeed = Mathf.MoveTowards(UDSPeed, 1, Time.deltaTime);
                targetFakeRotation.localEulerAngles = FakeRot_Left_Neutral;
                smoothRotation = Mathf.MoveTowards(smoothRotation, refSmoothCenter, Time.deltaTime * 5);
                wingRotState = 0;
            }

            if(!vehicleAI.enabled)
                Grp_MoveWithWings.transform.rotation = Quaternion.Slerp(Grp_MoveWithWings.transform.rotation, targetFakeRotation.rotation, Time.deltaTime * smoothRotation * animCurveFakeWing.Evaluate(UDSPeed));
            else
                Grp_MoveWithWings.transform.rotation = Quaternion.Slerp(Grp_MoveWithWings.transform.rotation, targetFakeRotation.rotation, Time.deltaTime * smoothRotation * animSmoothAI.Evaluate(UDSPeed));

            #endregion
        }

        
        void fallSound()
        {
            #region
            angle = CheckAngle();
            if (CheckAngle() < 0)
            {
                aSourceFallSound.volume = .7f * (CheckAngle() - 0) / (pitchMin - 0);
            }
            else
            {
                aSourceFallSound.volume = 0;
            }

            if (CheckAngle() < 0)
            {
                aSourceEngineSound.pitch = Mathf.MoveTowards(aSourceEngineSound.pitch, offsetPitch + 1 + .4f * (CheckAngle() - 0) / (pitchMin - 0), Time.deltaTime);
                aSourceWind.pitch = Mathf.MoveTowards(aSourceWind.pitch, offsetPitch + 1 + .4f * (CheckAngle() - 0) / (pitchMin - 0), Time.deltaTime);
            }
            else
            {
                aSourceEngineSound.pitch = Mathf.MoveTowards(aSourceEngineSound.pitch, offsetPitch + 1 - .4f * (CheckAngle() - 0) / (pitchMax - 0), Time.deltaTime);
                aSourceWind.pitch = Mathf.MoveTowards(aSourceWind.pitch, offsetPitch + 1 - .4f * (CheckAngle() - 0) / (pitchMax - 0), Time.deltaTime);
            }

            if (!aSourceWind.isPlaying &&
                (vehicleInfo.playerNumber == 0 || vehicleInfo.playerNumber == 1 && InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer == 2) &&
                !vehicleInfo.b_IsRespawn &&
                aSourceWind.gameObject.activeInHierarchy)
                aSourceWind.Play();

            var volBoost = Mathf.Clamp(vehicleBooster.currentBoost / vehicleBooster.HowManyBoost, 0, targetVolWind);
            aSourceWind.volume = aSourceWindVolumeRef + volBoost;


            #endregion
        }


        public void PlaneExplosion()
        {
            #region
            Grp_MoveWithWings.gameObject.SetActive(false);
            vehicleInfo.b_IsVehicleAvailableToMove = false;
            Force = 0;
            rb.isKinematic = true;
            aSourceWind.Stop();
            aSourceEngineSound.Stop();
            #endregion
        }

        public void PlaneRespawn()
        {
            StartCoroutine(RespawnPart2Routine());
        }


        public IEnumerator RespawnPart1Routine()
        {
            #region
            aiReachMaxSpeedAfterStart = 0;
            Force = 0;

            // Reset Input
            speed = 0;
            offsetPitch = 0;
            //offsetRotSpeedTarget = 3;

            //Reset Fake Plane Rotation
            UDSPeed = 0;
            wingRotState = 0;

            // Reset Up/Down Rotation
            UDState = 0;
            upDownRotSpeed = 0;

            // Reset Left/Right Rotation
            LRState = 0;
            curveSpeed = 0;
            rotationSpeed = 0;
            newXRotation = 0;

            rb.velocity = Vector3.zero;

            Vector3 respawnPoint = vehicleDamage.respawnPoint;
            Quaternion respawnRotation = vehicleDamage.respawnRotation;

            transform.position = respawnPoint;

            refRespawnPoint.transform.rotation = respawnRotation;
            refRespawnPoint.transform.rotation = Quaternion.LookRotation(-refRespawnPoint.transform.forward, refRespawnPoint.transform.up);
            refRespawnPoint.transform.position = GyroYAxis.transform.position;

            GyroYAxis.localEulerAngles = new Vector3(0, refRespawnPoint.transform.localEulerAngles.y, 0);

            GyroXAxis.localEulerAngles = new Vector3(refRespawnPoint.transform.localEulerAngles.x, 0, 0);

            Grp_MoveWithWings.localRotation = Quaternion.identity;
            transform.rotation = refRespawnPoint.transform.rotation;
            yield return null;
            #endregion
        }



        public void InitVehiclePosition(Vector3 pos)
        {
           transform.position = pos;
        }

        public void InitVehicleOffsetPosition(Vector3 pos)
        {
            // Init Offset vehicle position during countdown
            GetComponent<VehiclePathFollow>().offsetAIPos = new Vector2(pos.x, pos.y);
        }

        public void InitVehicleGyroPosition(Quaternion quat)
        {
            GyroYAxis.localEulerAngles
                = new Vector3(0, quat.eulerAngles.y, 0);

            GyroXAxis.localEulerAngles
                = new Vector3(quat.eulerAngles.x, 0, 0);


            Grp_MoveWithWings.localRotation = Quaternion.identity;
            transform.rotation = quat;
        }


        public bool EnablePropellerStop()
        {
            vehicleGrpPropellerStop.SetActive(true);
            vehicleGrpPropellerRotSim.SetActive(false);
            return true;
        }

        public bool EnablePropellerRotSim()
        {
            vehicleGrpPropellerStop.SetActive(false);
            vehicleGrpPropellerRotSim.SetActive(true);
            return true;
        }


        public void VehicleRespawnPart1()
        {
            StartCoroutine(RespawnPart1Routine());
        }



        public IEnumerator RespawnPart2Routine()
        {
            #region

            Grp_MoveWithWings.gameObject.SetActive(true);

            yield return new WaitUntil(() => transform.rotation == refRespawnPoint.transform.rotation);

            rb.isKinematic = false;

            yield return new WaitUntil(() => vehicleInfo.b_IsVehicleAvailableToMove);

            Force = 200;
            rb.velocity = GyroXAxis.transform.forward.normalized * -10;

            if (!vehicleAI.enabled && aSourceWind.gameObject.activeInHierarchy)
                aSourceWind.Play();

            if (aSourceEngineSound.gameObject.activeInHierarchy)
                aSourceEngineSound.Play();

            yield return null;
            #endregion
        }
    }
}


 