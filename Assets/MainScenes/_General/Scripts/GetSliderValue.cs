using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;


    public void Update()
    {
        text.text = slider.value.ToString();
    }
}
