//descrpition:
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TS.Generics
{
    public class VehicleFlagOnCam : MonoBehaviour
    {
		//-> Use to display a flag above enemies
		public void EnemyFlagPosition(int playerID)
		{
			if (VehicleFlagManager.instance.b_InitDone)
            {
				for (var j = 0; j < VehiclesVisibleByTheCamList.instance.listVehiclesVisibleByCamera[playerID].listVehiclesVisible.Count; j++)
				{
					if (VehiclesVisibleByTheCamList.instance.listVehiclesVisibleByCamera[playerID].listVehiclesVisible[j])
					{
						Vector3 screenPos = VehicleFlagManager.instance.listCams[playerID].WorldToScreenPoint(VehiclesRef.instance.listVehicles[j].transform.position);

						float dist = Vector3.Distance(transform.position, VehiclesRef.instance.listVehicles[j].transform.position);

						float scaledValue = dist / 200;

						scaledValue = Mathf.Clamp(scaledValue,0,1);

						if (!VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].gameObject.activeSelf)
						{
							VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].transform.position = screenPos;
							VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].gameObject.SetActive(true);
						}
						else
						{
							VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].transform.position =
								Vector3.Lerp(VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].transform.position, screenPos, Time.deltaTime * VehicleFlagManager.instance.speed);


							VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].localScale =
								Vector3.Lerp(
                                    VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].localScale, VehicleFlagManager.instance.flagScale
									- VehicleFlagManager.instance.ReduceFlagSizeWithEnemyDistance * scaledValue,
                                    Time.deltaTime * VehicleFlagManager.instance.speed);
						}

						VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listLastPos[j] = screenPos;
					}
					else
					{
						if (VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].gameObject.activeSelf)
							VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[j].gameObject.SetActive(false);
					}
				}
			}
		}

		//-> Use to display a flag above enemies
		public void DisableAllFlags(int playerID)
		{
            for(var i = 0;i< VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags.Count; i++)
            {
				if (VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[i].gameObject)
					VehicleFlagManager.instance.listPlayerFlagsInfo[playerID].listPlaneFlags[i].gameObject.SetActive(false);
			}
		}
    }

}
