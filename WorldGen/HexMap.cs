using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldGen
{
    class HexMap
    {
        private Hex[,] map;

        // ------------ Getters ------------

        public int width
        {
            get
            {
                return map.GetLength(0);
            }
        }

        public int height
        {
            get
            {
                return map.GetLength(1);
            }
        }

        // ------------ Methods ------------

        public HexMap(int width, int height)
        {
            map = new Hex[width, height];
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = new Hex();
                }
            }
        }

        private Hex GetHexAt(Coords coords)
        {
            if (coords.invalid) return null;

            else return map[coords.x, coords.y];
        }

        public Hex.Elevation GetElevationAt(Coords coords)
        {
            if (coords.invalid) return Hex.Elevation.Low;

            else return GetHexAt(coords).elevation;
        }

        public RiverSegment GetMainRiverSegmentAt(Coords coords)
        {
            if (coords.invalid) return null;
            else return GetHexAt(coords).MainRiverSegment;
        }

        public void SetTypeAt(int x, int y, Hex.HexType type)
        {
            map[x, y].type = type;
        }

        public void SetTypeAt(Coords coords, Hex.HexType type)
        {
            if (!coords.invalid)
            {
                map[coords.x, coords.y].type = type;
            }
        }

        public Color ColorAt(Coords coords)
        {
            return ColorAt(coords.x, coords.y);
        }

        public Color ColorAt(int x, int y)
        {
            return map[x, y].GetColor();
        }

        public Color ElevationColorAt(int x, int y)
        {
            return map[x, y].GetElevationColor();
        }

        public bool PlaceLand(Coords coords)
        {
            return PlaceLand(coords, Hex.HexType.Land);
        }

        public bool PlaceLand(Coords coords, Hex.HexType type)
        {
            // todo: replace with actual validation function
            // to guarantee ranging
            if (coords.invalid) return false;
            else
            {
                Hex hex = GetHexAt(coords);
                if (hex.CanPlace)
                {
                    SetTypeAt(coords, type);
                    hex.CanPlace = false;
                    return true;
                }
                return false;
            }
        }

        public bool BordersOcean(Coords coords)
        {

            Dictionary<Hex.Side, Coords> adj = GetAllAdjacentCoords(coords);
            foreach(Coords adjCoords in adj.Values)
            {
                if (GetHexAt(adjCoords).type == Hex.HexType.Ocean)
                    return true;
            }
            return false;
        }

        public List<Coords> GetAdjacentOceanHexes(Coords coords)
        {
            Dictionary<Hex.Side, Coords> adj = GetAllAdjacentCoords(coords);
            List<Coords> rv = new List<Coords>();
            foreach (Coords adjCoords in adj.Values)
            {
                if (GetHexAt(adjCoords).type == Hex.HexType.Ocean)
                    rv.Add(adjCoords);
            }
            return rv;
        }

        public bool CanPlaceAt(Coords coords)
        {
            if (coords.invalid) return false;
            else
            {
                Hex hex = GetHexAt(coords);
                if (hex.CanPlace)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsRiverAt(Coords coords)
        {
            if (coords.invalid) return false;
            return (GetHexAt(coords).HasRiver());
        }

        public bool IsLandAt(Coords coords)
        {
            if (coords.invalid) return false;
            return GetHexAt(coords).IsLand();
        }

        public void SetPlaceable(Coords coords, bool placeable)
        {
            Hex hex = GetHexAt(coords);
            hex.CanPlace = placeable;
        }

        public bool Raise(Coords coords)
        {
            Hex hex = GetHexAt(coords);
            if (hex.elevation < Hex.Elevation.High)
            {
                hex.elevation++;
                return true;
            }
            return false;
        }

        public bool AddMainRiverAt(Coords coords, RiverSegment riverSegment)
        {
            if (coords.invalid) return false;
            Hex hex = GetHexAt(coords);
            hex.MainRiverSegment = riverSegment;
            return true;
        }

        public Dictionary<Hex.Side, Coords> GetAllAdjacentCoords(Coords coords)
        {
            return GetAllAdjacentCoords(coords.x, coords.y);
        }

        public Dictionary<Hex.Side, Coords> GetAllAdjacentCoords(int x, int y)
        {
            Dictionary<Hex.Side, Coords> coords = new Dictionary<Hex.Side, Coords>();

            for (Hex.Side side = 0; side < Hex.Side.Nil; side++)
            {
                Coords maybeCoords = GetAdjacentCoords(x, y, side);
                if (!maybeCoords.invalid)
                {
                    coords.Add(side, maybeCoords);
                }
            }

            return coords;
        }

        public Dictionary<Hex.Side, Coords> GetPlaceableAdjacentCoords(Coords coords)
        {
            return GetPlaceableAdjacentCoords(coords.x, coords.y);
        }

        public Dictionary<Hex.Side, Coords> GetPlaceableAdjacentCoords(int x, int y)
        {
            Dictionary<Hex.Side, Coords> coords = new Dictionary<Hex.Side, Coords>();

            foreach (Hex.Side side in Enum.GetValues(typeof(Hex.Side)))
            {
                Coords maybeCoords = GetAdjacentCoords(x, y, side);
                if (!maybeCoords.invalid
                    && CanPlaceAt(maybeCoords))
                {
                    coords.Add(side, maybeCoords);
                }
            }

            return coords;
        }

        public bool IsAdjacent(Coords a, Coords b)
        {
            var allAdj = GetAllAdjacentCoords(a);
            return (allAdj.ContainsValue(b));
        }

        public Hex.Side GetAdjacentSide(Coords reference, Coords check)
        {
            for (Hex.Side side = Hex.Side.North; side < Hex.Side.Nil; side++)
            {
                if (GetAdjacentCoords(reference, side) == check)
                    return side;
            }
            return Hex.Side.Nil;
        }

        public List<Coords> GetAllCoords()
        {
            List<Coords> rv = new List<Coords>();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    rv.Add(new Coords(i, j));
                }
            }
            return rv;
        }

        public Coords GetAdjacentCoords(Coords coords, Hex.Side side)
        {
            return GetAdjacentCoords(coords.x, coords.y, side);
        }

        public Coords GetAdjacentCoords(int x, int y, Hex.Side side)
        {
            int upShift = 0;
            int downShift = 0;
            if (x % 2 != 0)
                downShift = 1;
            else
                upShift = -1;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > (width - 1)) x = (width - 1);
            if (y > (height - 1)) y = (height - 1);

            Coords rv = new Coords();
            switch (side)
            {
                case Hex.Side.North:
                    rv.x = x;
                    rv.y = y - 1;
                    break;
                case Hex.Side.Northwest:
                    rv.x = x - 1;
                    rv.y = y + upShift;
                    break;
                case Hex.Side.Northeast:
                    rv.x = x + 1;
                    rv.y = y + upShift;
                    break;
                case Hex.Side.South:
                    rv.x = x;
                    rv.y = y + 1;
                    break;
                case Hex.Side.Southwest:
                    rv.x = x - 1;
                    rv.y = y + downShift;
                    break;
                case Hex.Side.Southeast:
                    rv.x = x + 1;
                    rv.y = y + downShift;
                    break;
                default:
                    break;
            }

            if (rv.y < 0 || rv.y > (height - 1))
            {
                rv.invalid = true;
            }
            if (rv.x < 0)
            {
                rv.x = (width - 1);
            }
            else if (rv.x > (width - 1))
            {
                rv.x = 0;
            }

            return rv;
        }


        // console display method
        public override string ToString()
        {
            string rv = "";
            for (int i = (map.GetLength(0) - 1); i >= 0; i--)
            {
                string add = "";
                if (i % 2 != 0)
                {
                    add += " ";
                }
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    char disp = map[i, j].CharDisplay();
                    add += " " + disp;
                }
                rv += add + "\n";
            }

            rv += "<-+";
            return rv;
        }
    }
}
