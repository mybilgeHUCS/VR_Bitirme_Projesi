﻿using UnityEngine;

namespace BoatAttack
{
    /// <summary>
    /// This sends input controls to the boat engine if 'Human'
    /// </summary>
    public class HumanController : BaseController
    {

        public float _throttle;
        public float _steering;
        

        public override void OnEnable()
        {
            base.OnEnable();
        }

        private void Update() {
            //Debug.Log(_throttle  + " "+ _steering);
            _steering = Input.GetAxis("Horizontal");
            _throttle = Input.GetAxis("Vertical");
        }

        void FixedUpdate()
        {
            engine.Accelerate(_throttle);
            engine.Turn(_steering);
        }
    }
}

