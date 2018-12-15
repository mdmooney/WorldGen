using System.Windows.Shapes;
using System.Windows;
using System.Windows.Media;

namespace WorldGen
{
    /// <summary>
    /// Simple struct for tracking polygons and their location on the internal
    /// grid. Used for easy recoloring, etc. of hexagons on the display panel.
    /// </summary>
    public class MapHexagon
    {
        public int X { get { return Loc.x; } }
        public int Y { get { return Loc.y; } }
        public Coords Loc { get; set; }
        public PointCollection Points { get; set; }
        public SolidColorBrush HexColor { get; }

        public MapHexagon(Coords loc)
        {
            Loc = loc;
            Points = new PointCollection();
            HexColor = new SolidColorBrush();
        }

        public void ChangeColor(Color color)
        {
            HexColor.Color = color;
        }
    }
}
