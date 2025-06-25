using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParallaxController : MonoBehaviour
{
    private Transform cam;
    private Vector3 camStartPos;

    private GameObject[] backgrounds;
    private Material[] materials;
    private float[] parallaxFactors;

    private float farthestDistance;

    [Range(0.01f, 0.2f)]
    [SerializeField] private float parallaxSpeed = 0.1f;

    [Header("Follow Player Horizontally")]
    [SerializeField] private Transform player;

    private void Start()
    {
        //Automatically find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not assigned and no GameObject tagged 'Player' found.");
        }

        cam = Camera.main.transform;
        camStartPos = cam.position;

        int count = transform.childCount;
        backgrounds = new GameObject[count];
        materials = new Material[count];
        parallaxFactors = new float[count];

        for (int i = 0; i < count; i++)
        {
            backgrounds[i] = transform.GetChild(i).gameObject;
            materials[i] = backgrounds[i].GetComponent<Renderer>().material;
        }

        CalculateParallaxFactors(count);
    }

    private void CalculateParallaxFactors(int count)
    {
        farthestDistance = float.MinValue;

        for (int i = 0; i < count; i++)
        {
            float zDist = Mathf.Abs(backgrounds[i].transform.position.z - cam.position.z);
            if (zDist > farthestDistance)
                farthestDistance = zDist;
        }

        for (int i = 0; i < count; i++)
        {
            float zDist = Mathf.Abs(backgrounds[i].transform.position.z - cam.position.z);
            parallaxFactors[i] = 1 - (zDist / farthestDistance);
        }
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            //Move the entire background to follow player on X axis
            transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
        }

        float camDelta = cam.position.x - camStartPos.x;

        for (int i = 0; i < backgrounds.Length; i++)
        {
            float offset = camDelta * parallaxFactors[i] * parallaxSpeed;
            materials[i].SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}