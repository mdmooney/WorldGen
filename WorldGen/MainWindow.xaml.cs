using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WorldGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double DEFAULT_SCALE = 10.0;

        public MainWindow()
        {
            InitializeComponent();

            int maxX = 80;
            int maxY = 50;
            
            HexMap testMap = new HexMap(maxX, maxY);
            WorldGenerator wGen = new WorldGenerator(testMap);
            wGen.Generate();

            DrawHexMap(testMap);

            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void DrawHexMap(HexMap map, Double scale = DEFAULT_SCALE)
        {
            Point offset = GetHexagonOffsets(scale);

            double x = 0.0;
            double y = 0.0;

            for (int i = 0; i < map.Width; i++)
            {
                x += scale + offset.X;
                y = (i % 2 != 0) ? offset.Y : 0.0;
                for (int j = 0; j < map.Height; j++)
                {
                    y += (offset.Y * 2);
                    DrawHexagon(x, y, map.ElevationColorAt(i,j), scale);
                    Coords currCoords = new Coords(i, j);
                    if (map.IsRiverAt(currCoords))
                    {
                        RiverSegment seg = map.GetMainRiverSegmentAt(currCoords);
                        Hex.Side? entry = seg.EntrySide;
                        Hex.Side? exit = seg.ExitSide;
                        DrawRiver(x, y, entry, exit, scale);
                    }
                }
            }
        }

        private Point GetHexagonOffsets(Double scale = DEFAULT_SCALE)
        {
            const double ANGLE = 1.0472;  // 60 degrees in radians
            double point2x = Math.Cos(ANGLE) * scale;
            double point2y = Math.Sin(ANGLE) * scale;

            return new Point(point2x, point2y);
        }

        private void DrawHexagon(Double oriX, Double oriY, Color color, Double scale = DEFAULT_SCALE)
        {
            Polygon myPolygon;
            myPolygon = new Polygon()
            {
                Stroke = System.Windows.Media.Brushes.Black,
                Fill = new SolidColorBrush(color),
                StrokeThickness = 0.5,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            // Calculate relative coords of points on angles; this only needs to be done once
            Point offset = GetHexagonOffsets(scale);
            double point2x = offset.X;
            double point2y = offset.Y;

            // Start point is the upper-leftmost point of the hexagon
            double x = oriX;
            double y = oriY;
            System.Windows.Point Point1 = new System.Windows.Point(x, y);
            x += scale;
            System.Windows.Point Point2 = new System.Windows.Point(x, y);
            x += point2x;
            y += point2y;
            System.Windows.Point Point3 = new System.Windows.Point(x, y);
            x -= point2x;
            y += point2y;
            System.Windows.Point Point4 = new System.Windows.Point(x, y);
            x -= scale;
            System.Windows.Point Point5 = new System.Windows.Point(x, y);
            x -= point2x;
            y -= point2y;
            System.Windows.Point Point6 = new System.Windows.Point(x, y);

            PointCollection myPointCollection = new PointCollection();
            myPointCollection.Add(Point1);
            myPointCollection.Add(Point2);
            myPointCollection.Add(Point3);
            myPointCollection.Add(Point4);
            myPointCollection.Add(Point5);
            myPointCollection.Add(Point6);
            myPolygon.Points = myPointCollection;
            hexGrid.Children.Add(myPolygon);
        }

        private void DrawRiver(Double oriX, Double oriY, Hex.Side? entry, Hex.Side? exit, Double scale = DEFAULT_SCALE)
        {
            Point offset = GetHexagonOffsets(scale);

            Point center = new Point(oriX + offset.X, oriY + offset.Y);

            // draw entry line
            if (entry != null)
            {
                Hex.Side entrySide = (Hex.Side)entry;
                Point entryPoint = GetCoordsOfSideMidpoint(oriX, oriY, entrySide, scale);
                Line entryLine = new Line()
                {
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeThickness = 1,
                    X1 = center.X,
                    Y1 = center.Y,
                    X2 = entryPoint.X,
                    Y2 = entryPoint.Y
                };
                hexGrid.Children.Add(entryLine);
            }

            // draw exit line, if there is one
            if (exit != null)
            {
                Hex.Side exitSide = (Hex.Side)exit;
                Point exitPoint = GetCoordsOfSideMidpoint(oriX, oriY, exitSide, scale);
                Line exitLine  = new Line()
                {
                    Stroke = System.Windows.Media.Brushes.Blue,
                    StrokeThickness = 1,
                    X1 = center.X,
                    Y1 = center.Y,
                    X2 = exitPoint.X,
                    Y2 = exitPoint.Y
                };
                hexGrid.Children.Add(exitLine);
            }
        }

        // Helper method to get the coordinates of the middle of a given side from origin coords
        // (upper-left point of the hexagon)
        private Point GetCoordsOfSideMidpoint(Double oriX, Double oriY, Hex.Side side, Double scale = DEFAULT_SCALE)
        {
            Point offset = GetHexagonOffsets(scale);
            Point rv = new Point(oriX, oriY);
            switch (side)
            {
                case Hex.Side.North:
                    rv.X += (scale / 2.0);
                    break;
                case Hex.Side.Northeast:
                    rv.X += scale;
                    rv.X += (offset.X / 2.0);
                    rv.Y += (offset.X / 2.0);
                    break;
                case Hex.Side.Northwest:
                    rv.X -= (offset.X / 2.0);
                    rv.Y += (offset.X / 2.0);
                    break;
                case Hex.Side.South:
                    rv.X += (scale / 2.0);
                    rv.Y += (scale * 2);
                    break;
                case Hex.Side.Southeast:
                    rv.X += scale;
                    rv.X += (offset.X / 2.0);
                    rv.Y += scale;
                    rv.Y += (offset.X / 2.0);
                    break;
                case Hex.Side.Southwest:
                    rv.X -= (offset.X / 2.0);
                    rv.Y += scale;
                    rv.Y += (offset.X / 2.0);
                    break;
                default:
                    break;
            }
            return rv;
        }
    }
}
