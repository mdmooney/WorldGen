using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class WorldGenerator
    {

        private struct AbstractLandmass
        {
            public int totalHexes;
            public int remainingHexes;
            public bool seeded;
        }

        private HexMap map;
        private List<AbstractLandmass> landmasses;
        const double WORLD_RATIO = 0.30;
        const int MAX_LANDMASSES = 10;


        public WorldGenerator(HexMap map)
        {
            Random rnd = new Random();
            this.map = map;
            int worldTotal = (map.width * map.height);
            double overallTotalDec = (double)worldTotal;
            int totalLandHexes = (int)(overallTotalDec * WORLD_RATIO);
            int remainingHexes = totalLandHexes;

            landmasses = new List<AbstractLandmass>();
            int numLandmasses = rnd.Next(1, MAX_LANDMASSES);

            for (int i = 0; i < numLandmasses; i++)
            {
                AbstractLandmass landmass = new AbstractLandmass();
                int massHexes;
                if (i != numLandmasses - 1)
                {
                    massHexes = rnd.Next(1, remainingHexes / 2);
                }
                else
                {
                    massHexes = remainingHexes;
                }
                Console.WriteLine("Hexes in landmass " + i + ": " + massHexes);

                remainingHexes -= massHexes;
                landmass.totalHexes = landmass.remainingHexes = massHexes;
                landmasses.Add(landmass);
            }
        }

        public void Generate()
        {
            for (int i = 0; i < landmasses.Count; i++)
            {
                AbstractLandmass mass = landmasses[i];
                if (!mass.seeded)
                {
                    int width = map.width;
                    int height = map.height;

                    Random rnd = new Random();

                    List<Coords> unexpanded = new List<Coords>();
                    List<Coords> tempInvalids = new List<Coords>();

                    bool placedSeed = false;
                    while (!placedSeed)
                    {
                        int x = rnd.Next(width);
                        int y = rnd.Next(height);
                        Coords seedCoords = new Coords(x, y);
                        placedSeed = map.PlaceLand(seedCoords, Hex.HexType.NumHexTypes);
                        if (placedSeed)
                        {
                            mass.remainingHexes--;
                            unexpanded.Add(seedCoords);
                        }
                        mass.seeded = true;
                    }

                    while (mass.remainingHexes > 0
                            && unexpanded.Count > 0)
                    {
                        Coords expander = unexpanded[rnd.Next(unexpanded.Count)];

                        Dictionary<Hex.Side, Coords> adj = map.GetAllAdjacentCoords(expander);

                        int roll = rnd.Next(adj.Count + 1) + GetWeightedModifier(mass);
                        List<Hex.Side> contigs = map.FindNContiguousPlaceableSides(expander, roll);

                        // todo: reduce expected number of contigs and retry if we can't find enough contiguous hexes
                        if (contigs.Count > 0)
                        {
                            Hex.Side placeSide = contigs[rnd.Next(contigs.Count)];
                            int count = 0;
                            while (count < roll)
                            {
                                if (adj.ContainsKey(placeSide))
                                {
                                    Coords placeLoc = adj[placeSide];
                                    bool placed = map.PlaceLand(placeLoc);
                                    if (placed)
                                    {
                                        mass.remainingHexes--;
                                        unexpanded.Add(placeLoc);
                                    }
                                }
                                placeSide = Hex.RotateSideClockwise(placeSide);
                                count++;
                            }

                            while (count < 6)
                            {
                                if (adj.ContainsKey(placeSide))
                                {
                                    Coords invalidatedSide = adj[placeSide];
                                }
                                placeSide = Hex.RotateSideClockwise(placeSide);
                                count++;
                            }
                        }

                        unexpanded.Remove(expander);
                    }

                    foreach (Coords remainder in unexpanded)
                    {
                        Dictionary<Hex.Side, Coords> toGo = map.GetPlaceableAdjacentCoords(remainder);
                        foreach (Coords toGoInstance in toGo.Values)
                        {
                            map.SetPlaceable(toGoInstance, false);
                        }
                    }
                }
            }
        }

        private static int GetWeightedModifier(AbstractLandmass mass)
        {
            double ratio = mass.remainingHexes / mass.totalHexes;
            Random rnd = new Random();
            int mod = 0;

            while ((mod < 6)
                    && (ratio > 0))
            {
                double comp = rnd.NextDouble();
                if (comp <= ratio)
                {
                    mod++;
                    ratio /= 2;
                }
                else break;
            }
            return mod;
        }

    }
}
