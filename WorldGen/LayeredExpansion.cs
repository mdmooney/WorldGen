using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    /**
     * <summary>
     * Class to repeatedly carry out an expansion using the same HexExpander
     * over the same overall set of hexes. Expansion is done over a caller-
     * determined number of passes. 
     * </summary>
     * <remarks>
     * The number of hexes to be modified decreases each pass to a random
     * fraction, with bounds that can be set by the client.
     * </remarks>
     */
    class LayeredExpansion
    {
        /// <summary>
        /// Underlying HexExpander used to carry out the layered expansion.
        /// </summary>
        public LayeredExpander Expander { get; private set; }

        /// <summary>
        /// List of coords valid for layered expansion.
        /// </summary>
        public List<Coords> ValidCoords { get; private set; }

        /// <summary>
        /// Upper bound for random determination of the proportion of hexes to
        /// reuse in each pass of expansion.
        /// </summary>
        public double LowerProportionBound { get; private set; }

        /// <summary>
        /// Lower bound for random determination of the proportion of hexes to
        /// reuse in each pass of expansion.
        /// </summary>
        public double UpperProportionBound{ get; private set; }

        private static Random _rand = new Random();

        /**
         * <summary>
         * Constructor for LayeredExpansion takes a HexExpander to carry out the
         * expansion and a list of coords that are valid. Proportion bounds are
         * set to 0.0 for the lower and 1.0 for the upper.
         * </summary>
         * <param name="hexExpander">HexExpander that will carry out expansion.</param>
         * <param name="validCoords">Coords that may be expanded in.</param>
         */
        public LayeredExpansion(LayeredExpander layeredExpander, List<Coords> validCoords)
        {
            Expander = layeredExpander;
            ValidCoords = validCoords;
        }

        /**
         * <summary>
         * Constructor for LayeredExpansion which allows customization of the
         * upper and lower bounds for hex proportioning.
         * </summary>
         * <param name="hexExpander">HexExpander that will carry out expansion.</param>
         * <param name="validCoords">Coords that may be expanded in.</param>
         * <param name="lowerProportionBound">Lower bound of hex proportioning.</param>
         * <param name="upperProportionBound">Upper bound of hex proportioning.</param>
         */
        public LayeredExpansion(LayeredExpander layeredExpander, List<Coords> validCoords,
            double lowerProportionBound, double upperProportionBound)
            : this (layeredExpander, validCoords)
        {
            LowerProportionBound = lowerProportionBound;
            UpperProportionBound = upperProportionBound;
        }

        /**
         * <summary>
         * Layered expansion method. Basically just repeats an expansion using
         * the given HexExpander for the given number of passes. On each pass,
         * the number of hexes that will be modified is reduced by a random
         * amount between the two proportion bounds of this class.
         * </summary>
         * <param name="passes">Number of expansion passes.</param>
         */
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
