//Description: CanvasMainMenuManagerEditor. Custom Editor
#if (UNITY_EDITOR)
using UnityEditor;
using TS.Generics;

[CustomEditor(typeof(CanvasMainMenuManager))]
public class CanvasMainMenuManagerEditor : CanvasManagerEditor
{
    new void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnInspectorGUI()
    {
        #region
        base.OnInspectorGUI();
        #endregion
    }



  

    void OnSceneGUI()
    {
    }

}


#endif
