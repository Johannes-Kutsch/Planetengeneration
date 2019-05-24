using UnityEngine;

namespace PlanetGeneration.Settings
{
    [CreateAssetMenu(fileName = "Shape Settings", menuName = "Settings/Shape")]
    public class ShapeSettings : ScriptableObject
    {
        public float radius = 1;
        
        public NoiseLayer[] noiseLayers;

        [System.Serializable]
        public class NoiseLayer
        {
            public bool useFirstLayerAsMask;
            public bool enabled = true;
            public NoiseSettings noiseSettings;
        }
    }
}