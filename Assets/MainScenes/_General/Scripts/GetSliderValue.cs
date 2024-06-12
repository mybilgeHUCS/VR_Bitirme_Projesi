using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI textBox;


    public void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }
        textBox.text = slider.value.ToString();
    }
}
