using System.Linq;
using PlanetGeneration.Editor;
using PlanetGeneration.Noise;
using PlanetGeneration.Settings;
using UnityEngine;

namespace PlanetGeneration {
    public class ShapeGenerator
    {
        private ShapeSettings settings;
        private INoiseGenerator[] simpleNoiseGenerators;
        private MinMax elevationMinMax;

        private const int SeaLevel = 1;
        
        public void UpdateSettings(ShapeSettings settings, MinMax elevationMinMax)
        {
            this.settings = settings;
            simpleNoiseGenerators = new INoiseGenerator[settings.noiseLayers.Length];
            this.elevationMinMax = elevationMinMax;
            
            for (int i = 0; i < simpleNoiseGenerators.Length; i++) {
                simpleNoiseGenerators[i] = NoiseGeneratorFactory.CreateNoiseGenerator(settings.noiseLayers[i].noiseSettings);
            }
        }

        public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere)
        {
            return Elevation(pointOnUnitSphere);
        }

        public Vector3 GetPointOnUnitSphere(Vector3 pointOnUnitSphere, float unscaledElevation)
        {
            return pointOnUnitSphere * settings.radius * (SeaLevel + Mathf.Max(0, unscaledElevation));
        }
        
        public Vector3 GetPointOnUnitSphere(Vector3 pointOnUnitSphere)
        {
            return pointOnUnitSphere * settings.radius * (SeaLevel + Mathf.Max(0, Elevation(pointOnUnitSphere)));
        }

        private float Elevation(Vector3 pointOnSphere)
        {
            float firstLayerValue = 0;
            float elevation = 0;

            if (simpleNoiseGenerators.Length > 0) {
                firstLayerValue = simpleNoiseGenerators[0].Evaluate(pointOnSphere);
                if (settings.noiseLayers[0].enabled) {
                    elevation = firstLayerValue;
                }
            }
            
            for (int i = 1; i < simpleNoiseGenerators.Length; i++) {
                if (!settings.noiseLayers[i].enabled) {
                    continue;
                }

                float mask = settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1;
                elevation += simpleNoiseGenerators[i].Evaluate(pointOnSphere) * mask;
            }
            elevationMinMax.EvaluateValue(elevation);
            return elevation;
        }
    }
}
