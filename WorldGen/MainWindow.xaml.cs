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
        public MainWindow()
        {
            InitializeComponent();

            int maxX = 80;
            int maxY = 50;

            Hex[,] testArray = new Hex[maxX, maxY];
            for (int i = 0; i < maxX; i++)
            {
                for (int j = 0; j < maxY; j++)
                {
                    testArray[i, j] = new Hex();
                }
            }

            HexMap testMap = new HexMap(maxX, maxY);
            //testMap.PlaceLand(new Coords(maxX/2, 0));
            WorldGenerator wGen = new WorldGenerator(testMap);
            wGen.Generate();

            DrawHexMap(testMap, 5.0);

            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void DrawHexMap(HexMap map, Double scale = 10.0)
        {
            Point offset = GetHexagonOffsets(scale);

            double x = 0.0;
            double y = 0.0;

            for (int i = 0; i < map.width; i++)
            {
                x += scale + offset.X;
                y = (i % 2 != 0) ? offset.Y : 0.0;
                for (int j = 0; j < map.height; j++)
                {
                    y += (offset.Y * 2);
                    DrawHexagon(x, y, map.ElevationColorAt(i,j), scale);
                }
            }
        }

        private Point GetHexagonOffsets(Double scale = 10.0)
        {
            const double ANGLE = 1.0472;  // 60 degrees in radians
            double point2x = Math.Cos(ANGLE) * scale;
            double point2y = Math.Sin(ANGLE) * scale;

            return new Point(point2x, point2y);
        }

        private void DrawHexagon(Double oriX, Double oriY, Color color, Double scale = 10.0)
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
    }
}
