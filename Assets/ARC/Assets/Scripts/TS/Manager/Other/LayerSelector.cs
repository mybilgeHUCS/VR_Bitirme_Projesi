// Description: LayerSelector: When the scene starts, select layer for the object with this script attached to it
using UnityEngine;


namespace TS.Generics
{
    public class LayerSelector : MonoBehaviour
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public int layerName = 0;


        // Start is called before the first frame update
        void Start()
        {
            LayersRef objLayerRef = FindObjectOfType<LayersRef>();

            if (objLayerRef)
            {
                this.gameObject.layer = objLayerRef.layersListData.listLayerInfo[layerName].layerID;
            }
        }
    }
}

