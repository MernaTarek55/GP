using UnityEngine;

public class LaserQualitySettings : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] allLaserEffects;
    [SerializeField] private int particlesReductionFactor = 1;

    void Start()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();

        if (qualityLevel < 2) // Low quality
        {
            particlesReductionFactor = 3;
            foreach (var ps in allLaserEffects)
            {
                var main = ps.main;
                main.maxParticles /= 3;
            }
        }
    }
}