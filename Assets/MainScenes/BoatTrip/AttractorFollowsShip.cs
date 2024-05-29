using UnityEngine;

public class AttractorFollowsShip : MonoBehaviour
{

    [SerializeField] string boatName;
    GameObject boat;

    // Start is called before the first frame update
    void Start()
    {
        boat = GameObject.Find("boatName");
        transform.parent = boat.transform;
    }
}
