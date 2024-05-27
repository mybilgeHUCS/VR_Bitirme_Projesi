//Desciption: ButtonFxManager.cs. Methods to change Texte size
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace TS.Generics
{
    public class ButtonFxManager : MonoBehaviour
    { 
        public UnityEvent       OnPointerEnterEvents;
        public UnityEvent       OnPointerExitEvents;

        public void TypoSize0(bool b_Enable = true)
        {
            if (b_Enable) transform.GetChild(0).GetComponent<Text>().fontSize = 20;
        }

        public void TypoSize1(bool b_Enable = true)
        {
            if (b_Enable) transform.GetChild(0).GetComponent<Text>().fontSize = 14;
        }

    }

}
