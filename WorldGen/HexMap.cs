using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WorldGen
{
    /**
     * <summary>
     * Represents a geographical map, where the smallest unit is a Hex, a six-sided tile.
     * Maps have a fixed height (number of Hex objects in each column) and width (number of
     * Hex objects in each row). 
     * 
     * All HexMaps are treated as cylindrical (i.e. they wrap around horizontally, but
     * not vertically).
     * 
     * This class also contains various methods for interacting with the map and with the
     * underlying Hex objects at different Coords.
     * </summary>
     * <remarks>
     * Because hexagons are tesselated in a map of this sort, it's expected that rows will be
     * staggered.
     * </remarks>
     */
    class HexMap
    {
        // Underlying 2D representation.
        // A hex grid is basically considered a staggered rectangular array of Hexes.
        private Hex[,] _map;

        // List of landmasses
        public List<Landmass> Landmasses { get; set; }

        // ------------ Getters ------------

        /**
         * <summary>
         * The Width of the HexMap, i.e. how many Hexes are in each row.
         * </summary>
         */
        public int Width
        {
            get
            {
                return _map.GetLength(0);
            }
        }

        /**
         * <summary>
         * The Height of the HexMap, i.e. how many Hexes are in each column.
         * </summary>
         */
        public int Height
        {
            get
            {
                return _map.GetLength(1);
            }
        }

        // ------------ Methods ------------

        /**
        * <summary>
        * Constructor for HexMap requires width (number of tiles in each row)
        * and height (number of tiles in each column).
        * </summary>
        * <param name="width">Number of Hex objects in each row of the map.</param>
        * <param name="height">Number of Hex objects in each column of the map.</param>
        */
        public HexMap(int width, int height)
        {
            _map = new Hex[width, height];
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    _map[i, j] = new Hex();
                }
            }
        }

        /**
         * <summary>
         * Gets the Hex object at given Coords.
         * </summary>
         * <param name="coords">The Coords at which to retrieve a Hex.</param>
         * <returns>The Hex object at <paramref name="coords"/>. May be null if Coords are invalid.</returns>
         */
        private Hex GetHexAt(Coords coords)
        {
            if (coords.invalid) return null;

            else return _map[coords.x, coords.y];
        }

        /**
         * <summary>
         * Gets the ElevationLevel for the Hex at given Coords.
         * </summary>
         * <param name="coords">The Coords at which to retrieve ElevationLevel for a Hex.</param>
         * <returns>The ElevationLevel for the given Coords. Defaults to Low if Coords are invalid.</returns>
         */
        public Hex.ElevationLevel GetElevationAt(Coords coords)
        {
            if (coords.invalid) return Hex.ElevationLevel.Low;

            else return GetHexAt(coords).Elevation;
        }

        /**
         * <summary>
         * Gets the primary RiverSegment at given Coords.
         * </summary>
         * <param name="coords">The Coords at which to retrieve the main RiverSegment for a Hex.</param>
         * <returns>
         * The main RiverSegment for the Hex at Coords. May be null if Coords are invalid,
         * or if the Hex does not have any RIverSegments.
         * </returns>
         */
        public RiverSegment GetMainRiverSegmentAt(Coords coords)
        {
            if (coords.invalid) return null;
            else return GetHexAt(coords).MainRiverSegment;
        }

        /**
         * <summary>
         * Sets the general HexType for a Hex at given x and y values on the HexMap.
         * See <see cref="Hex.HexType"/>.
         * </summary>
         * 
         * <param name="x">x value of the Hex to be changed.</param>
         * <param name="y">y value of the Hex to be changed.</param>
         * <param name="type">New HexType for the given Hex.</param>
         */
        public void SetTypeAt(int x, int y, Hex.HexType type)
        {
            _map[x, y].Type = type;
        }

        /**
         * <summary>
         * Sets the general HexType for a Hex at given x and y values on the HexMap.
         * See <see cref="Hex.HexType"/>.
         * </summary>
         * <remarks>This overload uses Coords rather than specific x and y values.</remarks>
         * <param name="coords">The Coords for the Hex to be changed.</param>
         * <param name="type">New HexType for the given Hex.</param>
         */
        public void SetTypeAt(Coords coords, Hex.HexType type)
        {
            if (!coords.invalid)
            {
                _map[coords.x, coords.y].Type = type;
            }
        }

        /**
         * <summary>
         * Returns the base Color of the Hex at the specified Coords.
         * Color is Specific to the broad type of hex (e.g. land, ocean).
         * </summary>
         * <param name="coords">The Coords for the Hex to be queried.</param>
         * <returns>The Color of the Hex at the specified Coords.</returns>
         */
        public Color BaseColorAt(Coords coords)
        {
            return _map[coords.x, coords.y].GetBaseColor();
        }

        /**
         * <summary>
         * Returns the Color of the Hex at the specified Coords.
         * This Color should be based on the ElevationLevel of that Hex.
         * </summary>
         * <param name="coords">The Coords of the Hex to be queried.</param>
         * <returns>The elevation-based Color of the Hex at the specified coordinates.</returns>
         */
        public Color ElevationColorAt(Coords coords)
        {
            return _map[coords.x, coords.y].GetElevationColor();
        }

        /**
         * <summary>
         * Returns the Color of the Hex at the specified Coords.
         * Color here is based on the TemperatureLevel of that Hex.
         * </summary>
         * <param name="coords">Coords of the Hex to be queried.</param>
         * <returns>The temperature-based Color of the Hex at <paramref name="coords"/>.</returns>
         */
        public Color TemperatureColorAt(Coords coords)
        {
            return _map[coords.x, coords.y].GetTemperatureColor();
        }

        /**
         * <summary>
         * Returns the Color of the Hex at the specified Coords.
         * Color here is based on the HumidityLevel of that Hex.
         * </summary>
         * <param name="coords">Coords of the Hex to be queried.</param>
         * <returns>The humidity-based Color of the Hex at <paramref name="coords"/>.</returns>
         */
        public Color HumidityColorAt(Coords coords)
        {
            return _map[coords.x, coords.y].GetHumidityColor();
        }

        public Tuple<Hex.TemperatureLevel, Hex.HumidityLevel> GetTemperatureAndHumidityAt(Coords coords)
        {
            Hex hex = GetHexAt(coords);
            Tuple<Hex.TemperatureLevel, Hex.HumidityLevel> rv 
                = new Tuple<Hex.TemperatureLevel, Hex.HumidityLevel>(hex.Temperature, hex.Humidity);
            return rv;
        }

        public Biome GetBiomeAt(Coords coords)
        {
            return GetHexAt(coords).HexBiome;
        }

        public void SetBiomeAt(Biome biome, Coords coords)
        {
            Hex hex = GetHexAt(coords);
            hex.HexBiome = biome;
        }

        public Color BiomeColorAt(Coords coords)
        {
            return GetHexAt(coords).GetBiomeColor();
        }

        /**
         * <summary>
         * Sets the type of the Hex at specified Coords to HexType.Land, and updates
         * that Hex to signify that land has been placed there (as opposed to ocean,
         * etc).
         * </summary>
         * <param name="coords">The Coords of the Hex to update.</param>
         * <returns>True if successfully updated, false otherwise.</returns>
         */
        public bool PlaceLand(Coords coords)
        {
            // todo: replace with actual validation function
            // to guarantee ranging
            if (coords.invalid) return false;
            else
            {
                Hex hex = GetHexAt(coords);
                if (hex.CanPlace)
                {
                    SetTypeAt(coords, Hex.HexType.Land);
                    hex.CanPlace = false;
                    return true;
                }
                return false;
            }
        }

        /**
         * <summary>
         * Checks if the Hex at given Coords borders an ocean Hex.
         * </summary>
         * <param name="coords">Coords of the Hex to query.</param>
         * <returns>True if the Hex at <paramref name="coords"/> borders an ocean Hex, false otherwise.</returns>
         */
        public bool BordersOcean(Coords coords)
        {

            Dictionary<Hex.Side, Coords> adj = GetAllAdjacentCoords(coords);
            foreach(Coords adjCoords in adj.Values)
            {
                if (GetHexAt(adjCoords).Type == Hex.HexType.Ocean)
                    return true;
            }
            return false;
        }

        /**
         * <summary>
         * Given a set of Coords, retrieves a list of all Coords that are adjacent to those
         * Coords, and which are ocean Hexes.
         * </summary>
         * <param name="coords">The Coords of the Hex to be queried.</param>
         * <returns>
         * A List containing all adjacent Coords to <paramref name="coords"/>, where
         * the corresponding Hex is an ocean hex.
         * </returns>
         */
        public List<Coords> GetAdjacentOceanHexes(Coords coords)
        {
            Dictionary<Hex.Side, Coords> adj = GetAllAdjacentCoords(coords);
            List<Coords> rv = new List<Coords>();
            foreach (Coords adjCoords in adj.Values)
            {
                if (GetHexAt(adjCoords).Type == Hex.HexType.Ocean)
                    rv.Add(adjCoords);
            }
            return rv;
        }

        /**
         * <summary>
         * Predicate to determine if land can be placed on the Hex at given Coords.
         * </summary>
         * <param name="coords">The Coords of the Hex to be queried.</param>
         * <returns>True if land can be placed at <paramref name="coords"/>, false otherwise.</returns>
         */
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

        /**
         * <summary>
         * Predicate to determine if at least one river exists in the Hex at given Coords.
         * </summary>
         * <param name="coords">The Coords of the Hex to be queried.</param>
         * <returns>True if a river is on the Hex indicated by <paramref name="coords"/>, false otherwise.</returns>
         */
        public bool IsRiverAt(Coords coords)
        {
            if (coords.invalid) return false;
            return (GetHexAt(coords).HasRiver());
        }

        /**
         * <summary>
         * Predicate to determine if there is a land Hex at the given Coords.
         * See <see cref="Hex.IsLand"/>.
         * </summary>
         * <param name="coords">The Coords of the Hex to be queried.</param>
         * <returns>True if the Hex at <paramref name="coords"/> is land, false otherwise.</returns>
         */
        public bool IsLandAt(Coords coords)
        {
            if (coords.invalid) return false;
            return GetHexAt(coords).IsLand();
        }

        /**
         * <summary>
         * Predicate to determine if there is a water Hex at the given Coords.
         * See <see cref="Hex.IsWater"/>.
         * </summary>
         * <param name="coords">The Coords of the Hex to be queried.</param>
         * <returns>True if the Hex at <paramref name="coords"/> is water, false otherwise.</returns>
         */
        public bool IsWaterAt(Coords coords)
        {
            if (coords.invalid) return false;
            return GetHexAt(coords).IsWater();
        }

        /**
         * <summary>
         * Sets whether the Hex at given Coords is placeable; that is, whether
         * the Hex can be made a land Hex or not.
         * </summary>
         * <param name="coords">The Coords of the Hex to update.</param>
         * <param name="placeable">True if the Hex can be made into a land Hex, false if not.</param>
         */
        public void SetPlaceable(Coords coords, bool placeable)
        {
            Hex hex = GetHexAt(coords);
            hex.CanPlace = placeable;
        }

        /**
         * <summary>
         * Raises the ElevationLevel of the Hex at given Coords to the next step upwards.
         * </summary>
         * <param name="coords">The Coords of the Hex to update.</param>
         * <returns>True if the Hex's ElevationLevel was increased, false otherwise.</returns>
         */
        public bool Raise(Coords coords)
        {
            Hex hex = GetHexAt(coords);
            if (hex.Elevation < Hex.ElevationLevel.High)
            {
                hex.Elevation++;
                return true;
            }
            return false;
        }

        public bool Aridify(Coords coords)
        {
            Hex hex = GetHexAt(coords);
            if (hex.Humidity > Hex.HumidityLevel.Arid)
            {
                hex.Humidity--;
                return true;
            }
            return false;
        }

        /**
         * <summary>
         * Sets the TemperatureLevel of the Hex at the given Coords to the specified level.
         * </summary>
         * <param name="coords">Coords of the Hex to update.</param>
         * <param name="temperature">New temperature for the Hex at <paramref name="coords"/>.</param>
         */
        public void SetTemperatureAt(Coords coords, Hex.TemperatureLevel temperature)
        {
            Hex hex = GetHexAt(coords);
            hex.Temperature = temperature;
        }

        /**
         * <summary>
         * Adds a main RiverSegment to the Hex at given Coords. This is a tile-sized chunk of the
         * primary river that moves through this Hex.
         * </summary>
         * <param name="coords">The Coords of the Hex to update.</param>
         * <param name="riverSegment">The RiverSegment to make the primary river at this Hex.</param>
         * <returns>True if the Hex was updated. False only if the Coords were invalid.</returns>
         */
        public bool AddMainRiverAt(Coords coords, RiverSegment riverSegment)
        {
            if (coords.invalid) return false;
            Hex hex = GetHexAt(coords);
            hex.MainRiverSegment = riverSegment;
            return true;
        }

        /**
         * <summary>
         * Gets a Dictionary of all Coords adjacent to specified Coords on the map.
         * The keys of the Dictionary are the sides of the adjacency -- that is,
         * the side of the Hex at the given Coords that the value at that point in
         * the Dictionary is specifically adjacent to (<see cref="Hex.Side"/>).
         * </summary>
         * <remarks>
         * Handles wrap-around adjacency.
         * </remarks>
         * <param name="coords">The Coords to query.</param>
         * <returns>
         * Dictionary of all Coords adjacent to <paramref name="coords"/>, keyed
         * to the hex side on which they are adjacent.
         * </returns>
         */
        public Dictionary<Hex.Side, Coords> GetAllAdjacentCoords(Coords coords)
        {
            Dictionary<Hex.Side, Coords> adjCoords = new Dictionary<Hex.Side, Coords>();

            for (Hex.Side side = 0; side < Hex.Side.Nil; side++)
            {
                Coords maybeCoords = GetAdjacentCoords(coords, side);
                if (!maybeCoords.invalid)
                {
                    adjCoords.Add(side, maybeCoords);
                }
            }

            return adjCoords;
        }

        /**
         * <summary>
         * Gets a Dictionary of all Coords adjacent to specified Coords on the map
         * which are valid for placement (i.e. land can be placed at the Hexes there).
         * Values in the Dictionary are keyed to the side that the value is adjacent
         * on relative to the original coords (see <see cref="GetAllAdjacentCoords(Coords)"/>).
         * </summary>
         * <param name="coords">The Coords to query.</param>
         * <returns>
         * A Dictionary of all Coords adjacent to <paramref name="coords"/> where the
         * corresponding Hex is considered valid for land placement.
         * </returns>
         */
        public Dictionary<Hex.Side, Coords> GetPlaceableAdjacentCoords(Coords coords)
        {
            Dictionary<Hex.Side, Coords> adjCoords = new Dictionary<Hex.Side, Coords>();

            foreach (Hex.Side side in Enum.GetValues(typeof(Hex.Side)))
            {
                Coords maybeCoords = GetAdjacentCoords(coords, side);
                if (!maybeCoords.invalid
                    && CanPlaceAt(maybeCoords))
                {
                    adjCoords.Add(side, maybeCoords);
                }
            }

            return adjCoords;
        }

        /**
         * <summary>
         * Predicate to determine whether two Coords are adjacent on this HexMap.
         * </summary>
         * <remarks>
         * Takes wraparound into account.
         * </remarks>
         * <param name="a">First of the Coords to check.</param>
         * <param name="b">Second of the Coords to check.</param>
         * <returns>
         * True if the Coords <paramref name="a"/> and <paramref name="b"/> are
         * adjacent, false otherwise.
         * </returns>
         */
        public bool IsAdjacent(Coords a, Coords b)
        {
            var allAdj = GetAllAdjacentCoords(a);
            return (allAdj.ContainsValue(b));
        }

        /**
         * <summary>
         * Given two Coords, determines if they are adjacent and, if so, on which
         * side.
         * </summary>
         * <param name="reference">
         * The Coords of the reference Hex to check adjacency. This is the Hex
         * that the returned side will be relative to.
         * </param>
         * <param name="check">The Coordsto check adjacency against.</param>
         * <returns>
         * The Hex.Side of the Hex at <paramref name="reference"/> that
         * the Hex at <paramref name="check"/> is adjacent to, if it is.
         * Returns Hex.Side.Nil otherwise.
         * </returns>
         */
        public Hex.Side GetAdjacentSide(Coords reference, Coords check)
        {
            for (Hex.Side side = Hex.Side.North; side < Hex.Side.Nil; side++)
            {
                if (GetAdjacentCoords(reference, side) == check)
                    return side;
            }
            return Hex.Side.Nil;
        }

        /**
         * <summary>
         * Creates a 1D list of all valid Coords in this HexMap.
         * </summary>
         * <returns>A List of all Coords that are valid in this HexMap.</returns>
         */
        public List<Coords> GetAllCoords()
        {
            List<Coords> rv = new List<Coords>();
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    rv.Add(new Coords(i, j));
                }
            }
            return rv;
        }

        /**
         * <summary>
         * Given some Coords and a Hex.Side, returns the Coords of the Hex that
         * is adjacent to the Hex at the given Coords on that side.
         * </summary>
         * <param name="coords">Coords of the central Hex to check.</param>
         * <param name="side">Side on which to determine adjacency.</param>
         * <returns>
         * Coords of the Hex adjacent to the hex at
         * <paramref name="coords"/> on <paramref name="side"/>.
         * </returns>
         */
        public Coords GetAdjacentCoords(Coords coords, Hex.Side side)
        {
            int x = coords.x;
            int y = coords.y;
            int upShift = 0;
            int downShift = 0;
            if (x % 2 != 0)
                downShift = 1;
            else
                upShift = -1;

            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x > (Width - 1)) x = (Width - 1);
            if (y > (Height - 1)) y = (Height - 1);

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

            if (rv.y < 0 || rv.y > (Height - 1))
            {
                rv.invalid = true;
            }
            if (rv.x < 0)
            {
                rv.x = (Width - 1);
            }
            else if (rv.x > (Width - 1))
            {
                rv.x = 0;
            }

            return rv;
        }

        public AffinityMap GetAffinitiesForLandmass(Landmass landmass)
        {
            if (!Landmasses.Contains(landmass))
            {
                throw new KeyNotFoundException("Tried to get affinities for a landmass that is not in this HexMap.");
            }

            if (landmass.Affinities == null)
            {
                var totalAffinityScore = new Dictionary<string, int>();
                foreach (Coords coords in landmass.Hexes)
                {
                    Hex hex = GetHexAt(coords);
                    AffinityMap affinities = hex.Affinities;
                    foreach (var aspect in affinities.AspectList)
                    {
                        if (!totalAffinityScore.ContainsKey(aspect))
                            totalAffinityScore[aspect] = 0;
                        totalAffinityScore[aspect] += affinities.GetAffinity(aspect);
                    }
                }

                AffinityMap newAffinityMap = new AffinityMap();

                foreach (var aspect in totalAffinityScore.Keys)
                {
                    int score = totalAffinityScore[aspect];
                    int affinity = score / landmass.TotalHexes;
                    // don't allow 0 here; just weak affinities
                    if (affinity == 0)
                    {
                        affinity = (score > 0) ? 1 : -1;
                    }
                    newAffinityMap.SetAffinity(aspect, affinity);
                }

                landmass.Affinities = newAffinityMap;
            }

            return landmass.Affinities;
        }

        /**
         * <summary>
         * ToString override to produce a (somewhat) console-friendly basic
         * representation of the HexMap.
         * </summary>
         * <remarks>
         * Prints the map on its side (such that north is on the left side of the map).
         * </remarks>
         * <returns>
         * A simple console representation of this HexMap, complete with newlines
         * and a "compass rose".
         * </returns>
         */
        public override string ToString()
        {
            string rv = "";
            for (int i = (_map.GetLength(0) - 1); i >= 0; i--)
            {
                string add = "";
                if (i % 2 != 0)
                {
                    add += " ";
                }
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    char disp = _map[i, j].CharDisplay();
                    add += " " + disp;
                }
                rv += add + "\n";
            }

            rv += "<-+";
            return rv;
        }
    }
}
