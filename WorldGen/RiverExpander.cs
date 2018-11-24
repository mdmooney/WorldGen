using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class RiverExpander : HexExpander
    {
        private RiverSegment _lastSegment;
        private Coords _lastSegmentCoords;
        private River _currentRiver;

        public override bool AllowsReexpansion { get { return false; } }

        public RiverExpander(HexMap map) : base(map)
        {
            _currentRiver = new River();
        }

        /* <summary>
         * Predicates shared between first placemenet validity checking and expansion
         * validity checking. Checks to ensure that the river will be placed on land,
         * and that there is not already a river at the specified location.
         * </summary>
         * <param name="coords">Coords to check for validity.</param>
         * <returns>True if the hex is valid for placement, false otherwise.</returns>
         */
        private bool CanPlaceRiverShared(Coords coords)
        {
            // Rivers can only exist on land
            if (!_map.IsLandAt(coords)) return false;

            // For now, a hex can contain only one river going in some direction.
            // In the future this will be expanded to allow confluence.
            return !(_map.IsRiverAt(coords));
        }

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
            }

            // Rivers can't double back on themselves
            var existingSegment = _map.GetMainRiverSegmentAt(coords);
            if ((existingSegment != null)
                && (existingSegment.ParentRiver == _currentRiver))
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
            return CanPlaceRiverShared(coords);
        }

        protected override bool ModHex(Coords coords)
        {
            RiverSegment newSeg = new RiverSegment(_currentRiver);

            if (_map.AddMainRiverAt(coords, newSeg))
            {
                if (_lastSegment == null)
                {
                    // this is the first segment in the river
                    _currentRiver.FirstSeg = newSeg;
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
                _currentRiver.LastSeg = newSeg;

                _lastSegment = newSeg;
                _lastSegmentCoords = coords;
                return true;
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
