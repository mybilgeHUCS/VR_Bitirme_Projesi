// Description: TriggerAltPath. This script is used to save info info about an altenative path
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class TriggerAltPath : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public List<AltPath> AltPathList = new List<AltPath>();                 // List of Altenative that can be used by the AI.

        public Transform checkpointParent;                                      // This TriggerAltPath object is connected to this main path checkpoint.

        public GameObject GrpAltPath;

        public int bestPath = -1;
    }

}
