using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InScreenPanelManager : MonoBehaviour
{
    public GameObject ScrollBarObject;
    public GameObject TextObject;

    private Scrollbar scrollbar;
    private TMP_Text text;
    private int value;
    // Start is called before the first frame update
    void Start()
    {
        scrollbar = ScrollBarObject.GetComponent<Scrollbar>();
        text = TextObject.GetComponent<TMP_Text>();

        value = 0;
        text.text = "0";
    }

    // Update is called once per frame
    void Update()
    {

    }
}
