using System.IO;
using PlanetGeneration.Editor;
using PlanetGeneration.Noise;
using PlanetGeneration.Settings;
using UnityEngine;

namespace PlanetGeneration.Planet
{
    public class ColorGenerator
    {
        private ColorSettings settings;
        private Texture2D texture;
        private const int NumberColors = 2500;
        private int NumBiomes => settings.biomeColorSettings.biomes.Length;
        private INoiseGenerator biomeNoiseGenerator;

        public void UpdateSettings(ColorSettings settings)
        {
            this.settings = settings;
            biomeNoiseGenerator = NoiseGeneratorFactory.CreateNoiseGenerator(settings.biomeColorSettings.noise);
            
            if (texture == null || texture.height != NumBiomes) {
                texture = new Texture2D(NumberColors * 2, NumBiomes * 2 - 2, TextureFormat.RGBA32, false);
            }
        }

        public void UpdateElevation(MinMax elevationMinMax)
        {
            settings.material.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public float BiomePercentFromPoint(Vector3 pointOnUnitSphere)
        {
            float heightPercent = (pointOnUnitSphere.y + 1) / 2f;
            heightPercent += (biomeNoiseGenerator.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
            float biomeIndex = 0;
            int numBiomes = NumBiomes;
            float blendRange = settings.biomeColorSettings.blendAmount / 2f + .0001f;

            for (int i = 0; i < numBiomes; i++)
            {
                float dst = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
                float weight = Mathf.InverseLerp(-blendRange, blendRange, dst);
                biomeIndex *= (1 - weight);
                biomeIndex += i * weight;
            }

            return biomeIndex / Mathf.Max(1, numBiomes - 1);
        }

        public void UpdateColors()
        {
            Color[] colors = new Color[texture.width * texture.height];
            int colorIndex = 0;

            for (int i = 0; i < NumBiomes * 2 - 2; i++) {
                ColorSettings.BiomeColorSettings.Biome biome = settings.biomeColorSettings.biomes[(int)Mathf.Ceil(i / 2f)];
                
                for (int j = 0; j < NumberColors * 2; j++) {
                    Color gradientColor;
                    if (j < NumberColors) {
                        gradientColor = settings.oceanColor.Evaluate(j / (float) NumberColors);
                    } else {
                        gradientColor = biome.gradient.Evaluate((j - NumberColors) / (float)NumberColors);
                    }
                    Color tintColor = biome.tint;
                    colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                    colorIndex++;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            settings.material.SetTexture("_texture", texture);
        }

        public void SaveTextureToPng()
        {
            byte[] bytes = texture.EncodeToPNG();

            File.WriteAllBytes(Application.dataPath + "/SavedScreen.png", bytes);
        }
    }
}