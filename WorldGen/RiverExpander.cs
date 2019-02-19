using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    /**
     * <summary>
     * Class to expand a river by placing it, segment by segment, over a set of hexes.
     * </summary>
     */
    class RiverExpander : HexExpander
    {
        private RiverSegment _lastSegment;
        private Coords _lastSegmentCoords;

        public River ExpandedRiver { get; private set; }

        // No re-expansion; rivers are linear
        public override bool AllowsReexpansion { get { return false; } }

        /**
         * <summary>
         * Sole constructor for the RiverExpander class.
         * Initializes a new River object, which will be referred to for all
         * segments of the new river.
         * </summary>
         */
        public RiverExpander(HexMap map) : base(map)
        {
            ExpandedRiver = new River();
        }

        /**
         * <summary>
         * Predicates shared between first placement validity checking and expansion
         * validity checking. Checks to ensure that the river will be placed on land,
         * and that there is not already a river at the specified location.
         * </summary>
         * <param name="coords">Coords to check for validity.</param>
         * <returns>True if the hex is valid for placement, false otherwise.</returns>
         */
        private bool CanPlaceRiverShared(Coords coords)
        {
            // For now, a hex can contain only one river going in some direction.
            // In the future this will be expanded to allow confluence.
            return !(_map.IsRiverAt(coords));
        }

        /**
         * <summary>
         * Expansion validity checker for river segment placement.
         * Determines if the next segment of this river can be placed at the hex located
         * at a set of given coords (see <see cref="HexExpander.CanExpandTo(Coords)"/>).
         * </summary>
         * <remarks>
         * Checks that the river is contiguous, flows laterally or downhill (prefers downhill),
         * and that the river is not doubling back on itself, then checks the shared conditions
         * (see <see cref="CanPlaceRiverShared(Coords)"/>).
         * </remarks>
         * <param name="coords"></param>
         * <returns></returns>
         */
        protected override bool CanExpandTo(Coords coords)
        {
            if (_lastSegmentCoords != null)
            {
                // Rivers must be contiguous
                var adj = _map.GetAllAdjacentCoords(_lastSegmentCoords);
                if (!adj.ContainsValue(coords)) return false;

                // Rivers must flow laterally or downhill
                var lastHeight = _map.GetElevationAt(_lastSegmentCoords);
                var newHeight = _map.GetElevationAt(coords);
                if (newHeight > lastHeight) return false;

                // Prefer downhill movement
                if (newHeight == lastHeight)
                {
                    var lowerAdj = adj.Where(x => (_map.GetElevationAt(x.Value) < lastHeight)
                                             || (_map.IsWaterAt(x.Value))).ToList();
                    if (lowerAdj.Count > 0) return false;
                }
            }

            // Rivers can't double back on themselves
            var existingSegment = _map.GetMainRiverSegmentAt(coords);
            if ((existingSegment != null)
                && (existingSegment.ParentRiver == ExpandedRiver))
            {
                return false;
            }

            // Check shared conditions
            return CanPlaceRiverShared(coords);
        }

        protected override void FinishAdjacentUnexpanded(Coords coords)
        {
            // empty method body, we don't want to do anything here
        }

        protected override bool CanExpandFirst(Coords coords)
        {
            // Rivers can only begin on land
            if (!_map.IsLandAt(coords)) return false;

            return CanPlaceRiverShared(coords);
        }

        /**
         * <summary>
         * Attempts to modify a hex by placing a river segment there.
         * Can be a land or water hex, but if it's water hex, the river terminates.
         * </summary>
         * <param name="coords">Coords for the hex to be modified.</param>
         * <returns>True if a new RiverSegment was placed in the hex at <paramref name="coords"/>, false otherwise.</returns>
         */
        protected override bool ModHex(Coords coords)
        {
            if (_map.IsWaterAt(coords)
                && (_lastSegment != null))
            {
                // River flowing into a body of water, that's the end of the line.
                // No new segment will be added; there's no meaning to a river on top of
                // a water hex.
                Hex.Side exitSide = _map.GetAdjacentSide(_lastSegmentCoords, coords);
                _lastSegment.ExitSide = exitSide;
                FinalizeEarly();
                return true;
            }
            else
            {
                RiverSegment newSeg = new RiverSegment(ExpandedRiver);

                // Adding the river to land
                if (_map.AddMainRiverAt(coords, newSeg))
                {
                    if (_lastSegment == null)
                    {
                        // this is the first segment in the river
                        ExpandedRiver.FirstSeg = newSeg;
                    }
                    else
                    {
                        // add it to the chain
                        _lastSegment.NextSegment = newSeg;
                        newSeg.PrevSegment = _lastSegment;

                        // entry and exit sides
                        Hex.Side newEntrySide = _map.GetAdjacentSide(coords, _lastSegmentCoords);
                        Hex.Side lastExitSide = Hex.OppositeSide(newEntrySide);
                        _lastSegment.ExitSide = lastExitSide;
                        newSeg.EntrySide = newEntrySide;
                    }
                    newSeg.Location = coords;
                    ExpandedRiver.LastSeg = newSeg;

                    _lastSegment = newSeg;
                    _lastSegmentCoords = coords;
                    return true;
                }
            }

            return false;
        }

        protected override int RollBaseExpansion(int adjCount)
        {
            // Rivers do not diverge; they can expand to one hex only
            return 1;
        }

        protected override int RollModifier()
        {
            // No divergance
            return 0;
        }

    }
}
