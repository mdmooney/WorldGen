using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WorldGen
{
    public class MapRiver
    {
        public PointCollection Points { get; set; }
        public Point StartPoint { get { return Points.First(); } }

        public MapRiver()
        {
            Points = new PointCollection();
        }
    }
}
