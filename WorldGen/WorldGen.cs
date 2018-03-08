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
        const double WORLD_RATIO = 0.29;

        int worldTotal;
        int overallTotal;
        int overallRemaining;

        public WorldGenerator(HexMap map)
        {
            this.map = map;
            worldTotal = (map.width * map.height);
            double overallTotalDec = (double)worldTotal;
            overallTotal = (int)(overallTotalDec * WORLD_RATIO);
            overallRemaining = overallTotal;
            landmasses = new List<AbstractLandmass>();
            int half = overallTotal / 2;

            AbstractLandmass mass1 = new AbstractLandmass();
            mass1.totalHexes = half;
            mass1.remainingHexes = half;
            half = (overallTotal - half);
            AbstractLandmass mass2 = new AbstractLandmass();
            mass2.totalHexes = half;
            mass2.remainingHexes = half;

            landmasses.Add(mass1);
            landmasses.Add(mass2);
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
                        placedSeed = map.PlaceLand(seedCoords);
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

                        int roll = rnd.Next(adj.Count + 1);
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

    }
}
