//Description: LimitCollider: Gizmos
using UnityEngine;
using System.Collections.Generic;



namespace TS.Generics
{
    public class LimitCollider : MonoBehaviour
    {
        [HideInInspector]
        public int spots;

        public float height = 3;

        public Vector3[] vertices;

        public Mesh mesh;

        public MeshCollider meshCollider;

        public List<Transform> posList = new List<Transform>();

        public Path path;
        public GameObject refSpot;

        public bool b_EditMode;

        public bool editorTab;
        public GameObject objSingleLimit;

        [System.Serializable]
        public class MeshInfo
        {
            public string name = "";
            public GameObject objMesh;
            public bool bShow = true;
            public bool bGenerate = false;
            public float distanceToPath = 100;
            public Mesh mesh;
        }

        public List<MeshInfo> meshList = new List<MeshInfo>();

       

        private void OnDrawGizmos()
        {
            if (vertices == null)
            {
                return;
            }

            Gizmos.color = Color.black;
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], 0.1f);
            }
        }
    }

}
