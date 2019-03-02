using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorldGen
{
    /**
     * <summary>
     * WorldGenerator class, handling the generation of an entire world from
     * start to finish.
     * 
     * This may be split up or otherwise changed radically in the future as the
     * generation process gets more complex.
     * </summary>
     */
    partial class WorldGenerator
    {
        private World _world;

        private IRandomGen _rand;

        private RaceGen _raceGen;

        // Ratio of land:water in this world. Currently a constant at 3:7,
        // but will be alterable by the user later.
        const double WORLD_RATIO = 0.30;

        // Maximum number of landmasses permitted in this world.
        const int MAX_LANDMASSES = 10;

        // Temperature thresholds, in arbitrary units. These define the
        // ranges for hex temperature determination; see SetTemperatures().
        private static readonly int TempThreshHot = 75;
        private static readonly int TempThreshWarm = 37;
        private static readonly int TempThreshCool = -37;
        private static readonly int TempThreshCold = -75;

        // Default arbitrary temperature units for the poles and equator.
        private static readonly int DefaultPoleTemp = -100;
        private static readonly int DefaultEquatorTemp = 100;

        // Arbitrary temperature unit adjustment per additional level of elevation.
        private static readonly int TempHeightAdjust = -7;

        /**
         * <summary>
         * WorldGenerator constructor requires a HexMap, which will hold the
         * world that is about to be generated.
         * 
         * Does some basic setup, like determining how many landmasses will
         * exist in the world and how many Hexes they should contain, but does
         * not perform any actual generation.
         * </summary>
         * <param name="map">HexMap to generate a world in.</param>
         */
        public WorldGenerator(World world, IRandomGen rand)
        {
            _rand = rand;
            _world = world;
            int worldTotal = (_world.Map.Width * _world.Map.Height);
            double overallTotalDec = (double)worldTotal;
            int totalLandHexes = (int)(overallTotalDec * WORLD_RATIO);
            int remainingHexes = totalLandHexes;

            var landmasses = new List<Landmass>();
            int numLandmasses = _rand.GenerateInt(1, MAX_LANDMASSES);

            for (int i = 0; i < numLandmasses; i++)
            {
                Landmass landmass = new Landmass(rand);
                int massHexes;
                if (i != numLandmasses - 1)
                {
                    massHexes = _rand.GenerateInt(1, remainingHexes / 2);
                }
                else
                {
                    massHexes = remainingHexes;
                }

                remainingHexes -= massHexes;
                landmass.TotalHexes = landmass.RemainingHexes = massHexes;
                landmasses.Add(landmass);
            }

            _world.Map.Landmasses = landmasses;
        }

        /**
         * <summary>
         * Primary work method of this class, generating the entire world in
         * the given HexMap.
         * 
         * This will very likely be split into smaller methods in the near future.
         * </summary>
         */
        public void Generate()
        {
            List<Task> taskList = new List<Task>();
            for (int i = 0; i < _world.Map.Landmasses.Count; i++)
            {
                Landmass mass = _world.Map.Landmasses[i];
                List<Coords> allCoords = _world.Map.GetAllCoords();
                LandmassExpander lEx = new LandmassExpander(_rand, _world.Map);
                mass.Hexes = lEx.Expand(allCoords, mass.TotalHexes);
                mass.TotalHexes = mass.Hexes.Count;

                // Create shore/shallow water hexes adjacent to each hex of this landmass
                foreach (Coords owned in mass.Hexes)
                {
                    if (_world.Map.BordersOcean(owned))
                    {
                        List<Coords> shoreHexes = _world.Map.GetAdjacentOceanHexes(owned);
                        foreach (Coords shoreCoords in shoreHexes)
                        {
                            _world.Map.SetTypeAt(shoreCoords, Hex.HexType.Shore);
                            mass.ShoreHexes.Add(shoreCoords);
                        }
                    }
                }
                Task t = Task.Run(() => FillLandmass(mass));
                taskList.Add(t);
            }
            Task.WaitAll(taskList.ToArray());
        }

        private void FillLandmass(Landmass mass)
        {
            // Elevation
            // Pick a number of passes; a range of the total number of
            // elements in the enum works best
            int passes = _rand.GenerateInt(1, 5);
            List<Coords> eleHexes = new List<Coords>(mass.Hexes);
            HeightExpander hEx = new HeightExpander(_rand, _world.Map);
            LayeredExpansion layered = new LayeredExpansion(_rand, hEx, eleHexes, 0.6, 0.8);
            layered.Expand(passes);

            // Rivers
            int totalRiverHexes = mass.TotalHexes / 20;
            int remainingHexes = totalRiverHexes;
            while (remainingHexes > 0)
            {
                if (remainingHexes == 1)
                    break;
                int riverLength = _rand.GenerateInt(2, remainingHexes);
                RiverGen rgen = new RiverGen(_rand, _world, mass, riverLength);
                int genLength = rgen.Generate();
                if (genLength > 1)
                {
                    remainingHexes -= genLength;
                    rgen.Commit();
                    _world.Rivers.Add(rgen.GenRiver);
                }
            }


            // Temperature
            SetTemperatures();

            // Humidity
            passes = _rand.GenerateInt(1, 5);
            List<Coords> humiHexes = new List<Coords>(mass.Hexes);
            HumidityExpander humEx = new HumidityExpander(_rand, _world.Map);
            layered = new LayeredExpansion(_rand, humEx, humiHexes, 0.6, 0.8);
            layered.Expand(passes);

            // Biomes
            BiomeExpander bioEx;
            List<Coords> bioHexes = new List<Coords>(mass.Hexes);
            int tenPercent = mass.TotalHexes / 10;

            while (bioHexes.Count > 0)
            {
                int expandThisRound = bioHexes.Count;
                if (expandThisRound > tenPercent)
                {
                    double fraction = _rand.GenerateDouble() / 2.0;
                    expandThisRound = (int)(expandThisRound * fraction);
                }

                bioEx = new BiomeExpander(_rand, _world.Map, _world.Biomes);
                var placedCoords = bioEx.Expand(bioHexes, expandThisRound);
                bioHexes.RemoveAll(x => placedCoords.Contains(x));
            }
        }

        /**
         * <summary>
         * Sets temperatures for hexes in the nascent world.
         * </summary>
         * <remarks>
         * Temperature is based on three things:
         *  - Latitude of the hex, with hexes closer to the equator (map
         *      center) being warmer, and hexes closer to the poles (top and
         *      bottom of the map) being cooler.
         *  - Adjustment value. Positive values will increase the overall
         *      temperature of the world, and negative values will decrease the
         *      overall temperature.
         *  - Altitude (ElevationLevel) of the hex, with higher elevations
         *      lowering the temperature of that hex.
         * </remarks>
         * <param name="adjust">
         * Adjustment value, added to the arbitrary
         * temperature of the hex before it is fit into one of the temperature
         * level thresholds. Positive values will increase the temperature, and
         * negative values will lower it. At extremes (20 and above, or -20 and
         * below) some temperature levels will not exist in the world at all.
         * </param>
         */
        public void SetTemperatures(int adjust = 0)
        {
            // first locate the equator
            int equatorRow = _world.Map.Height / 2;

            // get gradient step
            int gradStep = (DefaultEquatorTemp - DefaultPoleTemp) / (equatorRow);
            bool stepFlipped = false;

            for (int y = 0, currTemp = DefaultPoleTemp; y < _world.Map.Height; y++)
            {
                // flip the temperature gradient after we hit the equator
                if (!stepFlipped
                    && (y >= equatorRow))
                {
                    gradStep *= -1;
                    stepFlipped = true;
                }

                // set temperature of all hexes according to thresholds
                for (int x = 0; x < _world.Map.Width; x++)
                {
                    Coords here = new Coords(x, y);
                    Hex.ElevationLevel el = _world.Map.GetElevationAt(here);
                    int heightAdjust = (int)el * TempHeightAdjust;
                    int hexTemp = currTemp + adjust + heightAdjust;

                    if (hexTemp > TempThreshHot)
                        _world.Map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Hot);
                    else if (hexTemp > TempThreshWarm)
                        _world.Map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Warm);
                    else if (hexTemp < TempThreshCold)
                        _world.Map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Cold);
                    else if (hexTemp < TempThreshCool)
                        _world.Map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Cool);
                    else
                        _world.Map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Temperate);
                }

                currTemp += gradStep;
            }
        }

        public void GenerateRace()
        {
            if (_raceGen == null)
                _raceGen = new RaceGen();

            if (_raceGen.IsEmpty())
                return;

            Landmass mass = _world.Map.GetRandomLandmass();
            AffinityMap massAffinity = _world.Map.GetAffinitiesForLandmass(mass);
            Race genRace = _raceGen.GenerateRace(massAffinity);
            Console.WriteLine("Rolled race: " + genRace);
            Console.WriteLine(genRace.Affinities);
            
            // arbitrarily low number to start with
            int highest = -50;
            // random coordinates as default
            Coords candidate = mass.RandomCoords();
            int tick = 0;
            int tenPercent = mass.Count / 10;
            bool tenPercentMore = false;
            foreach (Coords coords in mass.CoordsFromRandomPoint())
            {
                if (tick >= tenPercent)
                    break;
                AffinityMap hexAffinities = _world.Map.GetAffinitiesForCoords(coords);
                int sim = genRace.Affinities.GetSimilarityTo(hexAffinities);
                if (sim > highest)
                {
                    highest = sim;
                    candidate = coords;
                }
                if (!tenPercentMore && highest > 0)
                    tenPercentMore = true;
                if (tenPercentMore)
                {
                    tick++;
                }
            }
            Console.WriteLine("Placing at : " + candidate);
            Console.WriteLine(_world.Map.GetAffinitiesForCoords(candidate));
            Console.WriteLine("--> Final Score: " + highest);
            Console.WriteLine("------------------");
            _world.PlaceRaceAt(genRace, candidate);
        }
    }
}
