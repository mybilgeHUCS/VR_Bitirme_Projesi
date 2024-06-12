// Description: PU_Repair: Called by PowerUpsSystemAssisatnt on vehicle.
using UnityEngine;

namespace TS.Generics
{
    [System.Serializable]
    public class PU_Repair
    {
        public AudioClip repairSound;
        public float aVolume = 1;

        public void RepairVehicle(GameObject obj,AudioSource aSource)
        {
            // Repair the vehicle (Full Life restore)
            VehicleDamage vehicleDamage = obj.GetComponent<VehicleDamage>();
            vehicleDamage.lifePoints = vehicleDamage.refLifePoints;
            vehicleDamage.UpdateDamageParticleFx(0);


            if (repairSound && aSource && aSource.gameObject.activeInHierarchy)
            {
                aSource.clip = repairSound;
                aSource.volume = aVolume;
                aSource.Play();
            }

            vehicleDamage.UILifeGaugeUpdate(vehicleDamage.refLifePoints);
            //Debug.Log("Repair Done");
        }

        public void InitRepairPowerUp()
        {
            //Debug.Log("Repair Init");
        }
    }
}