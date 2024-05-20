using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneOffsetControl : MonoBehaviour
{
    public GameObject player;

    private Vector3 startPos = Vector3.zero;
    private Renderer planeRenderer;

    private int XPlayerMove => (int)(player.transform.position.x - startPos.x);
    private int ZPlayerMove => (int)(player.transform.position.z - startPos.z);
    private int XPlayerLocation => (int)Mathf.Floor(player.transform.position.x);
    private int ZPlayerLocation => (int)Mathf.Floor(player.transform.position.z);

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
                Vector2 newOffset = new Vector2(-XPlayerLocation * 0.001f, -ZPlayerLocation * 0.001f); // Scale the offset change for visibility
                planeMaterial.mainTextureOffset = newOffset;

                // Debug logs
                Debug.Log("New Plane Position: " + newPlanePosition);
                Debug.Log("New Texture Offset: " + newOffset);
            }
        }
    }

    private bool hasPlayerMoved(int playerX, int playerZ)
    {
        if (Mathf.Abs(playerX) >= 1 || Mathf.Abs(playerZ) >= 1)
        {
            startPos = player.transform.position; // Reset the start position
            return true;
        }
        return false;
    }
}
