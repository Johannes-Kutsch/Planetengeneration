using PlanetGeneration.Settings;
using UnityEngine;

namespace PlanetGeneration.Noise
{
    public class SimpleNoiseGenerator : INoiseGenerator
    {
        private const int SeaLevel = 1;
        private readonly SimplexNoise noise = new SimplexNoise();
        private readonly NoiseSettings.SimpleNoiseSettings settings;

        public SimpleNoiseGenerator(NoiseSettings.SimpleNoiseSettings settings)
        {
            this.settings = settings;
        }

        public float Evaluate(Vector3 point)
        {
            float noiseValue = 0;
            float frequency = settings.baseRoughness;
            float amplitude = 1;

            for (int i = 0; i < settings.numberLayers; i++) {
                float v = noise.Evaluate(point * frequency + settings.centre);
                noiseValue += (v + SeaLevel) * 0.5f * amplitude;
                frequency *= settings.roughness;
                amplitude *= settings.persistence;
            }

            noiseValue = noiseValue - settings.minValue;
            return noiseValue * settings.strength;
        }
    }
}