using PlanetGeneration.Settings;

namespace PlanetGeneration.Noise
{
    public static class NoiseGeneratorFactory
    {
        public static INoiseGenerator CreateNoiseGenerator(NoiseSettings settings)
        {
            switch (settings.generatorType) {
                case GeneratorType.SIMPLE:
                    return new SimpleNoiseGenerator(settings.simpleNoiseSettings);
                case GeneratorType.RIGID:
                    return new RigidNoiseGenerator(settings.rigidNoiseSettings);
            }

            return null;
        }
    }
}