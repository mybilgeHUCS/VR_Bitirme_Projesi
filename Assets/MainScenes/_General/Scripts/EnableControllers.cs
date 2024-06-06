using UnityEngine;

public class EnableControllers : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject[] controllers;


    private void Update()
    {
        SetActiveControllers();
    }

    private void LateUpdate()
    {
        SetActiveControllers();
    }

    public void SetActiveControllers()
    {
        foreach (var item in controllers)
        {
            item.SetActive(true);
        }
    }
}
