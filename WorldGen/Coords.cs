using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldGen
{
    /**
     * <summary>
     * Coords class, representing a pair of coordinates on a hex map.
     * </summary>
     */
    public class Coords
    {
        public int x;
        public int y;

        // This flag indicates whether this instance point to a valid location
        // or not. As this class does not know anything about the map, this will
        // always be set to "false" (representing a valid Coords object) by this
        // class, but it can be modified by client code.
        public bool invalid;

        /**
         * <summary>
         * Default constructor initializes both x and y to 0.
         * </summary>
         */
        public Coords()
        {
            x = y = 0;
            invalid = false;
        }

        /**
         * <summary>
         * Constructor to make a Coords with predetermined x and y values.
         * </summary>
         * <param name="x">The x value for this Coords object.</param>
         * <param name="y">The y value for this Coords object.</param>
         */
        public Coords(int x, int y)
        {
            this.x = x;
            this.y = y;
            invalid = false;
        }

        /**
         * <summary>
         * Copy constructor copies x and y, as well as validity flag.
         * </summary>
         * <param name="other">The Coords object to copy.</param>
         */
        public Coords(Coords other)
        {
            this.x = other.x;
            this.y = other.y;
            this.invalid = other.invalid;
        }

        /**
         * <summary>
         * Equality comparison for two Coords objects. Two Coords instances are considered
         * equal if the x and y values are both equal.
         * </summary>
         * <param name="other">The other Coords object with which to check equality.</param>
         * <returns>True if the Coords have equal x and y, false otherwise.</returns>
         */
        public bool Equals(Coords other)
        {
            return Equals(other, this);
        }


        // ------------ Overrides ------------

        /**
         * <summary>
         * Equality comparator override. Calls <see cref="Equals(Coords)"/>.
         * </summary>
         * <param name="obj">Other object against which to check equality.</param>
         * <returns>True if both objects are Coords and have equal x and y, false otherwise.</returns>
         */
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

        /**
         * <summary>
         * == operator override. Calls <see cref="Equals(Coords)"/>.
         * </summary>
         * <param name="c1">First Coords object to compare.</param>
         * <param name="c2">Second Coords object to compare.</param>
         * <returns>True if c1.Equals(c2) is true. False otherwise.</returns>
         */
        public static bool operator ==(Coords c1, Coords c2)
        {
            return c1.Equals(c2);
        }

        /**
         * <summary>
         * != operator override. Calls <see cref="Equals(Coords)"/> and inverts the result.
         * </summary>
         * <param name="c1">First Coords object to compare.</param>
         * <param name="c2">Second Coords object to compare.</param>
         * <returns>True if c1.Equals(c2) is false. False otherwise.</returns>
         */
        public static bool operator !=(Coords c1, Coords c2)
        {
            return !c1.Equals(c2);
        }

        /**
         * <summary>
         * String representation override. Displays the Coords as a pair of x and y
         * coordinates in parentheses.
         * </summary>
         * <returns>String representation of the Coords in the form "(x, y)".</returns>
         */
        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + x;
            hash = hash * 31 + y;
            return hash;
        }
    }
}
