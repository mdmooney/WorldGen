using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldGen
{
    struct Coords
    {
        public int x;
        public int y;
        public bool invalid;

        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
            invalid = false;
        }

        public bool Equals(Coords other)
        {
            return Equals(other, this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var objectToCompareWith = (Coords)obj;

            return objectToCompareWith.x == x
                   && objectToCompareWith.y == y;

        }

        public static bool operator ==(Coords c1, Coords c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(Coords c1, Coords c2)
        {
            return !c1.Equals(c2);
        }
    }
}
