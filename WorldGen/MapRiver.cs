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
        public Point? StartPoint {
            get
            {
                if (Points.Count > 0)
                    return Points.First();
                return null;
            }
        }

        public MapRiver()
        {
            Points = new PointCollection();
        }
    }
}
