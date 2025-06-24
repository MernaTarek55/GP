using UnityEngine;

[CreateAssetMenu( menuName = "Laser/Laser Settings")]
public class LaserRendererSettings : ScriptableObject
{
    [SerializeField] public Color color;
    [SerializeField] public float width;
    [SerializeField][Range(1f , 200f)] public float emissionAmount;

    public void Apply(LineRenderer lineRenderer)
    {
        if (lineRenderer == null) return;
        lineRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        lineRenderer.material.EnableKeyword("_EMISSION");
        lineRenderer.startColor = color;
        lineRenderer.startWidth = width;
        lineRenderer.material.SetColor("_EmissionColor", color * emissionAmount);

    }
}
