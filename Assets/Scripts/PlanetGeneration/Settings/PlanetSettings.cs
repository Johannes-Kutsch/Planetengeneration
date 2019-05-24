using UnityEngine;

namespace PlanetGeneration.Settings
{
    [CreateAssetMenu(fileName = "Planet Settings", menuName = "Settings/Planet", order = 0)]
    public class PlanetSettings : ScriptableObject
    {
        public bool autoUpdate = false;
        
        [Range(2,256)]
        public int resolution = 10;
        
        public ShapeSettings shapeSettings;
        public ColorSettings colorSettings;
    }
}