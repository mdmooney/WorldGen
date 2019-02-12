﻿using System;
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

        private HexMap map;

        private Random rnd;

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
        public WorldGenerator(HexMap map)
        {
            rnd = new Random();
            this.map = map;
            int worldTotal = (map.Width * map.Height);
            double overallTotalDec = (double)worldTotal;
            int totalLandHexes = (int)(overallTotalDec * WORLD_RATIO);
            int remainingHexes = totalLandHexes;

            var landmasses = new List<Landmass>();
            int numLandmasses = rnd.Next(1, MAX_LANDMASSES);

            for (int i = 0; i < numLandmasses; i++)
            {
                Landmass landmass = new Landmass();
                int massHexes;
                if (i != numLandmasses - 1)
                {
                    massHexes = rnd.Next(1, remainingHexes / 2);
                }
                else
                {
                    massHexes = remainingHexes;
                }

                remainingHexes -= massHexes;
                landmass.TotalHexes = landmass.RemainingHexes = massHexes;
                landmasses.Add(landmass);
            }

            this.map.Landmasses = landmasses;
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
            for (int i = 0; i < map.Landmasses.Count; i++)
            {
                Landmass mass = map.Landmasses[i];
                List<Coords> allCoords = map.GetAllCoords();
                LandmassExpander lEx = new LandmassExpander(map);
                mass.Hexes = lEx.Expand(allCoords, mass.TotalHexes);
                mass.TotalHexes = mass.Hexes.Count;

                // Create shore/shallow water hexes adjacent to each hex of this landmass
                foreach (Coords owned in mass.Hexes)
                {
                    if (map.BordersOcean(owned))
                    {
                        List<Coords> shoreHexes = map.GetAdjacentOceanHexes(owned);
                        foreach (Coords shoreCoords in shoreHexes)
                        {
                            map.SetTypeAt(shoreCoords, Hex.HexType.Shore);
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
            int passes = rnd.Next(1, 5);
            List<Coords> eleHexes = new List<Coords>(mass.Hexes);
            HeightExpander hEx = new HeightExpander(map);
            LayeredExpansion layered = new LayeredExpansion(hEx, eleHexes, 0.6, 0.8);
            layered.Expand(passes);

            // Rivers
            passes = rnd.Next(1, 20);
            int totalRiverHexes = mass.TotalHexes / 20;
            for (int pass = 1; pass <= passes; pass++)
            {
                double fraction = rnd.NextDouble();
                int riverHexesThisRound = (int)(totalRiverHexes * fraction);
                RiverExpander rEx = new RiverExpander(map);
                List<Coords> landAndShore = mass.Hexes.Union(mass.ShoreHexes).ToList();
                List<Coords> riverHexes = rEx.Expand(landAndShore, totalRiverHexes);
                totalRiverHexes -= riverHexes.Count;
            }

            // Temperature
            SetTemperatures();

            // Humidity
            passes = rnd.Next(1, 5);
            List<Coords> humiHexes = new List<Coords>(mass.Hexes);
            HumidityExpander humEx = new HumidityExpander(map);
            layered = new LayeredExpansion(humEx, humiHexes, 0.6, 0.8);
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
                    double fraction = rnd.NextDouble() / 2.0;
                    expandThisRound = (int)(expandThisRound * fraction);
                }

                bioEx = new BiomeExpander(map);
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
            int equatorRow = map.Height / 2;

            // get gradient step
            int gradStep = (DefaultEquatorTemp - DefaultPoleTemp) / (equatorRow);
            bool stepFlipped = false;

            for (int y = 0, currTemp = DefaultPoleTemp; y < map.Height; y++)
            {
                // flip the temperature gradient after we hit the equator
                if (!stepFlipped
                    && (y >= equatorRow))
                {
                    gradStep *= -1;
                    stepFlipped = true;
                }

                // set temperature of all hexes according to thresholds
                for (int x = 0; x < map.Width; x++)
                {
                    Coords here = new Coords(x, y);
                    Hex.ElevationLevel el = map.GetElevationAt(here);
                    int heightAdjust = (int)el * TempHeightAdjust;
                    int hexTemp = currTemp + adjust + heightAdjust;

                    if (hexTemp > TempThreshHot)
                        map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Hot);
                    else if (hexTemp > TempThreshWarm)
                        map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Warm);
                    else if (hexTemp < TempThreshCold)
                        map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Cold);
                    else if (hexTemp < TempThreshCool)
                        map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Cool);
                    else
                        map.SetTemperatureAt(new Coords(x, y), Hex.TemperatureLevel.Temperate);
                }

                currTemp += gradStep;
            }
        }
    }
}
