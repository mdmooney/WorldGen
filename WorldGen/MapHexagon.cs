using System.Windows.Shapes;

namespace WorldGen
{
    /// <summary>
    /// Simple struct for tracking polygons and their location on the internal
    /// grid. Used for easy recoloring, etc. of hexagons on the display panel.
    /// </summary>
    struct MapHexagon
    {
        public Coords Loc;
        public Polygon Hexagon;

        public MapHexagon(Coords loc, Polygon hexagon)
        {
            Loc = loc;
            Hexagon = hexagon;
        }
    }
}
