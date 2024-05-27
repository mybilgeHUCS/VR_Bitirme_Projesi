using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleShip : MonoBehaviour
{
    public GameObject[] shipParts;

    public void shipPartToggle()
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
    }
}
