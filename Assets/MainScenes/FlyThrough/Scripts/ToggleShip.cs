using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShip : MonoBehaviour
{
    public GameObject[] shipParts;

    private void Start()
    {
        bool isPlaneVisible = PlayerPrefs.GetFloat("CockpitVisibilityToggle") == 1f ? true : false;

        for (int i = 0; i < shipParts.Length; i++)
        {
            if (isPlaneVisible)
            {
                //shipParts[i].gameObject.SetActive(true);

                foreach (var item in shipParts[i].GetComponentsInChildren<MeshRenderer>())
                {
                    item.enabled = true;
                }
            }
            else
            {
                foreach (var item in shipParts[i].GetComponentsInChildren<MeshRenderer>())
                {
                    item.enabled = false;
                }
                //shipParts[i].gameObject.SetActive(false);
            }
        }

    }

    /*public void shipPartToggle()
    {
        for (int i = 0; i < shipParts.Length; i++)
        {
            if (shipParts[i].activeInHierarchy == true)
            {
                shipParts[i].gameObject.SetActive(false);
            }
            else if (shipParts[i].activeInHierarchy == false)
            {
                shipParts[i].gameObject.SetActive(true);
            }
        }
    }*/
}
