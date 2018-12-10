using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class LayeredExpansion
    {
        public HexExpander Expander { get; private set; }
        public List<Coords> ValidCoords { get; private set; }
        public double LowerProportionBound { get; private set; }
        public double UpperProportionBound{ get; private set; }

        private static Random _rand = new Random();

        public LayeredExpansion(HexExpander hexExpander, List<Coords> validCoords)
        {
            Expander = hexExpander;
            ValidCoords = validCoords;
        }

        public LayeredExpansion(HexExpander hexExpander, List<Coords> validCoords,
            double lowerProportionBound, double upperProportionBound)
            : this (hexExpander, validCoords)
        {
            LowerProportionBound = lowerProportionBound;
            UpperProportionBound = upperProportionBound;
        }

        public void Expand(int passes)
        {
            int hexesToModify = ValidCoords.Count;
            for (int i = 0; i < passes; i++)
            {
                double proportionThisRound = _rand.NextDouble();
                proportionThisRound *= (UpperProportionBound - LowerProportionBound);
                proportionThisRound += LowerProportionBound;

                hexesToModify = (int)(hexesToModify * proportionThisRound);
                Expander.Expand(ValidCoords, hexesToModify);
            }
        }
    }
}
