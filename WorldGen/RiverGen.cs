using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class RiverGen
    {
        private River _river;
        private Landmass _landmass;
        private World _world;
        private HashSet<Coords> _allCoords;
        private static Random _rand = new Random();
        private int _requestedLength;

        public River GenRiver { get {return _river;} }

        public RiverGen(World world, Landmass landmass, int length)
        {
            _world = world;
            _landmass = landmass;
            _allCoords = new HashSet<Coords>();
            _requestedLength = length;
        }

        private bool DoublesBack(Coords dest)
        {
            return ((_allCoords.Contains(dest))
                    || (_world.Map.IsRiverAt(dest)));   //  this condition is temporary until confluence is implemented
        }

        private bool IsLowerOrWater(Coords origin, Coords dest)
        {
            return ((_world.Map.GetElevationAt(origin) > _world.Map.GetElevationAt(dest))
                    || (_world.Map.IsWaterAt(dest)));
        }

        private bool IsLateral(Coords origin, Coords dest)
        {
            return ((_world.Map.GetElevationAt(origin) == _world.Map.GetElevationAt(dest)));
        }

        public int Generate()
        {
            PlaceFirstSegment();
            while ((_allCoords.Count < _requestedLength) && Extend()) ;
            return _allCoords.Count;
        }

        private void PlaceFirstSegment()
        {
            while (_river == null)
            {
                Coords candidate = _landmass.RandomCoords();
                if (!_world.Map.IsRiverAt(candidate))
                {
                    _river = new River(candidate);
                    _allCoords.Add(candidate);
                }
            }
        }
        
        private bool Extend()
        {
            RiverSegment lastSeg = _river.LastSeg;
            Coords lastLoc = lastSeg.Location;

            // Try to get downhill hexes first, if possible
            var adj = _world.Map.GetFilteredAdjacency(lastLoc, (x => !DoublesBack(x) && IsLowerOrWater(lastLoc, x)));

            // Nothing lower; need to do a lateral move
            if (adj.Count == 0)
            {
                adj = _world.Map.GetFilteredAdjacency(lastLoc, (x => !DoublesBack(x) && IsLateral(lastLoc, x)));
            }

            // Nowhere to go, can't extend
            if (adj.Count == 0)
                return false;

            var adjList = adj.ToList();
            var candidate = adjList[_rand.Next(adjList.Count)];
            

            if (_world.Map.IsWaterAt(candidate.Value))
            {
                _river.Terminate(candidate.Key);
                return false;
            }

            _river.Extend(candidate.Key, candidate.Value);
            _allCoords.Add(candidate.Value);
            return true;
        }


        public void Commit()
        {
            // iterate through the river and actually place the river segments where they belong
            RiverSegment seg = _river.FirstSeg;

            do
            {
                _world.Map.AddMainRiverAt(seg.Location, seg);
                seg = seg.NextSegment;
            }
            while (seg.NextSegment != null);
        }
    }
}
