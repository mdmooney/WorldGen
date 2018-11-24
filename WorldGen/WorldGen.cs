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
            public List<Coords> hexes = new List<Coords>();
            public List<Coords> shoreHexes = new List<Coords>();
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

                remainingHexes -= massHexes;
                landmass.totalHexes = landmass.remainingHexes = massHexes;
                landmasses.Add(landmass);
            }
        }

        public void Generate()
        {
            Random rnd = new Random();
            for (int i = 0; i < landmasses.Count; i++)
            {
                AbstractLandmass mass = landmasses[i];
                List<Coords> allCoords = map.GetAllCoords();
                LandmassExpander lEx = new LandmassExpander(map);
                mass.hexes = lEx.Expand(allCoords, mass.totalHexes);
                mass.totalHexes = mass.hexes.Count;

                // Create shore/shallow water hexes adjacent to each hex of this landmass
                foreach (Coords owned in mass.hexes)
                {
                    if (map.BordersOcean(owned))
                    {
                        List<Coords> shoreHexes = map.GetAdjacentOceanHexes(owned);
                        foreach (Coords shoreCoords in shoreHexes)
                        {
                            map.SetTypeAt(shoreCoords, Hex.HexType.Shore);
                            mass.shoreHexes.Add(shoreCoords);
                        }
                    }
                }

                // Elevation
                int passes = rnd.Next(1, 5);
                List<Coords> eleHexes = new List<Coords>(mass.hexes);
                int range = eleHexes.Count;
                
                for (int pass = 1; pass <= passes; pass++)
                {
                    int toElevate = rnd.Next(range / 5, (int)(range * 0.75));
                    if (toElevate == 0) break;

                    List<Coords> elevatedOnThisPass = new List<Coords>(eleHexes);

                    int seedPoints = rnd.Next(1, 5);
                    for (int seedPoint = 0; seedPoint < seedPoints; seedPoint++)
                    {
                        if (toElevate <= 0) break;
                        int elevating = rnd.Next(1, toElevate);
                        HeightExpander hEx = new HeightExpander(map, pass);
                        List<Coords> tempHexes = hEx.Expand(eleHexes, elevating);
                        toElevate -= tempHexes.Count;
                        elevatedOnThisPass.AddRange(tempHexes);
                    }

                    range = elevatedOnThisPass.Count;
                    eleHexes = elevatedOnThisPass;
                }

                // Rivers
                passes = rnd.Next(1, 5);
                int totalRiverHexes = 50;
                for (int pass = 1; pass <= passes; pass++)
                {
                    RiverExpander rEx = new RiverExpander(map);
                    List<Coords> landAndShore = mass.hexes.Union(mass.shoreHexes).ToList();
                    List<Coords> riverHexes = rEx.Expand(landAndShore, totalRiverHexes);
                    totalRiverHexes -= riverHexes.Count;
                }
            }
        }
    }
}
