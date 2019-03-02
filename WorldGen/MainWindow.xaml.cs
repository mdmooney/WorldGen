using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
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
        public ObservableCollection<MapHexagon> MapHexagons { get; set; }
        public ObservableCollection<MapRiver> MapRivers { get; set; }
        public CompositeCollection MapObjects { get; set; }

        private const double DEFAULT_SCALE = 10.0;

        public double Scale { get; set; }

        // Dimensions of the map are fixed to 80 columns and 50 rows, for now
        private static readonly int MaxX = 80;
        private static readonly int MaxY = 50;

        private World _world;

        private WorldGenerator _worldGen;

        public MainWindow()
        {
            DataContext = this;

            MapObjects = new CompositeCollection();
            MapHexagons = new ObservableCollection<MapHexagon>();
            MapRivers = new ObservableCollection<MapRiver>();
            MapObjects.Add(new CollectionContainer() { Collection = MapHexagons });
            MapObjects.Add(new CollectionContainer() { Collection = MapRivers });

            GenerateNewWorld(MaxX, MaxY);
            Scale = DEFAULT_SCALE;
            
            InitializeComponent();

            DrawHexMap(_world.Map, _world.Map.BaseColorAt);
        }

        private void GenerateNewWorld(int width, int height)
        {
            _world = new World(width, height);
            _worldGen = new WorldGenerator(_world, _world.RandomGenerator);
            _worldGen.Generate();
        }

        private void DrawHexMap(HexMap map, Func<Coords, Color> colorMethod, Double scale = DEFAULT_SCALE)
        {
            Point offset = GetHexagonOffsets();

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
                    MapHexagon newMapHex = CreateHexagon(x, y, colorMethod(currCoords), currCoords);
                    newMapHex.ChangeColor(map.BaseColorAt(currCoords));
                    MapHexagons.Add(newMapHex);
                    if (map.IsRiverAt(currCoords))
                    {
                        RiverSegment seg = map.GetMainRiverSegmentAt(currCoords);
                        Hex.Side? entry = seg.EntrySide;
                        Hex.Side? exit = seg.ExitSide;
                        //DrawRiver(x, y, entry, exit, scale);
                        DrawRivers();
                    }
                }
            }
        }

        public Point PointFromCoords(Coords coords)
        {
            Point offset = GetHexagonOffsets();
            double x = coords.x;
            double y = coords.y;
            x++;
            y++;

            x *= (Scale + offset.X);
            y *= (offset.Y * 2);
            y += (coords.x % 2 != 0) ? offset.Y : 0.0;

            return new Point(x, y);
        }

        public Point CenterPointFromCoords(Coords coords)
        {
            Point offset = GetHexagonOffsets();
            double x = coords.x;
            double y = coords.y;
            x++;
            y++;

            x *= (Scale + offset.X);
            x += offset.X;
            y *= (offset.Y * 2);
            y += offset.Y;
            y += (coords.x % 2 != 0) ? offset.Y : 0.0;

            return new Point(x, y);
        }

        private Point GetHexagonOffsets()
        {
            const double ANGLE = 1.0472;  // 60 degrees in radians
            double point2x = Math.Cos(ANGLE) * Scale;
            double point2y = Math.Sin(ANGLE) * Scale;

            return new Point(point2x, point2y);
        }

        private MapHexagon CreateHexagon(Double oriX, Double oriY, Color color, Coords mapCoords)
        {
            MapHexagon mapHex = new MapHexagon(mapCoords);

            // Calculate relative coords of points on angles; this only needs to be done once
            Point offset = GetHexagonOffsets();
            double point2x = offset.X;
            double point2y = offset.Y;

            // Start point is the upper-leftmost point of the hexagon
            double x = oriX;
            double y = oriY;
            System.Windows.Point Point1 = new System.Windows.Point(x, y);
            x += Scale;
            System.Windows.Point Point2 = new System.Windows.Point(x, y);
            x += point2x;
            y += point2y;
            System.Windows.Point Point3 = new System.Windows.Point(x, y);
            x -= point2x;
            y += point2y;
            System.Windows.Point Point4 = new System.Windows.Point(x, y);
            x -= Scale;
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
            mapHex.Points = myPointCollection;
            return mapHex;
        }

        private void DrawRivers(Double scale = DEFAULT_SCALE)
        {
            foreach (var river in _world.Rivers)
            {
                RiverSegment seg = river.FirstSeg;
                MapRiver mr = new MapRiver();

                mr.Points.Add(CenterPointFromCoords(seg.Location));

                while ((seg.NextSegment != null))
                {
                    if (CoordsWrap(seg.Location, seg.NextSegment.Location))
                    {
                        Point loc = PointFromCoords(seg.Location);
                        Point exitPoint = GetCoordsOfSideMidpoint(loc.X, loc.Y, (Hex.Side)seg.ExitSide);
                        mr.Points.Add(exitPoint);
                        MapRivers.Add(mr);
                        mr = new MapRiver();
                        loc = PointFromCoords(seg.NextSegment.Location);
                        Point entryPoint = GetCoordsOfSideMidpoint(loc.X, loc.Y, (Hex.Side)seg.NextSegment.EntrySide);
                        mr.Points.Add(entryPoint);
                    }
                    seg = seg.NextSegment;
                    mr.Points.Add(CenterPointFromCoords(seg.Location));
                }

                if (river.LastSeg.ExitSide != null)
                {
                    Hex.Side exitSide = (Hex.Side)river.LastSeg.ExitSide;
                    Point loc = PointFromCoords(river.LastSeg.Location);
                    Point sideMidpoint = GetCoordsOfSideMidpoint(loc.X, loc.Y, exitSide);
                    mr.Points.Add(sideMidpoint);
                }

                MapRivers.Add(mr);
            }
        }

        public bool CoordsWrap(Coords a, Coords b)
        {
            return ((a.x == (_world.Width - 1) && b.x == 0)
                    || (b.x == (_world.Width - 1) && a.x == 0));
        }

        // Helper method to get the coordinates of the middle of a given side from origin coords
        // (upper-left point of the hexagon)
        private Point GetCoordsOfSideMidpoint(Double oriX, Double oriY, Hex.Side side, Double scale = DEFAULT_SCALE)
        {
            Point offset = GetHexagonOffsets();
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
            foreach (MapHexagon mapHex in MapHexagons)
            {
                mapHex.ChangeColor(colorMethod(mapHex.Loc));
            }
        }

        private void LandmassViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(_world.Map.BaseColorAt);
        }

        private void HeightViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(_world.Map.ElevationColorAt);
        }

        private void NewWorldClick(object sender, RoutedEventArgs e)
        {
            MapHexagons.Clear();
            MapRivers.Clear();
            GenerateNewWorld(MaxX, MaxY);
            DrawHexMap(_world.Map, _world.Map.BaseColorAt);
        }

        private void TemperatureViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(_world.Map.TemperatureColorAt);
        }

        private void HumidityViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(_world.Map.HumidityColorAt);
        }

        private void BiomeViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(_world.Map.BiomeColorAt);
        }

        private void PopViewClick(object sender, RoutedEventArgs e)
        {
            RecolorHexMap(_world.PopColorAt);
        }

        private void RaceGenClick(object sender, RoutedEventArgs e)
        {
            _worldGen.GenerateRace();
        }
    }
}
