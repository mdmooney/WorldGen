using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WorldGen
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // List of all hexagons drawn on the map
        // Some small memory overhead for this, but it allows certain
        // operations like recoloring to be much more efficient
        private List<MapHexagon> _mapHexagons;

        private const double DEFAULT_SCALE = 10.0;

        // Dimensions of the map are fixed to 80 columns and 50 rows, for now
        private static readonly int MaxX = 160;
        private static readonly int MaxY = 100;

        private HexMap hexMap;

        public MainWindow()
        {
            InitializeComponent();

            _mapHexagons = new List<MapHexagon>();

            GenerateNewWorld(MaxX, MaxY);

            DrawHexMap(hexMap, hexMap.BaseColorAt);
        }

        private void GenerateNewWorld(int width, int height)
        {
            hexMap = new HexMap(width, height);
            WorldGenerator wGen = new WorldGenerator(hexMap);
            wGen.Generate();
        }

        private void DrawHexMap(HexMap map, Func<Coords, Color> colorMethod, Double scale = DEFAULT_SCALE)
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
                    Coords currCoords = new Coords(i, j);
                    Polygon newHex = CreateHexagon(x, y, colorMethod(currCoords), scale);
                    HexMapGrid.Children.Add(newHex);
                    _mapHexagons.Add(new MapHexagon(currCoords, newHex));
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

        private Polygon CreateHexagon(Double oriX, Double oriY, Color color, Double scale = DEFAULT_SCALE)
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

            PointCollection myPointCollection = new PointCollection
            {
                Point1,
                Point2,
                Point3,
                Point4,
                Point5,
                Point6
            };
            myPolygon.Points = myPointCollection;
            return myPolygon;
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
                HexMapGrid.Children.Add(entryLine);
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
                HexMapGrid.Children.Add(exitLine);
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

        private void RecolorHexMap(Func<Coords, Color> colorMethod)
        {
            foreach (MapHexagon mapHex in _mapHexagons)
            {
                mapHex.Hexagon.Fill = new SolidColorBrush(colorMethod(mapHex.Loc));
            }
        }

        private void LandmassViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(hexMap.BaseColorAt);
        }

        private void HeightViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(hexMap.ElevationColorAt);
        }

        private void NewWorldClick(object sender, RoutedEventArgs e)
        {
            HexMapGrid.Children.Clear();
            _mapHexagons.Clear();
            GenerateNewWorld(MaxX, MaxY);
            DrawHexMap(hexMap, hexMap.BaseColorAt);
        }

        private void TemperatureViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(hexMap.TemperatureColorAt);
        }

        private void HumidityViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(hexMap.HumidityColorAt);
        }

        private void BiomeViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(hexMap.BiomeColorAt);
        }
    }
}
