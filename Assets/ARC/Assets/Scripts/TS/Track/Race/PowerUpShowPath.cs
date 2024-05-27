// Description: PowerUpShowPath: Display Power-up path  
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class PowerUpShowPath : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public Path path;
        public GrpPowerUp grpPowerUp;

        public List<Transform> PathPoints = new List<Transform>();
        public List<Vector3> checkpointsPosition = new List<Vector3>();
        public List<float> checkpointsDistanceFromPathStart = new List<float>();

        public float pathLength;

        private List<int> checkpointsIDList = new List<int>() { 0, 0, 0, 0 };

        public int curveSmoothness = 100;

        public void Start()
        {
            PathPoints.Clear();

            int CheckpointID = 0;

            if (path && grpPowerUp)
            {
                for (var i = 0; i < path.checkpoints.Count; i++)
                {
                    if (path.checkpoints[i] == grpPowerUp.objInsertAfter)
                    {
                        CheckpointID = i;
                        break;
                    }
                }

                int CheckpointIDToAdd = (CheckpointID - 1 + path.checkpoints.Count) % path.checkpoints.Count;
                PathPoints.Add(path.checkpoints[CheckpointIDToAdd]);

                PathPoints.Insert(PathPoints.Count, grpPowerUp.objInsertAfter);
                PathPoints.Insert(PathPoints.Count, this.transform);

                CheckpointIDToAdd = (CheckpointID + 1 + path.checkpoints.Count) % path.checkpoints.Count;
                PathPoints.Insert(PathPoints.Count, path.checkpoints[CheckpointIDToAdd]);

                CheckpointIDToAdd = (CheckpointID + 2 + path.checkpoints.Count) % path.checkpoints.Count;
                PathPoints.Insert(PathPoints.Count, path.checkpoints[CheckpointIDToAdd]);

                CheckpointIDToAdd = (CheckpointID + 3 + path.checkpoints.Count) % path.checkpoints.Count;
                PathPoints.Insert(PathPoints.Count, path.checkpoints[CheckpointIDToAdd]);
            }

            if (PathPointsAvailable() && PathPoints.Count > 3 && grpPowerUp)
            {
                CreateLists();
            }
        }



        private void OnDrawGizmos()
        {
            if (!Application.isPlaying && path && grpPowerUp)
            {
                List<Transform> checkpointList = new List<Transform>();
                for (var i = 0; i < grpPowerUp.listLookAtPowerUp.Count; i++)
                {
                    if (this.transform == grpPowerUp.listLookAtPowerUp[i].powerUpToLookAT)
                    {
                        PathPoints = new List<Transform>(grpPowerUp.listLookAtPowerUp[i].checkpointsRef);
                    }
                }

                if (PathPointsAvailable() && PathPoints.Count > 3 && grpPowerUp)
                {
                    CreateLists();
                    pathLength = checkpointsDistanceFromPathStart[checkpointsDistanceFromPathStart.Count - 1];

                    int startPos = 0;
                    for (var k = 0; k < PathPoints.Count; k++)
                    {
                        if (this.transform == PathPoints[k])
                        {
                            startPos = k;
                            break;
                        }
                    }
                    //Debug.Log("Pos: " + startPos);
                    Vector3 prev = PathPoints[startPos - 1].position;
                    int counter = 0;
                    Vector3 FirstPosition = Vector3.zero;
                    for (float dist = 0; dist < pathLength; dist += pathLength / curveSmoothness)
                    {
                        Vector3 next = PositionOnPath(Mathf.Clamp(dist + 1, 0, pathLength));
                        if (dist > checkpointsDistanceFromPathStart[(PathPoints.Count + (startPos - 1)) % PathPoints.Count] && dist < checkpointsDistanceFromPathStart[(startPos + 1)])
                        {
                            Gizmos.color = Color.green;

                            if (counter == 0)
                            {
                                FirstPosition = next;
                            }

                            Gizmos.DrawLine(prev, next);

                            counter++;
                            prev = next;
                        }
                        else if (dist >= checkpointsDistanceFromPathStart[(startPos + 1)])
                        {
                            break;
                        }

                    }
                    // Debug.Log("FirstPosition: " + FirstPosition);
                    Gizmos.DrawLine(PathPoints[startPos - 1].position, FirstPosition);
                    Gizmos.DrawLine(prev, PathPoints[startPos + 1].position);
                }
            }
        }

        bool PathPointsAvailable()
        {
            for (var i = 0; i < PathPoints.Count; i++)
            {
                if (!PathPoints[i])
                    return false;
            }
            return true;
        }

        void CreateLists()
        {
            // Clear list of checkpoints positions and distance from the start of the path
            checkpointsPosition = new List<Vector3>();
            checkpointsDistanceFromPathStart = new List<float>();

            float distanceFromStart = 0;
            for (int i = 0; i < PathPoints.Count; ++i)
            {
                if (PathPoints[i % PathPoints.Count] && PathPoints[(i + 1) % PathPoints.Count])
                {
                    Vector3 checkpoint1 = PathPoints[i % PathPoints.Count].position;
                    Vector3 checkpoint2 = PathPoints[(i + 1) % PathPoints.Count].position;
                    // Save the position of each checkpoint
                    checkpointsPosition.Add(PathPoints[i % PathPoints.Count].position);
                    // Save the distance from the start for each checkpoint
                    checkpointsDistanceFromPathStart.Add(distanceFromStart);
                    Vector3 diff = checkpoint1 - checkpoint2;
                    distanceFromStart += diff.magnitude;
                }
            }
        }

        //-> Determine a point position on the track using CatmullRom equation
        public Vector3 PositionOnPath(float dist, int pathSegment = 0)
        {
            for (var i = 0; i < checkpointsDistanceFromPathStart.Count; i++)
            {
                //-> Find the closest checkpoint to the position we want to find on the path.
                if (checkpointsDistanceFromPathStart[i] < dist) pathSegment++;
                else break;
            }

            //-> Find the four checkpoints needed to use catmulRom equation
            checkpointsIDList[0] = (pathSegment - 2 + PathPoints.Count) % PathPoints.Count;
            checkpointsIDList[1] = (pathSegment - 1 + PathPoints.Count) % PathPoints.Count;
            checkpointsIDList[2] = pathSegment;
            checkpointsIDList[3] = (pathSegment + 1) % PathPoints.Count;

           
            float clampedDist = Mathf.Clamp(dist, checkpointsDistanceFromPathStart[checkpointsIDList[1]], checkpointsDistanceFromPathStart[checkpointsIDList[2]]);
            // Scale the distance between 0 and 1. It gives the distance from checkpointsIDList[1] to the point we want to find.
            float scaledDistP1P2 = (clampedDist - checkpointsDistanceFromPathStart[checkpointsIDList[1]]) / (checkpointsDistanceFromPathStart[checkpointsIDList[2]] - checkpointsDistanceFromPathStart[checkpointsIDList[1]]);

            checkpointsIDList[2] %= PathPoints.Count;

            return CatmullRom(checkpointsPosition[checkpointsIDList[0]], checkpointsPosition[checkpointsIDList[1]], checkpointsPosition[checkpointsIDList[2]], checkpointsPosition[checkpointsIDList[3]], scaledDistP1P2);
        }


        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float scaledDistP1P2)
        {
            return 0.5f *
                   ((2 * p1) + (-p0 + p2) * scaledDistP1P2 + (2 * p0 - 5 * p1 + 4 * p2 - p3) * scaledDistP1P2 * scaledDistP1P2 +
                    (-p0 + 3 * p1 - 3 * p2 + p3) * scaledDistP1P2 * scaledDistP1P2 * scaledDistP1P2);
        }

    }

}
