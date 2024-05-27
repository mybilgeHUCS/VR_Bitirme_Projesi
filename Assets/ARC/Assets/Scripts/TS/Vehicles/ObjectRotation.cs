using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class ObjectRotation : MonoBehaviour
    {
        public Transform trans;
        public float speed = 1000;

        public void Update()
        {
                trans.localEulerAngles += new Vector3(0, 0, Time.deltaTime * speed);
        }
    }

}
