// Description: Cam_Follow.cs: use on camera to follow the vehicle P1 | P2
using System.Collections;
using UnityEngine;

namespace TS.Generics
{
    public class Cam_Follow : MonoBehaviour
	{

		public bool                     b_InitDone;
		private bool                    b_InitInProgress;

		private VehicleDamage           vehicleDamage;

		[HideInInspector]
		public VehiclePrefabInit        vehiclePrefabInit;

		public int                      playerID;

		public GameObject               Grp_EnemyDetector;          // Use by PowerUpsSystem to access P1|P2 Enemy detector Raycast
		public PowerUpsAIDetectVehicle  EnemyDetector_01_Front;
		public PowerUpsAIDetectVehicle  EnemyDetector_02_Back;

		[HideInInspector]
		public VehicleInfo              vehicleInfo;
		[HideInInspector]
		public VehicleAI                vehicleAI;
		[HideInInspector]
		public VehicleInputs            vehicleInputs;

		public CameraFx                 cameraFx;

		private camSystem               _camSystem;

        [HideInInspector]
		public float                    distance = 10.0f;          // distance to targetpos2
	
		private float                   refDistance;
       
        [System.Serializable]
        public class CamPreset
        {
			public string       name;
			public Transform    targetLookAt;                   // The camera look at this target

			public float        distance = 10.0f;               // distance to followTargetPos
			public float        speedToReachDefaultDistance = 3;
			public float        accelerationDistance = 2;
			public float        speedToReachAccelerationDistance = 1;
			public Transform    followTargetPos;                // Camera position
            public bool         bSmoothTransition = true;
			public bool         b_IsCamRotationLocked;          // Cam is locked on the same rotation as the target
		}

        public int                      currentCamSelecected;

		private bool                    b_IsEnemyFlagEnable = false;
		private VehicleFlagOnCam        vehicleFlagOnCam;

        public int                      CamState = 0;           // 0: Free Use during countdown | 1: Vehicle Cam Use during the race to follow the vehicle
		private Transform				refCamPosFreeMode;
		public bool                     b_MoveAvailable = true;

		public float					camDampAfterRace = 3;
		[HideInInspector]
		public Transform refCamViewAfterTheRacePos;

		void Start()
		{
			
		}

		public void OnDestroy()
		{
			#region
			if (vehicleInfo)
            {
				vehicleInputs.AccelerationButtonPressed -= AccelerationButtonPressed;
				vehicleInputs.AccelerationButtonUp      -= AccelerationButtonUp;
				vehicleInputs.CameraViewButtonDown      -= CameraViewButtonDown;

				vehicleDamage.VehicleExplosionAction    -= VehicleExplosion;
				vehicleDamage.VehicleRespawnPart2       -= ResetCamera;
			}
			#endregion
		}

        

		//-> Initialisation
		public bool bInitCamFollow()
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

		public IEnumerator InitRoutine()
		{
            if (vehicleInfo)
            {
				vehicleInputs = vehicleInfo.gameObject.GetComponent<VehicleInputs>();
				_camSystem = vehicleInfo.gameObject.GetComponent<camSystem>();

				vehicleFlagOnCam = GetComponent<VehicleFlagOnCam>();
				if (vehicleFlagOnCam)
				{
					b_IsEnemyFlagEnable = true;
				}
				vehicleInputs.AccelerationButtonPressed += AccelerationButtonPressed;
				vehicleInputs.AccelerationButtonUp      += AccelerationButtonUp;
				vehicleInputs.CameraViewButtonDown      += CameraViewButtonDown;

				SelectPreset(currentCamSelecected);

				vehicleDamage = vehicleInfo.gameObject.GetComponent<VehicleDamage>();
				vehicleDamage.VehicleExplosionAction    += VehicleExplosion;
				vehicleDamage.VehicleRespawnPart2       += ResetCamera;

				vehicleAI = vehicleInfo.gameObject.GetComponent<VehicleAI>();
			}

			b_InitDone = true;
			//Debug.Log("Init: Cam_Follow -> Done");
			yield return null;
		}

		//-> Move backward the camera when the player presses the acceleration button.
		public void AccelerationButtonPressed()
        {
            if (vehicleInfo && vehicleInfo.b_IsPlayerInputAvailable) {
				StopAllCoroutines();
				distance = Mathf.MoveTowards(distance, refDistance, Time.deltaTime * _camSystem.camPresetList[currentCamSelecected].speedToReachDefaultDistance);
			}
		}

		//-> Move to default distance the camera when the player releases the acceleration button.
		public void AccelerationButtonUp()
        {
			if (vehicleInfo && vehicleInfo.b_IsPlayerInputAvailable)
			{
				if (vehicleInfo.b_IsVehicleAvailableToMove)
					distance = Mathf.MoveTowards(distance, refDistance + _camSystem.camPresetList[currentCamSelecected].accelerationDistance, Time.deltaTime * _camSystem.camPresetList[currentCamSelecected].speedToReachAccelerationDistance);
				else
					distance = Mathf.MoveTowards(distance, refDistance, Time.deltaTime * _camSystem.camPresetList[currentCamSelecected].speedToReachDefaultDistance);

			}
		}

		//-> Change the camera view when the player changes the camera view
		public void CameraViewButtonDown()
        {
			//-> Change Camera view
			if (vehicleInfo &&
				vehicleInfo.b_IsVehicleAvailableToMove &&
				!PauseManager.instance.Bool_IsGamePaused)
			{
				currentCamSelecected++;
				currentCamSelecected %= _camSystem.camPresetList.Count;
				SelectPreset(currentCamSelecected);
			}	
		}

        void LateUpdate()
		{
            if (b_MoveAvailable)
            {
				if (CamState == 0)
					FreeMode();
				if (CamState == 1)
					CameraVehicleMode();
				if (CamState == 2)
					CamViewAfterTheRace();

			}
		}

        public bool CameraMode(int newCameraMode = 0)
        {
			CamState = newCameraMode;
			return true;
        }

        public void ChooseNewCamPosFreeMode(Transform newCamPos)
        {
			refCamPosFreeMode = newCamPos;
		}

        void FreeMode()
        {
			if (b_InitDone && vehiclePrefabInit &&
                vehiclePrefabInit.b_InitDone &&
                refCamPosFreeMode != null)
			{
				transform.position = refCamPosFreeMode.position;
				transform.rotation = refCamPosFreeMode.rotation;

				//-> Use to display a flag above enemies
				if (b_IsEnemyFlagEnable && ReturnPlayer() && VehicleFlagManager.instance.bFlagAllowed)
					vehicleFlagOnCam.EnemyFlagPosition(playerID);
			}
		}

		void CameraVehicleMode()
        {
			if (b_InitDone && vehiclePrefabInit && vehiclePrefabInit.b_InitDone)
			{
				if (_camSystem.camPresetList[currentCamSelecected].targetLookAt)
				{
					transform.position = _camSystem.camPresetList[currentCamSelecected].followTargetPos.transform.position - _camSystem.camPresetList[currentCamSelecected].followTargetPos.transform.forward * distance;

					if (!_camSystem.camPresetList[currentCamSelecected].b_IsCamRotationLocked)
						transform.LookAt(_camSystem.camPresetList[currentCamSelecected].targetLookAt);

					// Fixed target
					if (_camSystem.camPresetList[currentCamSelecected].b_IsCamRotationLocked)
						transform.rotation = _camSystem.camPresetList[currentCamSelecected].targetLookAt.rotation;
				}


				//-> Use to display a flag above enemies
				//Debug.Log("b_IsEnemyFlagEnable: " + b_IsEnemyFlagEnable + " |ReturnPlayer() " + ReturnPlayer() + " |bFlagAllowed " + VehicleFlagManager.instance.bFlagAllowed);
				if (b_IsEnemyFlagEnable && ReturnPlayer() && VehicleFlagManager.instance.bFlagAllowed)
                {
					vehicleFlagOnCam.EnemyFlagPosition(playerID);
				}
					
			}
		}

        public void SelectPreset(int whichPreset)
        {
            if (_camSystem.camPresetList[currentCamSelecected].bSmoothTransition)
            {
				distance = _camSystem.camPresetList[whichPreset].distance + _camSystem.camPresetList[currentCamSelecected].accelerationDistance * .5f;
			}
            else
            {
				distance = _camSystem.camPresetList[whichPreset].distance + _camSystem.camPresetList[currentCamSelecected].accelerationDistance;

			}
			refDistance = _camSystem.camPresetList[whichPreset].distance;   
		}

        bool ReturnPlayer()
        {
			if (playerID == 0 && VehicleFlagManager.instance.listCams.Count > 0 || playerID == 1 && VehicleFlagManager.instance.listCams.Count > 1)
				return true;
			else
				return false;
		}


        //-> Do something when the vehicle explode
		public void VehicleExplosion()
		{
			b_MoveAvailable = false;

            if(!vehicleAI.enabled)
			    vehicleFlagOnCam.DisableAllFlags(playerID);
		}

        //-> Reset Camera during respawn
		public void ResetCamera()
		{
			if(gameObject.activeInHierarchy)
				StartCoroutine(ResetCameraRoutine());
		}

		IEnumerator ResetCameraRoutine()
		{
			Rigidbody rb = vehicleInfo.gameObject.GetComponent<Rigidbody>();

			while (!vehicleInfo.b_IsVehicleAvailableToMove)
				yield return null;

			while (rb.isKinematic)
				yield return null;

			b_MoveAvailable = true;

			//Debug.Log("Reset Follow");
			yield return null;
		}


		public void InitCamViewAfterRace(Transform camPos)
		{
			refCamViewAfterTheRacePos = camPos;
			CamState = 2; // CamViewAfterTheRace Mode
		}
		
		void CamViewAfterTheRace()
		{
			if (b_InitDone && vehiclePrefabInit &&
				vehiclePrefabInit.b_InitDone &&
				refCamViewAfterTheRacePos != null)
			{
				transform.position = refCamViewAfterTheRacePos.position;

				var rotation = Quaternion.LookRotation(refCamViewAfterTheRacePos.GetChild(0).position - transform.position);
				rotation.x = 0;
				rotation.z = 0;
				transform.rotation = Quaternion.Slerp(transform.rotation, rotation, camDampAfterRace * Time.deltaTime);



				//-> Use to display a flag above enemies
				if (b_IsEnemyFlagEnable && ReturnPlayer() && VehicleFlagManager.instance.bFlagAllowed)
					vehicleFlagOnCam.EnemyFlagPosition(playerID);
			}
		}
	}
}

