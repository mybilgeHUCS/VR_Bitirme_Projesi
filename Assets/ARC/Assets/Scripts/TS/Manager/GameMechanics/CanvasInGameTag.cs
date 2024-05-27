// Description: CanvasInGameTag: Script used as a tag
using UnityEngine;
using System.Collections.Generic;

public class CanvasInGameTag : MonoBehaviour
{
    public static CanvasInGameTag instance = null;

    public List<GameObject> objList = new List<GameObject>();

    void Awake()
    {
        //-> Check if instance already exists
        if (instance == null)
            instance = this;
    }
}
