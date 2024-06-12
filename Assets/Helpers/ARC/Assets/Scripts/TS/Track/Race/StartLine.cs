// Description: StartLine: Managed start race line
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class StartLine : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int editorSelectedList;
        public string editorNewCountdownName;

        public static StartLine instance = null;

        public Color GizmoColor = new Color(0, .9f, 1f, .5f);
        public Color GizmoGridPosColor = new Color(0, .9f, 1f, .5f);
        public Color GizmoColorContdownEnd = new Color(0, .9f, 1f, .5f);

        public float StartPosDistanceFromSTart = 250;
        public PathRef pathRef;
        public Transform Grp_StartLineColliders;
        public Transform objStartLineColliders;
        public Transform objBufferZoneIn;

        public Transform Grp_StartLine_3DModels;

        public float gizmoVehicleGridSphereSize = 2;


        public int HowManyVehicleGridPos = 1;
        public int HowManyVehicleByGridLine = 2;
        public float ForwardDistanceFromOtherOneVehicle = 50;


        public List<Vector3> listOffsetOnGrid = new List<Vector3>();


        Vector3 origin;
        Vector3 origin2;

        public float countdownDuration = 3;
        public float vehicleDistanceEachSecond = 75;

        
       
        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }


        public bool ReturnGridPosition(int whichPosOnGrid,Transform Grp_Vehicle)
        {
            Gizmos.color = GizmoColor;
            if (pathRef)
            {
                Path Track = pathRef.Track;

                int lineCounter = 1;

                //-> Find the grid line
                int count = 0;
                int offsetCounter = 0;
                for (var k = 0; k < whichPosOnGrid; k++)
                {
                    count++;
                    count %= HowManyVehicleByGridLine;
                    if (count == 0)
                        lineCounter++;

                    offsetCounter++;
                    offsetCounter %= listOffsetOnGrid.Count;
                }


                origin = Track.checkpoints[0].position;
                origin2 = Track.checkpoints[1].position;

                Vector3 dir = (origin2 - origin).normalized;

                if (listOffsetOnGrid.Count > 0)
                {
                    Vector3 newPos = origin - dir * (-ForwardDistanceFromOtherOneVehicle * (lineCounter - 1) + countdownDuration * vehicleDistanceEachSecond);
                    

                    Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
                    Vector3 Up = Vector3.Cross(left, Vector3.left).normalized;

                    newPos += left * listOffsetOnGrid[offsetCounter].x + Up * listOffsetOnGrid[offsetCounter].y - dir * listOffsetOnGrid[offsetCounter].z;

                    IVehicleStartLine<Vector3> kPosition = Grp_Vehicle.GetComponent<VehiclePrefabInit>().vehicleInfo.transform.GetComponent<IVehicleStartLine<Vector3>>();
                    IGyroStartLine<Quaternion> kRotation = Grp_Vehicle.GetComponent<VehiclePrefabInit>().vehicleInfo.transform.GetComponent<IGyroStartLine<Quaternion>>();

                    kPosition.InitVehiclePosition(newPos);
                    kRotation.InitVehicleGyroPosition(Quaternion.LookRotation(-dir, -Up));
                    kPosition.InitVehicleOffsetPosition(new Vector3(-listOffsetOnGrid[offsetCounter].x, listOffsetOnGrid[offsetCounter].y,0));
                }
            }
            return true;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = GizmoColor;
            if (pathRef && pathRef.Track.checkpoints.Count > 3)
            {
                float progressDistance = (pathRef.Track.pathLength - StartPosDistanceFromSTart % pathRef.Track.pathLength) % pathRef.Track.pathLength;
                Path Track = pathRef.Track;

                //-> Display the position when the countdown ended.
                Gizmos.color = GizmoColorContdownEnd;

                //-> V2 Line using checkpoint 0 and 1 direction
                origin = Track.checkpoints[0].position;
                origin2 = Track.checkpoints[1].position;
                Vector3 dir2 = (origin2 - origin).normalized;
                Gizmos.DrawLine(origin, origin - dir2 * countdownDuration * vehicleDistanceEachSecond);


                int lineCounter = 0;
                int offsetCounter = 0;
                for (var k = 0; k < HowManyVehicleGridPos; k++)
                {
                    if (listOffsetOnGrid.Count > 0)
                    {
                        if (k % HowManyVehicleByGridLine == 0)
                        {
                            lineCounter++;
                        }

                        Vector3 newPos = origin - dir2 * (-ForwardDistanceFromOtherOneVehicle * (lineCounter-1) + countdownDuration * vehicleDistanceEachSecond);

                        Vector3 left = Vector3.Cross(dir2, Vector3.up).normalized;
                        Vector3 Up = Vector3.Cross(left, Vector3.left).normalized;

                        // Right
                        Gizmos.DrawLine(newPos, newPos + left * 10);
                        // Left
                        Gizmos.DrawLine(newPos, newPos - left * 10);

                        // Up
                        Gizmos.DrawLine(newPos, newPos + Up * 10);
                        // Down
                        Gizmos.DrawLine(newPos, newPos - Up * 10);

                        newPos += left * listOffsetOnGrid[offsetCounter].x + Up * listOffsetOnGrid[offsetCounter].y - dir2 * listOffsetOnGrid[offsetCounter].z;

                        Gizmos.DrawSphere(newPos, gizmoVehicleGridSphereSize / 2);
                        Gizmos.DrawWireSphere(newPos, gizmoVehicleGridSphereSize / 2);

                        offsetCounter++;
                        offsetCounter %= listOffsetOnGrid.Count;
                    }
                }
            }
        }
    }
}
