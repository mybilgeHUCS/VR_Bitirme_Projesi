using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneOffsetControl : MonoBehaviour
{
    public GameObject player;

    private Vector3 startPos = Vector3.zero;
    private Renderer planeRenderer;

    private float XPlayerMove => (float)(player.transform.position.x - startPos.x);
    private float ZPlayerMove => (float)(player.transform.position.z - startPos.z);
    private float XPlayerLocation => (float)Mathf.Floor(player.transform.position.x);
    private float ZPlayerLocation => (float)Mathf.Floor(player.transform.position.z);
    [SerializeField] float offsetSpeed = 0.001f;


    void Start()
    {
        startPos = player.transform.position;
        planeRenderer = GetComponent<Renderer>();
        if (planeRenderer == null)
        {
            Debug.LogError("Renderer component not found on the plane object.");
        }
    }

    void FixedUpdate()
    {
        if (hasPlayerMoved(XPlayerMove, ZPlayerMove))
        {
            Vector3 newPlanePosition = new Vector3(XPlayerLocation, transform.position.y, ZPlayerLocation);
            transform.position = newPlanePosition;

            if (planeRenderer != null)
            {
                // Assuming the material has a "_MainTex" property for the texture offset
                Material planeMaterial = planeRenderer.material;
                Vector2 newOffset = new Vector2(-XPlayerLocation * offsetSpeed, -ZPlayerLocation * offsetSpeed); // Scale the offset change for visibility
                planeMaterial.mainTextureOffset = newOffset;
            }
        }
    }

    private bool hasPlayerMoved(float playerX, float playerZ)
    {
        if (Mathf.Abs(playerX) >= 1 || Mathf.Abs(playerZ) >= 1)
        {
            startPos = player.transform.position; // Reset the start position
            return true;
        }
        return false;
    }
}
