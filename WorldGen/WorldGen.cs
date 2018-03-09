using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGen
{
    class WorldGenerator
    {

        private class AbstractLandmass
        {
            public int totalHexes;
            public int remainingHexes;
            public bool seeded;
            public List<Coords> hexes = new List<Coords>();
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
                List<Coords> allCoords = map.GetAllCoords();
                LandmassExpander lEx = new LandmassExpander(map);
                mass.hexes = lEx.Expand(allCoords, mass.totalHexes);
                
                foreach (Coords owned in mass.hexes)
                {
                    if (map.BordersOcean(owned))
                    {
                        List<Coords> shoreHexes = map.GetAdjacentOceanHexes(owned);
                        foreach (Coords shoreCoords in shoreHexes)
                        {
                            map.SetTypeAt(shoreCoords, Hex.HexType.Shore);
                        }
                    }
                }

            }
        }

        private void GenerateElevations()
        {

        }

    }
}
