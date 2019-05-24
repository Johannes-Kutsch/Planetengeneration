using PlanetGeneration.Settings;
using UnityEngine;

namespace PlanetGeneration.Noise
{
    public class RigidNoiseGenerator : INoiseGenerator
    {
        private readonly SimplexNoise noise = new SimplexNoise();
        private readonly NoiseSettings.RigidNoiseSettings settings;

        public RigidNoiseGenerator(NoiseSettings.RigidNoiseSettings settings)
        {
            this.settings = settings;
        }

        public float Evaluate(Vector3 point)
        {
            float noiseValue = 0;
            float frequency = settings.baseRoughness;
            float amplitude = 1;
            float weight = 1;

            for (int i = 0; i < settings.numberLayers; i++) {
                float v = 1 - Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
                v *= v;
                v *= weight;
                weight = Mathf.Clamp01(v * settings.weightMultiplier);

                noiseValue += v * amplitude;
                frequency *= settings.roughness;
                amplitude *= settings.persistence;
            }

            noiseValue = noiseValue - settings.minValue;
            return noiseValue * settings.strength;
        }
    }
}