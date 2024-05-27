// Description: BackAnim: Rotate a list of RectTransform.
using System.Collections.Generic;
using UnityEngine;

namespace TS.Generics
{
    public class BackAnim : MonoBehaviour
    {
        public List<RectTransform> rectList;
        public float speed = 30;

        public void Start()
        {
            RectTransform[] allRect = GetComponentsInChildren<RectTransform>();

            foreach (RectTransform rect in allRect)
            {
                if(rect.name != "Grp" && rect != GetComponent<RectTransform>())
                {
                    rect.localEulerAngles += new Vector3(0, 0, 10);
                    rectList.Add(rect);
                }   
            }
        }


        public void Update()
        {
            foreach (RectTransform rect in rectList)
                rect.localEulerAngles += new Vector3(0,0,Time.deltaTime * speed);
        }
    }

}
