using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen.Tests
{
    class MockRandomGen : IRandomGen
    {
        private int _nextInt = 0;
        private double _nextDubs = 0.0;

        public void SetNextInt(int next)
        {
            _nextInt = next;
        }

        public void SetNextDouble(double next)
        {
            _nextDubs = next;
        }

        public double GenerateDouble()
        {
            return _nextDubs;
        }

        public int GenerateInt(int upper)
        {
            return _nextInt;
        }

        public int GenerateInt(int lower, int upper)
        {
            return _nextInt;
        }
    }
}
