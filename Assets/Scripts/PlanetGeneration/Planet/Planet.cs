using PlanetGeneration.Editor;
using PlanetGeneration.Settings;
using UnityEngine;

namespace PlanetGeneration.Planet
{
    public class Planet : MonoBehaviour
    {
        public PlanetSettings settings;

        [HideInInspector]
        public bool shapeSettingsFoldout;
        [HideInInspector]
        public bool colourSettingsFoldout;
        [HideInInspector]
        public bool planetSettingsFoldout;

        [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
        private TerrainFace[] terrainFaces;
        private ShapeGenerator shapeGenerator = new ShapeGenerator();
        private ColorGenerator colorGenerator = new ColorGenerator();
        private MinMax elevationMinMax;

        private const int NumberFaces = 6;
        private static readonly Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        private void Initialize()
        {
            elevationMinMax = new MinMax();
            shapeGenerator.UpdateSettings(settings.shapeSettings, elevationMinMax);
            colorGenerator.UpdateSettings(settings.colorSettings);
            CreateMeshFilters();
            CreatTerrainFaces();
        }

        public void GeneratePlanet()
        {
            Initialize();
            GenerateMesh();
            UpdateColors();
        }

        private void CreateMeshFilters()
        {
            if (!MeshFiltersInitialized())
            {
                InitializeMeshFilters();
            }
            
            for (int i = 0; i < NumberFaces; i++)
            {
                if (!MeshFilterExists(i))
                {
                    CreateMeshFilter(i);
                }
            }
        }

        private void CreatTerrainFaces()
        {
            terrainFaces = new TerrainFace[6];
            for (int i = 0; i < NumberFaces; i++) {
                terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, settings.resolution, directions[i]);
            }
        }
        
        private bool MeshFiltersInitialized()
        {
            return meshFilters != null && (meshFilters != null || meshFilters.Length != 0);
        }

        private void InitializeMeshFilters()
        {
            meshFilters = new MeshFilter[6];
        }

        private bool MeshFilterExists(int index)
        {
            return meshFilters[index] != null;
        }

        private void CreateMeshFilter(int index)
        {
            GameObject meshObj = new GameObject("mesh");
            meshObj.transform.parent = transform;

            meshObj.AddComponent<MeshRenderer>().sharedMaterial = settings.colorSettings.material;
            meshFilters[index] = meshObj.AddComponent<MeshFilter>();
            meshFilters[index].sharedMesh = new Mesh();
        }
        
        public void OnShapeSettingsUpdated()
        {
            if (settings.autoUpdate) {
                Initialize();
                GenerateMesh();
            }
        }
        
        private void GenerateMesh()
        {
            foreach (TerrainFace face in terrainFaces)
            {
                face.ConstructMesh();
            }
            
            colorGenerator.UpdateElevation(elevationMinMax);
        }

        public void OnColorSettingsUpdated()
        {
            if (settings.autoUpdate) {
                Initialize();
                UpdateColors();
            }
        }

        private void UpdateColors()
        {
            colorGenerator.UpdateColors();
            colorGenerator.SaveTextureToPng();
            
            foreach (TerrainFace face in terrainFaces)
            {
                face.UpdateUVs(colorGenerator);
            }
        }
        
        public void OnPlanetSettingsUpdated()
        {
            if (settings.autoUpdate) {
                GeneratePlanet();
            }
        }
    }
}