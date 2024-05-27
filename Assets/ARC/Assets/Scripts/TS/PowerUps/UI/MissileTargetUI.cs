// Description: MissileTargetUI: Rotate triangle that represent missile target.
using System.Collections.Generic;
using UnityEngine;

public class MissileTargetUI : MonoBehaviour
{
    public Transform        objRef;

    public List<Transform>  listObjToRotate = new List<Transform>();

    public bool             b_EnableRotation = true;
    public float            rotationSpeed = 300;

    
    // Update is called once per frame
    void Update()
    {
        if (objRef && b_EnableRotation)
            objRef.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);

        for (var i = 0;i< listObjToRotate.Count; i++)
        {
            if (listObjToRotate[i] && listObjToRotate[i].gameObject.activeSelf)
            {
                listObjToRotate[i].localRotation = objRef.localRotation;
            }
        }
    }
}
