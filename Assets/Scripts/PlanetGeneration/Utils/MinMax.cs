namespace PlanetGeneration.Editor
{
    public class MinMax
    {
        public float Min { get; private set; }
        public float Max { get; private set; }

        public MinMax()
        {
            Min = float.MaxValue;
            Max = float.MinValue;
        }

        public void EvaluateValue(float value)
        {
            if (value > Max)
            {
                Max = value;
            }
            
            if (value < Min)
            {
                Min = value;
            }
        }
    }
}