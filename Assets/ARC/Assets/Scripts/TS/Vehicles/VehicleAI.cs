// Description : VehicleAI.cs : Manage car AI driving. Input Left/Right/Up/Down and acceleration. This script works with the vehicle controller.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TS.ARC;
using TS.Generics;

namespace TS.Generics
{
	public class VehicleAI : MonoBehaviour
	{
		public bool b_InitDone;
		private bool b_InitInProgress;
		private VehiclePrefabInit vehiclePrefabInit;

		public bool SeeInspector = false;
		[HideInInspector]
		public bool b_Pause = false;            // use game is paused
		private GameObject Target;                                                   // Target followed by the car AI                                     
		private VehicleBooster vehicleBooster;
		private VehicleInputs vehicleInputs;
		private VehicleInfo vehicleInfo;

		private VehiclePathFollow vehiclePathFollow;
		private LapCounterAndPosition lapCounterAndPosition;

		[HideInInspector]
		public float angle = 0;                 // Find angle between car the target path

        
		[HideInInspector]
		public float percentageAIRotationLR;
		[HideInInspector]
		public float percentageAIRotationUD;

		

		//public LayerMask layerMaskPowerUps;
		[HideInInspector]
		public float XTremRotCaseTarget = 3;    // If the angle between vehicle and target is too big. Override the vehicle rotation speed using the next two values
		[HideInInspector]
		public float XTremRotCaseSpeed = 5;


		//---->
		[HideInInspector]
		public float distanceToTarget;
		[HideInInspector]
		public float angleVehicleLookAT;
		[HideInInspector]
		public float anglePart2And4;

		public bool b_ComeBackToTheRaceFront = false;
		[HideInInspector]
		public float distanceFromAIToP1;
		[HideInInspector]
		public float tmpProgressionCPU = 0;
		[HideInInspector]
		public float OffsetSpeedDistanceAiToP1 = 0;
		[HideInInspector]
		public float targetSpeed = 2000;
		[HideInInspector]
		public float multiplier;
		[HideInInspector]
		public Vector3 desiredVelocity;
		//[HideInInspector]
		public float movementSpeed = 6000;
		[HideInInspector]
		public bool b_ReachMaxSpeed;

		//---->

		[HideInInspector]
		public float angleLR;
		[HideInInspector]
		public float angleUD;
		//public Transform objRefAngleUDAxis;
		[HideInInspector]
		public float minimumMovement = .2f;
		[HideInInspector]
		public float minminimumAngle = 1f;

		[HideInInspector]
		public float aiSmoothStart = 1;


		public CallMethods_Pc callMethods;                              // Access script taht allow to call public function in this script.
		public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
				= new List<EditorMethodsList_Pc.MethodsList>();

		//---->

		// --> Use this for initialization
		void Start()
		{
			vehicleBooster = GetComponent<VehicleBooster>();
			vehiclePathFollow = GetComponent<VehiclePathFollow>();
			vehicleInputs = GetComponent<VehicleInputs>();
			vehicleInfo = GetComponent<VehicleInfo>();

			//b_InitDone = true;
		}

		//-> Initialisation
		public bool bInitVehicleAI()
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

			Target = GetComponent<VehiclePathFollow>().target.transform.GetChild(0).gameObject;


            // If this vehicle is player 0 or 1 disable the vehicleAiScript
			GetComponent<VehicleInfo>().DisableAIDuringInitialisationIfNeeded();

			//-> Init Default vehicle Speed using Difficulty manager Data
			int howManyPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
			int vehicleID = GetComponent<VehicleInfo>().playerNumber;
			int currentDifficulty = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentDifficulty;

			if (vehicleID >= howManyPlayer)
            {
				int pos = Mathf.Abs(vehicleID + 1 - GameModeGlobal.instance.vehicleIDList.Count);

				pos = Mathf.Clamp(pos, 0, DataRef.instance.difficultyManagerData.difficultyParamsList[currentDifficulty].aICarParams.Count-1); 
				float speedOffset = DataRef.instance.difficultyManagerData.difficultyParamsList[currentDifficulty].aICarParams[pos].speedOffset;
				GetComponent<AirplaneController>().defaultSpeed += speedOffset;
				//Debug.Log("vehicleID: " + vehicleID + " -> " + GameModeGlobal.instance.vehicleIDList.Count + " -> " + pos);
			}
            /*else
            {
				Debug.Log("Real Player: " + vehicleID + " -> " + GameModeGlobal.instance.vehicleIDList.Count);
			}*/

            


			b_InitDone = true;
			//Debug.Log("Init: VehicleAI -> Done");
			yield return null;
		}

		void Update()
		{
            if (b_InitDone &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                !PauseManager.instance.Bool_IsGamePaused)
            {
				//-> Check 1: Check if there is a huge angle between the 2 Targets. Know if a big turn coming soon.
				//float angle = Vector3.SignedAngle(vehiclePathFollow.target.forward, vehiclePathFollow.targetNextPointTurnCheckPos.forward, Vector3.up);

				//-> Check 2: Check The distance between the 2 targets is good enough. 
				distanceToTarget = Vector3.Distance(vehiclePathFollow.target.position, vehiclePathFollow.targetNextPointTurnCheckPos.position);

				//-> Check 3: Check if the vehicle is looking to its target.
				angleVehicleLookAT = Vector3.SignedAngle(-vehiclePathFollow.transform.forward, vehiclePathFollow.objLookAtTargetY.forward, Vector3.up);

				//-> CHeck 4: Check The difficulty of the turn (ex: U turn)
				anglePart2And4 = Vector3.SignedAngle(vehiclePathFollow.target.forward, vehiclePathFollow.targetPart4.forward, Vector3.up);

				// Check Distance from AI to P1
				callMethods.Call_A_Method(methodsList);

				Vector3 direction = (vehiclePathFollow.target.transform.position - transform.position).normalized;
				desiredVelocity = direction * movementSpeed;
			}
		}

		// --> Fixed Update function
		void FixedUpdate()
		{
			if (b_InitDone &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
				!PauseManager.instance.Bool_IsGamePaused)
				TurnLeftAndRight();                                                 // Turn car to the left or to the right if needed
		}


		public void CheckDistanceToRealPlayer()
		{
			// Check Distance from AI to P1
			tmpProgressionCPU = vehiclePathFollow.lapProgression;

			float tmpProgressionP1 = VehiclesRef.instance.listVehicles[0].GetComponent<VehiclePathFollow>().lapProgression;

			distanceFromAIToP1 = tmpProgressionCPU - tmpProgressionP1;

            if(LapCounterAndPosition.instance.posList.Count > 0)
            {
                // multiplier manage distance for each AI depending its race position
				int multiplier = LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].RacePos;

				if (distanceFromAIToP1 < -0.02f * multiplier && !b_ComeBackToTheRaceFront)
				{
					OffsetSpeedDistanceAiToP1 = Mathf.MoveTowards(OffsetSpeedDistanceAiToP1, 10, Time.deltaTime * 2.5f);
					b_ComeBackToTheRaceFront = true;
				}
				else if (b_ComeBackToTheRaceFront)
				{
					OffsetSpeedDistanceAiToP1 = Mathf.MoveTowards(OffsetSpeedDistanceAiToP1, 10, Time.deltaTime * 2.5f);
					if (distanceFromAIToP1 > .01f - 0.02f * multiplier)
						b_ComeBackToTheRaceFront = false;
				}
				else
				{
					OffsetSpeedDistanceAiToP1 = Mathf.MoveTowards(OffsetSpeedDistanceAiToP1, 0, Time.deltaTime * 2.5f);
				}
			}

			//Debug.Log(b_ComeBackToTheRaceFront + " -> here: " + tmpProgressionP1 + " - " + tmpProgressionCPU + " = " +distanceFromAIToP1);
		}


		
		// -- > Turn car to the left or to the right if needed
		public void TurnLeftAndRight()
		{
			Vector3 PosCenter = transform.position;
			Vector3 dir = Target.transform.position - PosCenter;                                    // Angle between car and target

			angle = Vector2.Angle(new Vector2(dir.x, dir.z),                                                // Find angle between car the target path
				new Vector2(transform.forward.x, transform.forward.z));


			angleLR = CheckAngleLR();                                  // Find if car is on the left or on th( right of the target path
			angleUD = CheckAngleUD();
			

			percentageAIRotationLR = (Mathf.Abs(angleLR) - 0) / (10F - 0);
			percentageAIRotationLR = Mathf.Clamp(percentageAIRotationLR, 0, 1);
			// scaledValue = (rawValue - min) / (max - min);
			percentageAIRotationUD = (Mathf.Abs(angleUD) - 0) / (10F - 0);
			percentageAIRotationUD = Mathf.Clamp(percentageAIRotationUD, 0, 1);

			// -> Follow the path
			// Check if the vehicle needs to turn left or right
			if (angleLR > minminimumAngle && percentageAIRotationLR > minimumMovement )
			{                                           
				 if (vehicleInputs.DirLeftGetKeyPressed != null)
					vehicleInputs.DirLeftGetKeyPressed.Invoke();
			}
			else if (angleLR < -minminimumAngle && percentageAIRotationLR > minimumMovement)
			{
				if (vehicleInputs.DirRightGetKeyPressed != null)
					vehicleInputs.DirRightGetKeyPressed.Invoke();
			}
			else
			{
				if (vehicleInputs.DirResetLeftRight != null)
					vehicleInputs.DirResetLeftRight.Invoke();
			}

			// Check if the vehicle needs to turn up or down
			if (angleUD < -minminimumAngle && percentageAIRotationUD > minimumMovement)
			{                                           
				if (vehicleInputs.DirUpGetKeyPressed != null)
					vehicleInputs.DirUpGetKeyPressed.Invoke();
			}
			else if (angleUD > minminimumAngle && percentageAIRotationUD > minimumMovement)
			{
				if (vehicleInputs.DirDownGetKeyPressed != null)
					vehicleInputs.DirDownGetKeyPressed.Invoke();
			}
			else
			{
				if (vehicleInputs.DirResetUpDown != null)
					vehicleInputs.DirResetUpDown.Invoke();
			}

            //-> Catch current selected Power-up
            if (vehiclePathFollow.b_IsForcedTargetEnabled)
            {
				angleUD = CheckAngleUDForcedTarget();
			}
		}



		public float CheckAngle(Transform airplanePos, Transform target_02)
		{
			#region
			Vector3 targetDir = target_02.transform.position - airplanePos.position;
			Vector3 forward = airplanePos.transform.forward;

			float angle = Vector3.SignedAngle(targetDir, forward, airplanePos.transform.right);

			return angle;
			#endregion
		}

		public float CheckAngleLR()
		{
			#region
			Vector3 targetDir = -vehiclePathFollow.objLookAtTargetY.forward;
			Vector3 forward = transform.forward;
			float angle = Vector3.SignedAngle(targetDir, forward, vehiclePathFollow.objLookAtTargetY.transform.up);

			return angle;
			#endregion
		}

		public float CheckAngleUD()
		{
			#region
			Vector3 targetDir = vehiclePathFollow.objLookAtTargetX.forward;
			Vector3 forward = transform.forward;
			float angle = Vector3.SignedAngle(targetDir, forward, vehiclePathFollow.objLookAtTargetX.transform.right);

			return angle;
			#endregion
		}

		public float CheckAngleLRForcedTarget()
		{
			#region
			Vector3 targetDir = vehiclePathFollow.objColliderForcedFollowTHePath.transform.position - transform.position;
			Vector3 forward = transform.forward;
			float angle = Vector3.SignedAngle(targetDir, forward, vehiclePathFollow.objLookAtTargetY.transform.up);

			return angle;
			#endregion
		}

		public float CheckAngleUDForcedTarget()
		{
			#region
			Vector3 targetDir = vehiclePathFollow.objColliderForcedFollowTHePath.transform.position - transform.position;

		
			Vector3 forward = transform.forward;
			float angle = Vector3.SignedAngle(targetDir, forward, vehiclePathFollow.objLookAtTargetX.transform.right);

			return angle;
			#endregion
		}

     

		public void StopCo()
		{
			StopAllCoroutines();
		}


		public void Pause()
		{
			if (b_Pause)
			{                                   // -> Stop Pause
				b_Pause = false;
			}
			else
			{                                           // -> Start Pause
				b_Pause = true;
			}
		}


	}

}
