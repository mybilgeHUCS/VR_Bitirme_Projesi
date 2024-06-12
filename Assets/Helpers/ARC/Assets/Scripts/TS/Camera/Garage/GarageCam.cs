using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class GarageCam : MonoBehaviour
    {
        public List<Transform>      posList = new List<Transform>();
        public Transform            target;
        public Transform            Camera;
        private int                  currentTextTarget = 0;
        public float                durationBetweenTwoPoints = 3;

        public AnimationCurve       animCurve;

        public bool                 isAvailable = true;

        void Start()
        {
            if(isAvailable)
                StartCoroutine(MoveCamRoutine());
        }

        /*private void Update()
        {
            Camera.transform.LookAt(target);
        }*/

        IEnumerator MoveCamRoutine()
        {
 
            float t = 0;

            while (t < 1)
            {
                t += Time.deltaTime / durationBetweenTwoPoints;
                Camera.transform.position = Vector3.Lerp(posList[currentTextTarget].position, posList[(currentTextTarget + 1) % posList.Count].position, animCurve.Evaluate(t));
                Camera.transform.LookAt(target);
                yield return null;
            }

            currentTextTarget ++;
            currentTextTarget %= posList.Count;

            StartCoroutine(MoveCamRoutine());
            yield return null;
        }
    }

}
