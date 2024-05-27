// Description: PagePauseAssistant
using UnityEngine;

namespace TS.Generics
{
    public class PagePauseAssistant : MonoBehaviour
    {
        public void DisablePausePage()
        {
            PauseManager.instance.GetComponent<DisplayPauseMenu>().OnPauseAction();
            InfoPlayerTS.instance.b_IsPageCustomPartInProcess = false;
        }
    }
}

