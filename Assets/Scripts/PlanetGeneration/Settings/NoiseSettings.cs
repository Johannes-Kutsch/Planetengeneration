using System;
using UnityEngine;
using PlanetGeneration;
using PlanetGeneration.Editor;

namespace PlanetGeneration.Settings
{
    [Serializable]
    public class NoiseSettings
    {
        public GeneratorType generatorType;
        [ConditionalHide("generatorType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        [ConditionalHide("generatorType", 1)]
        public RigidNoiseSettings rigidNoiseSettings;
        
        [Serializable]
        public class SimpleNoiseSettings
        {
            [Header("Noise Settings")]
            public float strength = 1;
            public float roughness = 2;
            public Vector3 centre;
            
            [Header("Layer Settings")]
            [Range(1,8)]
            public int numberLayers = 1;
            public float persistence = 0.5f;
            public float baseRoughness;
            public float minValue;
        }

        [Serializable]
        public class RigidNoiseSettings : SimpleNoiseSettings
        {
            [Header("Ridgid Noise Settings")]
            public float weightMultiplier = .8f;
        }
    }
    
    public enum GeneratorType {SIMPLE, RIGID}
}