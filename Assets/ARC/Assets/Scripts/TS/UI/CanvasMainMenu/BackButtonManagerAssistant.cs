// Desciption: BackButtonManagerAssistant: Attached to EventSystem object. 
using UnityEngine;
using TS.Generics;

public class BackButtonManagerAssistant : MonoBehaviour
{
    public bool returnIfRemapIsInProgress()
    {
        if(InputRemapper.instance && InputRemapper.instance.IsRemapInProcess)
            return true;
        else
            return false;
    }

    public bool DisableBackButtonOnPageZero()
    {
        if (CanvasMainMenuManager.instance.currentSelectedPage == 0)
            return true;
        else
            return false;
    }

    public bool ConditionsWhenGameIsPaused()
    {
        if (PauseManager.instance &&
            CanvasMainMenuManager.instance.currentSelectedPage == 1)
            return true;

        else
            return false;
    }
}
