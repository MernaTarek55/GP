using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DynamicLavaTrail : MonoBehaviour
{
    [Header("Trail Settings")]
    public float trailWidth = 0.5f;
    public float segmentLength = 0.2f;
    public float fadeDuration = 3f;
    public float heightOffset = 0.05f; // Prevents z-fighting with ground

    [Header("Visuals")]
    public Material lavaMaterial;
    public Texture2D lavaTexture;
    public float textureScrollSpeed = 0.5f;
    private MeshCollider meshCollider;


    private List<Vector3> pathPoints = new List<Vector3>();
    private Mesh trailMesh;
    private Vector3 lastPosition;
    private float textureOffset;
    private Coroutine fadeCoroutine;

    void Start()
    {
        trailMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = trailMesh;
        GetComponent<MeshRenderer>().material = new Material(lavaMaterial);
        GetComponent<MeshRenderer>().material.SetFloat("_EmissionIntensity", 2.0f);
        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = trailMesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;
        if (lavaTexture)
        {
            GetComponent<MeshRenderer>().material.mainTexture = lavaTexture;
        }

        lastPosition = transform.position;

    }

    void Update()
    {
        // Add new points when enemy moves enough distance
        if (Vector3.Distance(transform.position, lastPosition) > segmentLength)
        {
            AddTrailPoint(transform.position);
            lastPosition = transform.position;
        }

        // Scroll texture
        textureOffset += Time.deltaTime * textureScrollSpeed;
        GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(textureOffset, 0);

        // Always update mesh to handle moving points
        UpdateTrailMesh();
        meshCollider.sharedMesh = trailMesh;

    }

    void AddTrailPoint(Vector3 newPoint)
    {
        // Apply slight height offset to prevent z-fighting
        newPoint.y += heightOffset;
        pathPoints.Add(newPoint);

        // Start fade out if this is the first point
        if (pathPoints.Count == 1 && fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeOutTrail());
        }
    }

    void UpdateTrailMesh()
    {
        if (pathPoints.Count < 2)
        {
            trailMesh.Clear();
            return;
        }

        Vector3[] vertices = new Vector3[pathPoints.Count * 2];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[(pathPoints.Count - 1) * 6];
        Color[] colors = new Color[vertices.Length];

        // Calculate age-based alpha for each point
        float[] pointAlphas = new float[pathPoints.Count];
        for (int i = 0; i < pathPoints.Count; i++)
        {
            pointAlphas[i] = 1f - ((float)i / pathPoints.Count);
        }

        // Create vertices
        for (int i = 0; i < pathPoints.Count; i++)
        {
            Vector3 forward;

            if (i == 0)
            {
                forward = pathPoints[i + 1] - pathPoints[i];
            }
            else if (i == pathPoints.Count - 1)
            {
                forward = pathPoints[i] - pathPoints[i - 1];
            }
            else
            {
                forward = (pathPoints[i + 1] - pathPoints[i - 1]).normalized;
            }

            Vector3 right = Vector3.Cross(forward, Vector3.up).normalized * trailWidth;

            vertices[i * 2] = pathPoints[i] + right;
            vertices[i * 2 + 1] = pathPoints[i] - right;

            // UV mapping (stretch along length)
            float uvX = (float)i / (pathPoints.Count - 1);
            uv[i * 2] = new Vector2(uvX, 0);
            uv[i * 2 + 1] = new Vector2(uvX, 1);

            // Vertex colors for fading
            colors[i * 2] = new Color(1, 1, 1, pointAlphas[i]);
            colors[i * 2 + 1] = new Color(1, 1, 1, pointAlphas[i]);
        }

        // Create triangles
        for (int i = 0; i < pathPoints.Count - 1; i++)
        {
            int triIndex = i * 6;
            int vertIndex = i * 2;

            triangles[triIndex] = vertIndex;
            triangles[triIndex + 1] = vertIndex + 2;
            triangles[triIndex + 2] = vertIndex + 1;

            triangles[triIndex + 3] = vertIndex + 1;
            triangles[triIndex + 4] = vertIndex + 2;
            triangles[triIndex + 5] = vertIndex + 3;
        }

        // Apply to mesh
        trailMesh.Clear();
        trailMesh.vertices = vertices;
        trailMesh.uv = uv;
        trailMesh.triangles = triangles;
        trailMesh.colors = colors;
        trailMesh.RecalculateNormals();
        trailMesh.RecalculateBounds();
    }

    IEnumerator FadeOutTrail()
    {
        while (pathPoints.Count > 0)
        {
            yield return new WaitForSeconds(fadeDuration / pathPoints.Count);

            if (pathPoints.Count > 0)
            {
                pathPoints.RemoveAt(0);
                UpdateTrailMesh();
            }
        }

        fadeCoroutine = null;
    }

    void OnDestroy()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
    }
}