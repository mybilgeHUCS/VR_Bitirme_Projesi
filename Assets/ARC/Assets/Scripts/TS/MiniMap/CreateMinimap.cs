// Description: CreateMinimap: Used to generate the minimap 
using UnityEngine;

namespace TS.Generics
{
    public class CreateMinimap : MonoBehaviour
    {
        public bool         SeeInspector;
        public bool         moreOptions;
        public bool         helpBox = false;

        public GameObject   grpPath;

        public LineRenderer lineRenderer;
        public Camera       cam;

    }

}
