using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    /**
     * <summary>
     * Abstract subclass of HexExpander, intended for use in layered expansions
     * (i.e. using the <see cref="LayeredExpansion"/> class). HexExpanders that
     * subclass this should generally perform modifications that can be layered
     * (usually stepwise moves through enums, like changing elevation levels).
     * </summary>
     */
    abstract class LayeredExpander : HexExpander
    {
        private List<Coords> _alreadyModified;

        /**
         * <summary>
         * Subclasses must implement this method to modify a hex that hasn't
         * already been modified in this pass.
         * </summary>
         * <remarks>
         * This is basically like ModHex (see
         * <see cref="ModHex(Coords)"/>), except that this class must do some
         * processing of its own in ModHex that should be invisible to the subclass.
         * </remarks>
         * <param name="coords">Coords of the Hex to modify.</param>
         */
        protected abstract bool LayeredModHex(Coords coords);

        /**
         * <summary>
         * Constructor for LayeredExpander takes the map to alter.
         * </summary>
         * <param name="map">The HexMap to update.</param>
         */
        public LayeredExpander(HexMap map) : base(map)
        {
            _alreadyModified = new List<Coords>();
        }


        /**
         * <summary>
         * Expansion validity check. Simply checks to ensure that the Hex at
         * the given Coords has not already been changed in this pass.
         * </summary>
         * <param name="coords">The Coords to query.</param>
         * <returns>
         * True if the the Hex at <paramref name="coords"/> has not yet been
         * modified in this pass, false otherwise.
         * </returns>
         */
        protected override bool CanExpandTo(Coords coords)
        {
            return !(_alreadyModified.Contains(coords));
        }

        /**
         * <summary>
         * Update method for Hexes adjacent to Coords that were altered in
         * this round of expansion, but not expanded from themselves.
         * </summary>
         * <remarks>
         * For this class, we just clear the already-modified hex tracking
         * for the next round of expansion.
         * </remarks>
         */
        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // Just clear out already modfied hexes for the next round
            // of expansion
            _alreadyModified.Clear();
        }

        /**
         * <summary>
         * Hex modification. Function of this depends on subclass implementation
         * of LayeredModHex, but on a successful modification will always add
         * the coords to the list of altered hexes for this round.
         * </summary>
         * <returns>True if the hex was modified, false otherwise.</returns>
         */
        protected override bool ModHex(Coords coords)
        {
            if (LayeredModHex(coords))
            {
                _alreadyModified.Add(coords);
                return true;
            }
            return false;
        }
    }
}
