using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    [CreateAssetMenu(fileName = "layersListData", menuName = "TS/layersListData")]
    public class LayersListData : ScriptableObject
    {
        [System.Serializable]
        public class LayerInfo
        {
            public string name;
            public int layerID;
        }
        public List<LayerInfo> listLayerInfo = new List<LayerInfo>();
    }
}

