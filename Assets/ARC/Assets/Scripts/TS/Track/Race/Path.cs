// Description: Path. Info about the path of the track.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class Path : MonoBehaviour
    {
        [HideInInspector]
        public List<Texture2D>          listTex = new List<Texture2D>();
        [HideInInspector]
        public List<GUIStyle>           listGUIStyle = new List<GUIStyle>();
        [HideInInspector]
        public bool                     SeeInspector;
        [HideInInspector]
        public bool                     moreOptions;
        [HideInInspector]
        public bool                     helpBox = true;
        [HideInInspector]
        public int                      currentSelectedCheckpoint;
        [HideInInspector]
        public bool                     showCheckpoints = true;

        //public int                      pathID;
        public List<Transform>          checkpointsRef = new List<Transform>();                 // Use to remember the default Main Path
        public List<Transform>          checkpoints = new List<Transform>();                    // The current checkpoints use as path for the player or AI.
        [HideInInspector]
        public List<Vector3>            checkpointsPosition = new List<Vector3>();              // The position of each checkpoint of checkpoints list (current path for AI |Players)
        [HideInInspector]
        public List<float>              checkpointsDistanceFromPathStart = new List<float>();   // The distance from start for each checkpoint of checkpoints list (current path for AI |Players)

        public float                    pathLength;                                             // The lenght of the current path

        private List<int>               checkpointsIDList = new List<int>() { 0,0,0,0};         // Use for the CatmullRom calculation

        public int                      curveSmoothness = 100;                                  // Use to have smoother representation of the track path curve

        public List<AltPath>            AltPathList = new List<AltPath>();                      // References to Alternative path connected to the main path

        public GameObject               prefabCheckpoint;                                       // Use as reference prefab when a new checkpoint is added to the path using the Inspector Editor 

        public float                    gizmoCheckpointSize = 10;
        public bool                     gizmoShowPath = true;


        // Show vehicle path that use offset
        [System.Serializable]
        public class AdditionalPath
        {
            public Vector3  offset;
            public Color    color = Color.red;
            public bool     b_Show = true;
        }
            
        public List<AdditionalPath> additionalPathsList = new List<AdditionalPath>();

       
        // Use this for initialization
        private void Awake()
        {
            if (CheckpointAvailable() && checkpoints.Count > 3)
            {
                checkpointsRef = new List<Transform>(checkpoints);
                CreateLists();
            }
            else Debug.Log("Checkpoints are missing in this path: " + this.name);
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
            if(invertY) delta = p1 - p2;
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

            pathLength = checkpointsDistanceFromPathStart[checkpointsDistanceFromPathStart.Count - 1];
        }


        //-> Determine a point position on the track using CatmullRom equation
        public Vector3 PositionOnPath(float dist,int _checkpoint = 0)
        {
            for (var i = 0; i < checkpointsDistanceFromPathStart.Count; i++)
            {
                //-> Find the closest checkpoint to the position we want to find on the path.
                if (checkpointsDistanceFromPathStart[i] < dist) _checkpoint++;
                else break;
            }

            //-> Find the four checkpoints needed to use catmulRom equation
            checkpointsIDList[0] = (_checkpoint - 2 + checkpoints.Count) % checkpoints.Count;
            checkpointsIDList[1] = (_checkpoint - 1 + checkpoints.Count) % checkpoints.Count;
            checkpointsIDList[2] = _checkpoint;
            checkpointsIDList[3] = (_checkpoint + 1) % checkpoints.Count;


            float clampedDist = Mathf.Clamp(dist, checkpointsDistanceFromPathStart[checkpointsIDList[1]], checkpointsDistanceFromPathStart[checkpointsIDList[2]]);
            // Scale the distance between 0 and 1. It gives the distance from checkpointsIDList[1] to the point we want to find.
            float scaledDistP1P2 = (clampedDist - checkpointsDistanceFromPathStart[checkpointsIDList[1]]) /  (checkpointsDistanceFromPathStart[checkpointsIDList[2]] - checkpointsDistanceFromPathStart[checkpointsIDList[1]]);

            checkpointsIDList[2] %= checkpoints.Count;

            return CatmullRom(checkpointsPosition[checkpointsIDList[0]], checkpointsPosition[checkpointsIDList[1]], checkpointsPosition[checkpointsIDList[2]], checkpointsPosition[checkpointsIDList[3]], scaledDistP1P2);
        }


        private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float scaledDistP1P2)
        {
            return 0.5f * ((2 * p1) + (-p0 + p2) * scaledDistP1P2 + (2 * p0 - 5 * p1 + 4 * p2 - p3) * scaledDistP1P2 * scaledDistP1P2 + (-p0 + 3 * p1 - 3 * p2 + p3) * scaledDistP1P2 * scaledDistP1P2 * scaledDistP1P2);
        }



        //-> This section draw a line to represent the track (Scene view)
        //-> + It draws line that represent the limits of the AI movement
        private void OnDrawGizmos()
        {
            if (gizmoShowPath)
            {
                //-> Track path is drawned only if there are at least 3 checkpoints and no checkpoint are missing in the list
                if (CheckpointAvailable() && checkpoints.Count > 3)
                {
                    //-> In edit Mode generate the checkpoint list
                    if (!Application.isPlaying)
                    {
                        CreateLists();
                        pathLength = checkpointsDistanceFromPathStart[checkpointsDistanceFromPathStart.Count - 1];
                    }

                    //-> Draw the main path
                    Vector3 prev = checkpoints[0].position;

                    for (float dist = 0; dist < pathLength; dist += pathLength / curveSmoothness)
                    {
                        Vector3 next = PositionOnPath(Mathf.Clamp(dist + 1, 0, pathLength));
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawLine(prev, next);
                        prev = next;
                    }
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(prev, checkpoints[0].position);

                    for (var i = 0; i < checkpoints.Count; i++)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(checkpoints[i].position, 3);
                    }


                    //-> Draw the path AI limit.
                    // Those limit are useful to know if the AI vehicles will no enter in collision with walls.
                    for (var i = 0; i < additionalPathsList.Count; i++)
                    {
                        if (additionalPathsList[i].b_Show)
                        {
                            Vector3 prevAlt = checkpoints[0].position;
                            Vector3 offset = additionalPathsList[i].offset;
                            Vector3 pos = new Vector3();
                            for (float dist = 0; dist < pathLength; dist += pathLength / curveSmoothness)
                            {
                                Vector3 nextAlt = PositionOnPath(Mathf.Clamp(dist + 1, 0, pathLength));
                                Gizmos.color = additionalPathsList[i].color;

                                Vector3 dir = (nextAlt - prevAlt).normalized;
                                Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;

                                Vector3 Up = Vector3.Cross(left, dir).normalized;

                                pos = left * offset.x + Up * offset.y + dir * offset.z;

                                Gizmos.DrawLine(prevAlt + pos, nextAlt + pos);
                                prevAlt = nextAlt;
                            }
                            Gizmos.DrawLine(prevAlt + pos, checkpoints[0].position + pos);
                        }
                    }
                }
            }
        }

        //-> Prevent bug if checkpoints are missing in the checkpoint array
        bool CheckpointAvailable()
        {
            for(var i = 0; i< checkpoints.Count; i++)
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
    }
}
   

