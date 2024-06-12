// Description: GrpAIBooster: Attached to Grp_Triggers_AIBooster
// Allows to create an AI booster (Trigger) in the Hierarchy
using UnityEngine;

namespace TS.Generics
{
    public class GrpAIBooster : MonoBehaviour
    {
        public GameObject   trigger_AIBooster;

        [HideInInspector]
        public bool         SeeInspector;
        [HideInInspector]
        public bool         moreOptions;
        [HideInInspector]
        public bool         helpBox = true;
    }
}