//Description : CarPathFollow.cs : Allow car AI to follow a path. ||  Use to know the position of each car on race. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TS.Generics {
	public class VehiclePathFollow : MonoBehaviour
	{
        public bool b_InitDone;
		private bool b_InitInProgress;
		private VehiclePrefabInit vehiclePrefabInit;

		[Header("Used during Initialisation")]
		public GameObject TargetPart1Prefab;
		public GameObject TargetPart2Prefab;
		public GameObject TargetPart3Prefab;

		//[HideInInspector]
		public Transform target;
		[HideInInspector]
		public Transform refPathTarget;

        [Header ("Info Lap")]
        public float progressDistance = 0;                   // The progress round the route, used in smooth mode.
		public int lapCounter;
        public float lapProgression;


        public float nextPointDistance = 1.45f;
		[HideInInspector]
		public Transform targetNextPointTurnCheckPos;
		[HideInInspector]
		public Transform targetPart4; 
		private Vector3 NextPointTurnCheckPos;
		private Quaternion NextPointTurnCheckRot;
		[HideInInspector]
		public Vector3 refOverridePositionToPath;
		//[HideInInspector]
		public Path Track;                                   // A reference to the waypoint-based route we should follow

		private bool pathExist = false;                     // check if there are checkpoints on track path

        [HideInInspector]
		public VehicleInfo vehicleInfo;
		private VehicleDamage vehicleDamage;
		private VehicleAI vehicleAI;

		public Transform objLookAtTargetX;
		public Transform objLookAtTargetY;
		[HideInInspector]
		public Transform objColliderForcedFollowTHePath;


		public bool b_IsForcedTargetEnabled = false;

		private GameObject lastTriggerPathOverride;
		private GameObject lastTriggerLookATAPowerUp;
		private GameObject lastTriggerUpdatePath;

		public Vector2 offsetAIPos = Vector2.zero;
		private Vector2 currentOffsetAIPos = Vector2.zero;
        [HideInInspector]
		public Vector2 currentTargetOffsetAIPos = Vector2.zero;
		private float reachOffsetTargetSpeed = 8;
		public bool b_MoveAvailable = true;

		private int currentAltPath = 0;         // Use to determine which alt path is choosed by the AI
		float distPPath;                        

		void Start()
		{
			//StartCoroutine(InitRoutine());

			vehicleDamage = GetComponent<VehicleDamage>();
			vehicleDamage.VehicleExplosionAction += VehicleExplosion;

			currentOffsetAIPos = offsetAIPos;
			currentTargetOffsetAIPos = currentOffsetAIPos;
		}

        void OnDestroy()
        {
			vehicleDamage.VehicleExplosionAction -= VehicleExplosion;
		}

		//-> Initialisation
		public bool bInitVehiclePathFollow()
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
			//Clone and Init Path the path
			PathRef pathRef = Instantiate(PathRef.instance, PathRef.instance.transform.parent);

			// Connect track to the script
			Track = pathRef.Track;
			objColliderForcedFollowTHePath = pathRef.BonusSpot;

			if (Track != null && Track.checkpoints.Count > 0)
				pathExist = true;

			//-> Create Path Targets
			GameObject newTargetPart01 = Instantiate(TargetPart1Prefab);
			newTargetPart01.name = "P" + transform.GetComponent<VehicleInfo>().playerNumber + "_Target";
			newTargetPart01.transform.GetChild(0).name = "P" + transform.GetComponent<VehicleInfo>().playerNumber + "_Target_Part2";

			GameObject newTargetPart02 = Instantiate(TargetPart2Prefab);
			newTargetPart02.name = "P" + transform.GetComponent<VehicleInfo>().playerNumber + "_Target_Part3";

			GameObject newTargetPart03 = Instantiate(TargetPart3Prefab);
			newTargetPart03.name = "P" + transform.GetComponent<VehicleInfo>().playerNumber + "_Target_Part4";

			//-> Connect path targets to this script
			target = newTargetPart01.transform;                                                                          // access the target that follow the car																
			refPathTarget = target;
			refOverridePositionToPath = target.GetChild(0).transform.localPosition;

			targetNextPointTurnCheckPos = newTargetPart02.transform;
            targetPart4 = newTargetPart03.transform;

			vehicleInfo = GetComponent<VehicleInfo>();
			vehicleAI = GetComponent<VehicleAI>();

			progressDistance = PathRef.instance.Track.pathLength - StartLine.instance.StartPosDistanceFromSTart% PathRef.instance.Track.pathLength;

			vehiclePrefabInit = transform.parent.GetComponent<VehiclePrefabInit>();

			b_InitDone = true;
			//Debug.Log("Init: VehiclePathFollow -> Done");
			yield return null;
		}

		void Update()
		{
            if (b_InitDone &&
                vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                !PauseManager.instance.Bool_IsGamePaused)
            {
				if (Track != null && target != null && pathExist)
				{
					currentOffsetAIPos = Vector2.MoveTowards(currentOffsetAIPos, currentTargetOffsetAIPos, Time.deltaTime * reachOffsetTargetSpeed);

					target.position = Track.TargetPositionOnPath((progressDistance + nextPointDistance) % Track.pathLength);                                   // find the next position for the target	
					target.position += target.up * currentOffsetAIPos.y + target.right * currentOffsetAIPos.x;
					target.rotation = Quaternion.LookRotation(Track.TargetRotationOnPath((progressDistance + nextPointDistance) % Track.pathLength));                    // find the new rotation for the target

					if (targetNextPointTurnCheckPos)
					{
						targetNextPointTurnCheckPos.position = Track.TargetPositionOnPath((progressDistance + 4 * nextPointDistance) % Track.pathLength);                                   // find the next position for the target	
						targetNextPointTurnCheckPos.position += targetNextPointTurnCheckPos.up * currentOffsetAIPos.y + targetNextPointTurnCheckPos.right * currentOffsetAIPos.x;
						targetNextPointTurnCheckPos.rotation = Quaternion.LookRotation(Track.TargetRotationOnPath((progressDistance + 4 * nextPointDistance) % Track.pathLength));                    // find the new rotation for the target

						targetPart4.position = Track.TargetPositionOnPath((progressDistance + 1.5f * nextPointDistance) % Track.pathLength);                                   // find the next position for the target	
						targetPart4.position += targetPart4.up * currentOffsetAIPos.y + targetPart4.right * currentOffsetAIPos.x;
						targetPart4.rotation = Quaternion.LookRotation(Track.TargetRotationOnPath((progressDistance + 1.5f * nextPointDistance) % Track.pathLength));
					}

					Vector3 progressDelta = Track.TargetPositionOnPath(progressDistance) - transform.position;
					if (Vector3.Dot(progressDelta, Track.TargetRotationOnPath(progressDistance)) < 0)
					{                                               // if progress point position is behind the car
						progressDistance += progressDelta.magnitude * 0.5f;                                                     // change the progress point position
					}


					

                    //-> If The player 1 or 2 is too far from the target on the path.
                    // Recalculate the target position.
                    // The target is used to calculate the position of each vehicle.
                    if (!vehicleAI.enabled &&
						LapCounterAndPosition.instance.posList.Count > vehicleInfo.playerNumber)
                    {
						distPPath = progressDistance - LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].lastPathDistance;

						if (Mathf.Abs(distPPath) > 150)
							progressDistance = LapCounterAndPosition.instance.posList[vehicleInfo.playerNumber].lastPathDistance;
					}



					//-> Look to the target on path
					if (objLookAtTargetY)
					{
						objLookAtTargetY.LookAt(target.GetChild(0).transform);
						objLookAtTargetY.localEulerAngles = new Vector3(0, objLookAtTargetY.localEulerAngles.y, 0);
					}
					if (objLookAtTargetX)
					{
						objLookAtTargetX.LookAt(target.GetChild(0).transform);
						objLookAtTargetX.localEulerAngles = new Vector3(objLookAtTargetX.localEulerAngles.x, 0, 0);
					}

				}

				if (Track != null && progressDistance / Track.pathLength > 1)
				{
					progressDistance = progressDistance % Track.pathLength;
					lapCounter++;
				}

				lapProgression = progressDistance / Track.pathLength;
			}
		}
	

		

		void OnDrawGizmos()
		{
			if (Application.isPlaying && Track != null && target && target.GetChild(0).transform != null)
			{
				Gizmos.color = Color.yellow;                                                                                // Create a line between the car position and the target position
				Gizmos.DrawLine(transform.position, target.GetChild(0).transform.position);

				Gizmos.color = Color.red;                                                                                // Create a line between the car position and the target position
				Gizmos.DrawSphere(NextPointTurnCheckPos, 5);

				Gizmos.color = Color.blue;
				Gizmos.DrawLine(NextPointTurnCheckPos, NextPointTurnCheckRot.eulerAngles * 20);
			}
		}


        public void ForcedFollowSpecificTarget(Transform newTarget,Transform objInsertAfter)
        {
			b_IsForcedTargetEnabled = true;

			//Debug.Log("newTarget: " + newTarget.name);
			NewOffsetTarget(new Vector2(0, 0), false);

			objColliderForcedFollowTHePath.position = newTarget.position;
			objColliderForcedFollowTHePath.rotation = newTarget.rotation;
			//Debug.Log("Power Up 1");

			for (var i = 0; i < Track.checkpoints.Count; i++)
            {
                if(Track.checkpoints[i] == objColliderForcedFollowTHePath)
                {
					Track.checkpoints.RemoveAt(i);
					break;
				}
				
			}

	        for (var i = 0; i < Track.checkpoints.Count; i++)
			{
				//Debug.Log("Power Up 2");
				if (objInsertAfter.position == Track.checkpoints[i].position)
				{
					//Debug.Log("Power Up 3: " + i + " -> " + objInsertAfter.name); 
					Transform trans;

					trans = objColliderForcedFollowTHePath;



					int insertPos = (i + 1) % Track.checkpoints.Count;
					Track.checkpoints.Insert(insertPos, trans);
					//Track.IDUpdatePathPos = insertPos;

					Track.CreateLists();

					break;
				}
			}
          
		}

        public void ForcedFollowThePath()
        {
			b_IsForcedTargetEnabled = false;
			target = refPathTarget;
			NewOffsetTarget(Vector2.zero, true);    //-> Init the vehicle offset position
		}

		public void NewOffsetTarget(Vector2 newOffset = default(Vector2), bool b_DefaultOffset = false)
		{
			if (!b_DefaultOffset)
				currentTargetOffsetAIPos = newOffset;
			else
				currentTargetOffsetAIPos = offsetAIPos;
		}


		

		[Serializable]
		public class PathInfo
		{
			public Path objPath;
			public int ID;

            public PathInfo(Path _objPath,int _ID)
            {
				this.objPath = _objPath;
				this.ID = _ID;
			}
		}

		

        public void VehicleExplosion()
        {
			ForcedFollowThePath();
		}


		//int counter;
		void OnTriggerEnter(Collider other)
		{
            if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone)
            {
				if (vehicleAI && vehicleAI.enabled)
				{
					CheckAltPath(other.gameObject);

                    //->
					TriggerPathOverride triggerPathOverride = other.GetComponent<TriggerPathOverride>();
					if (triggerPathOverride &&
						other.gameObject != lastTriggerPathOverride)
					{
						//Debug.Log("a");
						if (!b_IsForcedTargetEnabled)
						{
							if (target)
							{
								if (triggerPathOverride.refPosition)
									target.GetChild(0).transform.localPosition = refOverridePositionToPath;
								else
									target.GetChild(0).transform.localPosition = triggerPathOverride.OverrideTargetPosition;
							}
						}
						else
						{
							if (refPathTarget)
							{
								if (triggerPathOverride.refPosition)
									refPathTarget.GetChild(0).transform.localPosition = refOverridePositionToPath;
								else
									refPathTarget.GetChild(0).transform.localPosition = triggerPathOverride.OverrideTargetPosition;
							}
						}

						lastTriggerPathOverride = other.gameObject;
					}


					//-> Go to specific Target (Use to pick up power-up)
					if (other.GetComponent<PathTriggerAIChoosePU>() &&
						other.gameObject != lastTriggerLookATAPowerUp)
					{
						//Debug.Log("Look at a power up");
						GrpPowerUp grpPowerUp = other.transform.parent.GetComponent<GrpPowerUp>();

						Transform newTarget = grpPowerUp.SelectAPowerUp(GetComponent<VehicleInfo>().playerNumber);

						if (newTarget != null)
						{
							//Debug.Log(Track.checkpoints[grpPowerUp.objInsertAfterPathID].name);
							ForcedFollowSpecificTarget(newTarget, grpPowerUp.objInsertAfter);
						}
						lastTriggerLookATAPowerUp = other.gameObject;
					}

					
				}
                //-> Vehicle is not managed by the AI (P1 or P2)
                else
                {
					//-> The Player P1 P2 use an Alternative Path
					if (other.GetComponent<AltPathPlayerTrigger>() &&
						other.gameObject != lastTriggerUpdatePath)
					{
						TriggerAltPath triggerAltPath = other.GetComponent<AltPathPlayerTrigger>().altPath.triggerAltPath;
						int whichAltPath = 0;
					    for (var i = 0; i < triggerAltPath.AltPathList.Count; i++)
						{
							if (triggerAltPath.AltPathList[i] == other.GetComponent<AltPathPlayerTrigger>().altPath)
							{
								break;
							}
							else
							{
								whichAltPath++;
							}
						}
						//counter++;
						//Debug.Log("whichAltPath: " + whichAltPath + " --> " + counter);
						CheckAltPath(other.GetComponent<AltPathPlayerTrigger>().altPath.triggerAltPath.gameObject, whichAltPath);
						lastTriggerUpdatePath = other.gameObject;
					}

					if (other.GetComponent<TriggerAltPath>() &&
						other.gameObject != lastTriggerUpdatePath)
					{
						int whichAltPath = -2;
						CheckAltPath(other.gameObject, whichAltPath);
						lastTriggerUpdatePath = other.gameObject;
					}
				}
			}
		}



		// WHen the AI enter into a TriggerAltPath. Determine if the AI Path must be updated 
		void CheckAltPath(GameObject other,int whichAltPath = -1)
        {
			//Debug.Log("other :" + other.name + " -> whichAltPath: " + whichAltPath);
			TriggerAltPath triggerAltPath = other.GetComponent<TriggerAltPath>();
			int aiID = vehicleInfo.playerNumber;
			if (triggerAltPath && !LapCounterAndPosition.instance.posList[aiID].IsRaceComplete)
			{
				//-> Select a Path with random value
				// AI case
				if (whichAltPath == -1)
                {
					int howManyRealPlayer = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.HowManyPlayer;
					//int howManyPlayerTotal = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.howManyVehicleInSelectedGameMode;
					//int difficultyManagerID = aiID - howManyRealPlayer;
					//-> Check best path probability
					int currentAIDifficulty = InfoRememberMainMenuSelection.instance.playerMainMenuSelection.currentDifficulty;
					float proba = 0;


                    if (aiID >= howManyRealPlayer)
					{
						int pos = Mathf.Abs(aiID + 1 - GameModeGlobal.instance.vehicleIDList.Count);

						pos = Mathf.Clamp(pos, 0, DataRef.instance.difficultyManagerData.difficultyParamsList[currentAIDifficulty].aICarParams.Count - 1);
						proba = DataRef.instance.difficultyManagerData.difficultyParamsList[currentAIDifficulty].aICarParams[pos].chooseBestAltPath;
					}


					int rand = UnityEngine.Random.Range(0, 100);

                    //-> Use the best path
                    if(proba >= rand)
						currentAltPath = triggerAltPath.bestPath + 1;
                    //-> Use a random selected path
                    else 
					    currentAltPath = UnityEngine.Random.Range(0, triggerAltPath.AltPathList.Count + 1);
				}
                // P1|P2 case
                else
                {
					currentAltPath = whichAltPath;
				}

				//Debug.Log("Alt Path numebr: " + currentAltPath);

				// The player P1 or P2 use the Main path
				if (whichAltPath == -2)
				{
					#region
					//-> Create a temporary list of path checkpoints using the Main Path Checkpoints
					List<Transform> tmpCheckpoints = new List<Transform>(Track.checkpointsRef);
					
					int counter = 0;
					for (var i = 0; i < tmpCheckpoints.Count; i++)
					{
						if (PathRef.instance.Track.checkpointsRef[i] == triggerAltPath.checkpointParent)
						{
							//counter++;
							break;
						}
						else
						{
							counter++;
						}
					}

					//-> Update Track Checkpoints
					Track.checkpoints = new List<Transform>(tmpCheckpoints);

					//-> Update All the parameters needed to use the Track
					Track.CreateLists();
                    
					//-> Recalculate the AI target to follow
					progressDistance = Track.DistanceFromCheckpointToStart(counter) + nextPointDistance;

				}
				//-> If currentAltPath != 0 an Alt path is selected. So add Alt Path checkpoints to the Main Path
				// currentAltPath != 0 means that it is a request done by AI
				// whichAltPath != -1  means that it is a request done by P1|P2
				else if (currentAltPath != 0 || whichAltPath != -1)
				{
					#region
					//-> Create a temporary list of path checkpoints using the Main Path Checkpoints
					List<Transform> tmpCheckpoints = new List<Transform>(Track.checkpointsRef);

                    if (whichAltPath == -1)
						currentAltPath--;

					//-> Find the Alt Path
					AltPath altPath = triggerAltPath.AltPathList[currentAltPath];
					if (altPath)
					{
						int counter = 0;
						// Find where the Alt Path starts on the Main Path Point
						for (var i = 0; i < tmpCheckpoints.Count; i++)
						{
							if (PathRef.instance.Track.checkpointsRef[i] == triggerAltPath.AltPathList[currentAltPath].checkpointStart)
							{
								counter++;
								break;
							}
							else
							{
								counter++;
							}
						}
						//Debug.Log("counter: " + counter);

						// Remove points between the checkpointStart and checkpointEnd
						int HowManyPointsBetweenTheTwoPoints = 0;
						for (var i = counter; i < tmpCheckpoints.Count; i++)
						{
							if (PathRef.instance.Track.checkpointsRef[i] == triggerAltPath.AltPathList[currentAltPath].checkpointEnd)
							{
								//HowManyPointsBetweenTheTwoPoints--;
								break;
							}
							else
							{
								HowManyPointsBetweenTheTwoPoints++;
							}
						}

						//Debug.Log("HowManyPointsBetweenTheTwoPoints: " + HowManyPointsBetweenTheTwoPoints);
						// Remove checkpoints that are not needed
						for (var i = 0; i < HowManyPointsBetweenTheTwoPoints; i++)
						{
							//Debug.Log(i + " : " + tmpCheckpoints[counter].name);
							tmpCheckpoints.RemoveAt(counter);
						}


						//-> Add Alt Path checkpoints
						for (var i = altPath.tmpCheckpoints.Count - 1; i >= 0; i--)
						{tmpCheckpoints.Insert(counter, altPath.tmpCheckpoints[i]);}

                        //-> Update Track Checkpoints
						Track.checkpoints = new List<Transform>(tmpCheckpoints);

                        //-> Update All the parameters needed to use the Track
						Track.CreateLists();

						//-> Recalculate the AI target to follow
						progressDistance = Track.DistanceFromCheckpointToStart(counter-1) + nextPointDistance;
						
					}
					
					#endregion
				}
					#endregion
			}
		}
	}
}
