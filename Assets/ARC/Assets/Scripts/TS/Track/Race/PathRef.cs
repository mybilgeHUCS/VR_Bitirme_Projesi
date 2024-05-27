//Description: PathRef: Access info about track object from any script.
using UnityEngine;

namespace TS.Generics
{
    public class PathRef : MonoBehaviour
    {
        public static PathRef   instance = null;
        public Transform        BonusSpot;
        public Path             Track;
        public GameObject       Grp_AltPathTigger;

        public GameObject       prefabAltPath_Ref;
        public GameObject       prefabCheckpoint;

        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;
        }

    }

}
