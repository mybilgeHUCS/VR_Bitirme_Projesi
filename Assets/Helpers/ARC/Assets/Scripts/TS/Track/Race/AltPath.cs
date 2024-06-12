// Description: AltPath: Manage Alt path
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class AltPath : MonoBehaviour
    {
        [HideInInspector]
        public bool                     SeeInspector;
        [HideInInspector]
        public bool                     moreOptions;
        [HideInInspector]
        public bool                     helpBox = true;

        public Transform                checkpointStart;
        public Transform                checkpointEnd;
        public List<Transform>          tmpCheckpoints = new List<Transform>();

        public List<Transform>          checkpoints = new List<Transform>();

        [HideInInspector]
        public List<Vector3>            checkpointsPosition = new List<Vector3>();
        [HideInInspector]
        public List<float>              checkpointsDistanceFromPathStart = new List<float>();

        public float                    pathLength;

        private List<int>               checkpointsIDList = new List<int>() { 0, 0, 0, 0 };

        public int                      curveSmoothness = 100;

        // Show vehicle path that use offset
        [System.Serializable]
        public class AdditionalPath
        {
            public Vector3  offset;
            public Color    color = Color.red;
            public bool     b_Show = true;
        }

        public List<AdditionalPath>     additionalPathsList = new List<AdditionalPath>();

        public float                    distanceToRemove = 200;


        public TriggerAltPath           triggerAltPath;                                    // The alternative path is connected to this triggerAltPath on th Hierarchy.

        // Use this for initialization
        private void Awake()
        {
            checkpoints = new List<Transform>(ReturnCheckPointsList());
        }


        // Use to find the new position of the AI target
        public Vector3 TargetPositionOnPath(float dist)
        {
            return PositionOnPath(Mathf.Clamp(dist, 0, pathLength));
        }
        // Use to find the new rotation of the AI target
        public Vector3 TargetRotationOnPath(float dist, bool invertY = false)
        {
            Vector3 p1 = PositionOnPath(Mathf.Clamp(dist, 0, pathLength));
            Vector3 p2 = PositionOnPath(Mathf.Clamp(dist + 0.1f, 0, pathLength));
            Vector3 delta = p2 - p1;
            if (invertY) delta = p1 - p2;
            return delta.normalized;
        }

        public void CreateLists()
        {
            // Clear list of checkpoints positions and distance from the start of the path
            checkpointsPosition = new List<Vector3>();
            checkpointsDistanceFromPathStart = new List<float>();

            float distanceFromStart = 0;
            for (int i = 0; i < checkpoints.Count + 1; ++i)
            {
                if (checkpoints[i % checkpoints.Count] && checkpoints[(i + 1) % checkpoints.Count])
                {
                    Vector3 checkpoint1 = checkpoints[i % checkpoints.Count].position;
                    Vector3 checkpoint2 = checkpoints[(i + 1) % checkpoints.Count].position;
                    // Save the position of each checkpoint
                    checkpointsPosition.Add(checkpoints[i % checkpoints.Count].position);
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
            checkpointsIDList[0] = (pathSegment - 2 + checkpoints.Count) % checkpoints.Count;
            checkpointsIDList[1] = (pathSegment - 1 + checkpoints.Count) % checkpoints.Count;
            checkpointsIDList[2] = pathSegment;
            checkpointsIDList[3] = (pathSegment + 1) % checkpoints.Count;


            float clampedDist = Mathf.Clamp(dist, checkpointsDistanceFromPathStart[checkpointsIDList[1]], checkpointsDistanceFromPathStart[checkpointsIDList[2]]);
            // Scale the distance between 0 and 1. It gives the distance from checkpointsIDList[1] to the point we want to find.
            float scaledDistP1P2 = (clampedDist - checkpointsDistanceFromPathStart[checkpointsIDList[1]]) / (checkpointsDistanceFromPathStart[checkpointsIDList[2]] - checkpointsDistanceFromPathStart[checkpointsIDList[1]]);

            checkpointsIDList[2] %= checkpoints.Count;

            return CatmullRom(checkpointsPosition[checkpointsIDList[0]], checkpointsPosition[checkpointsIDList[1]], checkpointsPosition[checkpointsIDList[2]], checkpointsPosition[checkpointsIDList[3]], scaledDistP1P2);
        }


        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float scaledDistP1P2)
        {
            return 0.5f *
                   ((2 * p1) + (-p0 + p2) * scaledDistP1P2 + (2 * p0 - 5 * p1 + 4 * p2 - p3) * scaledDistP1P2 * scaledDistP1P2 +
                    (-p0 + 3 * p1 - 3 * p2 + p3) * scaledDistP1P2 * scaledDistP1P2 * scaledDistP1P2);
        }




        private void OnDrawGizmos()
        {
            if (!Application.isPlaying && checkpointStart && checkpointEnd)
            {
                checkpoints = new List<Transform>(ReturnCheckPointsList());
            }


             if (CheckpointAvailable() && checkpoints.Count > 3)
            {
                CreateLists();
                pathLength = checkpointsDistanceFromPathStart[checkpointsDistanceFromPathStart.Count - 1];

                int startPos = 0;
                for(var k = 0; k < checkpoints.Count; k++)
                {
                    if(tmpCheckpoints[0] == checkpoints[k])
                    {
                        startPos = k;
                        break;
                    }
                }
                int endPos = 0;
                for (var k = 0; k < checkpoints.Count; k++)
                {
                    if (tmpCheckpoints[tmpCheckpoints.Count-1] == checkpoints[k])
                    {
                        endPos = k;
                        break;
                    }
                }

                //Debug.Log("Pos: " + startPos);
                Vector3 prev = checkpoints[startPos -1].position;
                int counter = 0;
                Vector3 FirstPosition = Vector3.zero ;
                for (float dist = 0; dist < pathLength; dist += pathLength / curveSmoothness)
                {
                    Vector3 next = PositionOnPath(Mathf.Clamp(dist + 1, 0, pathLength));
                    if (dist > checkpointsDistanceFromPathStart[(checkpoints.Count + (startPos - 1)) % checkpoints.Count] && dist < checkpointsDistanceFromPathStart[(endPos + 1)])
                    {
                        Gizmos.color = Color.yellow;

                        if (counter == 0)
                        {
                            FirstPosition = next;
                        }

                        Gizmos.DrawLine(prev, next);

                        counter++;
                        prev = next;
                    }
                    else if(dist >= checkpointsDistanceFromPathStart[(startPos + 1)])
                    {
                        break;
                    }
                }
               // Debug.Log("FirstPosition: " + FirstPosition);
                Gizmos.DrawLine(checkpoints[startPos -1].position, FirstPosition);
                Gizmos.DrawLine(prev, checkpoints[endPos + 1].position);

                for (var i = 0; i < additionalPathsList.Count; i++)
                {
                    if (additionalPathsList[i].b_Show)
                    {
                        Vector3 prevAlt = checkpoints[startPos - 1].position;
                        Vector3 offset = additionalPathsList[i].offset;
                        Vector3 pos = new Vector3();
                        counter = 0;
                        FirstPosition = Vector3.zero;
                        for (float dist = 0; dist < pathLength; dist += pathLength / curveSmoothness)
                        {
                            Vector3 nextAlt = PositionOnPath(Mathf.Clamp(dist + 1, 0, pathLength));

                            if (dist > checkpointsDistanceFromPathStart[(checkpoints.Count + (startPos - 1)) % checkpoints.Count] && dist < checkpointsDistanceFromPathStart[(endPos + 1)])
                            {
                                Gizmos.color = Color.yellow;

                                if (counter == 0)
                                {
                                    FirstPosition = nextAlt;
                                }
                                Gizmos.color = additionalPathsList[i].color;

                                Vector3 dir = (nextAlt - prevAlt).normalized;
                                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

                                Vector3 Up = Vector3.Cross(left, dir).normalized;

                                pos = left * offset.x + Up * offset.y + dir * offset.z;

                                Gizmos.DrawLine(prevAlt + pos, nextAlt + pos);
                                prevAlt = nextAlt;

                                counter++;
                            }
                            else if (dist >= checkpointsDistanceFromPathStart[(startPos + 1)])
                            {
                                break;
                            }
                        }
                        Gizmos.DrawLine(checkpoints[startPos - 1].position + pos, FirstPosition + pos);
                        Gizmos.DrawLine(prevAlt + pos, checkpoints[endPos + 1].position + pos);
                    }
                }
            }
        }

        // Prevent bug if checkpoints are missing in the checkpoint array
        bool CheckpointAvailable()
        {
            for (var i = 0; i < checkpoints.Count; i++)
            {
                if (!checkpoints[i])
                    return false;
            }
            return true;
        }

        public float DistanceFromCheckpointToStart(int checkpointID)
        {
            //Debug.Log("checkpointID: " + checkpointID);
            return checkpointsDistanceFromPathStart[checkpointID];
        }


        List<Transform> ReturnCheckPointsList()
        {
            checkpoints.Clear();

            if (checkpointStart == null)
            {
                Debug.Log("checkpointStart is missing. Check object " + gameObject.name + " to solve the problem.");
                return new List<Transform>();
            }


            List<Transform> checkpointsRef = new List<Transform>(checkpointStart.parent.GetComponent<Path>().checkpoints);

            for (var i = 0; i < checkpointsRef.Count; i++)
            {
                if (checkpointsRef[i] == null)
                {
                    Debug.Log("A checkpoint is missing. Check the main path to solve the problem.");
                    return new List<Transform>();
                }
            }

            int counter = 0;
            // Find where the Alt Path starts on the Main Path Point
            for (var i = 0; i < checkpointsRef.Count; i++)
            {
                if (checkpointsRef[i] == checkpointStart)
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
            for (var i = counter; i < checkpointsRef.Count; i++)
            {
                if (checkpointsRef[i] == checkpointEnd)
                {
                    break;
                }
                else
                {
                    HowManyPointsBetweenTheTwoPoints++;
                }
            }

            // Remove checkpoints that are not needed
            for (var i = 0; i < HowManyPointsBetweenTheTwoPoints; i++)
            {
                //Debug.Log(i + " : " + checkpointsRef[counter].name);
                checkpointsRef.RemoveAt(counter);
            }

            // Add Alt Path Point to the list of checkpoints
            for (var i = 0; i < checkpointsRef.Count; i++)
            {
                if (checkpointsRef[i] == checkpointStart)
                {
                    for (var k = 0; k < tmpCheckpoints.Count; k++)
                    {
                        checkpointsRef.Insert(i + 1 + k, tmpCheckpoints[k]);
                    }
                    break;
                }
            }

            return new List<Transform>(checkpointsRef);
        }
    }
}


