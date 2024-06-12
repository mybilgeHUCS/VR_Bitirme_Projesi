// Desciption: AIBooster
using System.Collections.Generic;
using UnityEngine;


namespace TS.Generics
{
    public class AIBooster : MonoBehaviour 
    {
        [HideInInspector]
        public bool SeeInspector;
        [HideInInspector]
        public bool moreOptions;
        [HideInInspector]
        public bool helpBox = true;

        public List<EditorMethodsList_Pc.MethodsList> methodsList       // Create a list of Custom Methods that could be edit in the Inspector
        = new List<EditorMethodsList_Pc.MethodsList>();

        public CallMethods_Pc callMethods;                              // Access script that allows to call public function in this script.

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<TriggerAIBooster>())
            {
                for (var i = 0; i < methodsList.Count; i++)
                {
                    callMethods.Call_BoolMethod_CheckIfReturnTrue(methodsList, i);
                }
            }
        }
    }
}
