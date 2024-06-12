using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoatAttack
{
    /// <summary>
    /// This is an overall controller for a boat
    /// </summary>
    public class Boat : MonoBehaviour
    {
        // Boat stats
        public Renderer boatRenderer; // The renderer for the boat mesh
        public Renderer engineRenderer; // The renderer for the boat mesh
        public Engine engine;
        private Matrix4x4 _spawnPosition;

        // RaceStats
        [NonSerialized] public int Place = 0;
        [NonSerialized] public float LapPercentage;
        [NonSerialized] public int LapCount;
        [NonSerialized] public bool MatchComplete;
        private int _wpCount = -1;

        [NonSerialized] public readonly List<float> SplitTimes = new List<float>();

        private float _camFovVel;
        private Object _controller;
        private int _playerIndex;

        // Shader Props
        private static readonly int LiveryPrimary = Shader.PropertyToID("_Color1");
        private static readonly int LiveryTrim = Shader.PropertyToID("_Color2");

        // debug
        [SerializeField] internal bool debugControl = false;

        private void Awake()
		{
            if (debugControl)
            {
                Setup(1, true);
            }
            _spawnPosition = transform.localToWorldMatrix;
            TryGetComponent(out engine.RB);
        }

        public void Setup(int player = 1, bool isHuman = true)
        {
            _playerIndex = player - 1;
            SetupController(isHuman); // create or change controller
        }

        void SetupController(bool isHuman)
        {
            var controllerType =  typeof(HumanController) ;
            // If controller exists then make sure it's teh right one, if not add it
            if (_controller)
            {
                if (_controller.GetType() == controllerType) return;
                Destroy(_controller);
                _controller = gameObject.AddComponent(controllerType);
            }
            else
            {
                _controller = gameObject.AddComponent(controllerType);
            }
        }

      


      

        





        /// <summary>
        /// This sets both the primary and secondary colour and assigns via a MPB
        /// </summary>


    }

    [Serializable]
    public class BoatData
    {
        public string boatName;
        public bool human;
        [NonSerialized] public Boat Boat;
        [NonSerialized] public GameObject BoatObject;

        public void SetController(GameObject boat, Boat controller)
        {
            BoatObject = boat;
            this.Boat = controller;
        }
    }

}
