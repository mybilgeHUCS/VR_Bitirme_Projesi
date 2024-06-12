// Description: VehicleTrail: Used to reset the trail when the vehicle respawned.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class VehicleTrail : MonoBehaviour
    {
		public VehicleDamage        vehicleDamage;
		public VehicleInfo          vehicleInfo;
		public List<GameObject>     trailsList;


        void Start()
        {
			if (vehicleDamage)
			{
				vehicleDamage.VehicleExplosionAction += VehicleExplosion;
				vehicleDamage.VehicleRespawnPart2 += ResetTrail;
			}
		}

		public void OnDestroy()
		{
			#region
			if (vehicleDamage)
			{
				vehicleDamage.VehicleExplosionAction -= VehicleExplosion;
				vehicleDamage.VehicleRespawnPart2 -= ResetTrail;
			}
			#endregion
		}

		public void VehicleExplosion()
		{
			foreach (GameObject trail in trailsList)
				trail.gameObject.SetActive(false);
		}

		public void ResetTrail()
		{
			StartCoroutine(ResetTrailRoutine());
		}


		IEnumerator ResetTrailRoutine()
		{
			Rigidbody rb = vehicleDamage.gameObject.GetComponent<Rigidbody>();

			while (!vehicleInfo.b_IsVehicleAvailableToMove)
				yield return null;


			while (rb.isKinematic)
				yield return null;


			foreach (GameObject trail in trailsList)
				trail.gameObject.SetActive(true);

			Debug.Log("Reset Trails");


			yield return null;
		}
	}
}
