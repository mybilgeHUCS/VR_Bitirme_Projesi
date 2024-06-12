// Description: CarPageAssistantOut: Method called when the car page is closed (Main Menu).
using UnityEngine;

namespace TS.Generics
{
    public class CarPageAssistantOut : MonoBehaviour
    {
        public void StateGrpCamP1(bool value)
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            CarSelectionManager.instance.StateGrpCamP1(value);
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
        public void StateGrpCamP2(bool value)
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            CarSelectionManager.instance.StateGrpCamP2(value);
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }

        public void EnterCarSelection()
        {
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = true;
            CarSelectionManager.instance.EnterCarSelection();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }
}