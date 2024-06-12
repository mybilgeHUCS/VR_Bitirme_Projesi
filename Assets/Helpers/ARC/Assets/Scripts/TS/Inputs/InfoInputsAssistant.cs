// Description: InfoInputsAssistant. Useful method to use in association with InfoInputs
using UnityEngine;

namespace TS.Generics
{
    public class InfoInputsAssistant : MonoBehaviour
    {
        public float MobileInputExample()
        {
            if (Input.GetKey(KeyCode.E))
                return 1;
            else
                return 0;
        }

        public float OtherInputExample()
        {
            if (Input.GetKey(KeyCode.F))
                return 1;
            else
                return 0;
        }
    }

}
