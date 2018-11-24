using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldGen
{
    class Coords
    {
        public int x;
        public int y;
        public bool invalid;

        public Coords()
        {
            x = y = 0;
            invalid = false;
        }

        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
            invalid = false;
        }

        public Coords(Coords other)
        {
            this.x = other.x;
            this.y = other.y;
            this.invalid = other.invalid;
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

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }
}
