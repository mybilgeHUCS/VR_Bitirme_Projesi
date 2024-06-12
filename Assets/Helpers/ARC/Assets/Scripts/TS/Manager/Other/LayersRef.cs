// Description: LayersRef: Attached to LayersRef object. Reference to layer set up in Unity Project Settings
using UnityEngine;

namespace TS.Generics
{
    public class LayersRef : MonoBehaviour
    {
        public static LayersRef instance = null;

        [HideInInspector]
        public bool             SeeInspector;
        [HideInInspector]
        public bool             moreOptions;
        [HideInInspector]
        public bool             helpBox = true;

        public LayersListData   layersListData;


        void Awake()
        {
            //-> Check if instance already exists
            if (instance == null)
                instance = this;

            //-> If instance already exists and it's not this:
            else if (instance != this)
                Destroy(gameObject);
        }
    }
}

