using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace TS.ARC
{
    public class WarningSignal : MonoBehaviour
    {
        private CanvasGroup cG;
        public int ID;
        [HideInInspector]
        public float warningSpeed = 1;
        public float offset = 2;
        public float warningMaxSpeed = 4;
        private int target;

        // Start is called before the first frame update
        void Start()
        {
            cG = GetComponent<CanvasGroup>();
        }


        // Update is called once per frame
        void Update()
        {
            if (gameObject.activeSelf)
            {
                cG.alpha = Mathf.MoveTowards(cG.alpha, target, Time.deltaTime * (warningSpeed+ offset) * warningMaxSpeed);
                if (cG.alpha == 0) target = 1;
                else if (cG.alpha == 1) target = 0;
            }
                
        }
    }

}
