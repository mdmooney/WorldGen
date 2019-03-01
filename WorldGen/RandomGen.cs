using System;

namespace WorldGen
{
    class RandomGen : IRandomGen
    {
        private static Random _rand = new Random();

        public double GenerateDouble()
        {
            return _rand.NextDouble();
        }

        public int GenerateInt(int upper)
        {
            return _rand.Next(upper);
        }

        public int GenerateInt(int lower, int upper)
        {
            return _rand.Next(lower, upper);
        }
    }
}
