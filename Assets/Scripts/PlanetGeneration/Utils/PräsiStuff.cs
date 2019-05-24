using PlanetGeneration.Settings;
using UnityEngine;

namespace PlanetGeneration.Editor
{
    
    public class PräsiStuff : MonoBehaviour
    {
        public Planet.Planet planet;
        public Vector3 offsetChange;
        public float offsetChangeSpeed = 1f;
        public float rotationSpeed = 5f;

        private NoiseSettings.SimpleNoiseSettings SimpleNoiseSettings =>
            planet.settings.shapeSettings.noiseLayers[0].noiseSettings.simpleNoiseSettings;
        private Vector3 startOffset;

        private void Awake()
        {
            startOffset = SimpleNoiseSettings.centre;
            planet.GeneratePlanet();
        }

        private void OnDestroy()
        {
            SimpleNoiseSettings.centre = startOffset;
        }

        private void Update()
        {
            Rotate(Vector3.down, rotationSpeed * Time.deltaTime);
            OffsetSimpleNoiseSettings(SimpleNoiseSettings,
                offsetChange * Time.deltaTime * offsetChangeSpeed);
            if (offsetChangeSpeed > 0) {
                planet.OnShapeSettingsUpdated();
            }
        }

        private void Rotate(Vector3 rotation, float speed)
        {
            transform.Rotate(rotation * speed);
        }

        private void OffsetSimpleNoiseSettings(NoiseSettings.SimpleNoiseSettings settings, Vector3 offset)
        {
            settings.centre += offset;
        }

    }
}