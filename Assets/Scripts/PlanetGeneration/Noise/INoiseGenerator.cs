using UnityEngine;

namespace PlanetGeneration.Noise
{
    public interface INoiseGenerator
    {
        float Evaluate(Vector3 point);
    }
}