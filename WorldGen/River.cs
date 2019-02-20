namespace WorldGen
{
    /**
     * <summary>
     * Class representing a river overall.
     * 
     * This is not (currently) a container for RiverSegments, though it does
     * keep track of the first and final segments of the river it indicates.
     * It is primarily meant to give RiverSegments something to point to that
     * indicates which overall river they belong to.
     * </summary>
     */
    class River
    {
         /// <summary>
         /// Name of this River.
         /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// First RiverSegment of this river.
        /// </summary>
        public RiverSegment FirstSeg { get; set; }

        /// <summary>
        /// Final RiverSegment of this river.
        /// </summary>
        public RiverSegment LastSeg { get; set; }

        private int _count = -1;
        public int Count
        {
            get
            {
                // Only count when requested, and only do it once; rivers are not
                // expected to change after creation
                if (_count < 0)
                {
                    if (FirstSeg == null)
                    {
                        _count = 0;
                    }
                    else
                    {
                        RiverSegment seg = FirstSeg;
                        while ((seg.NextSegment != null))
                        {
                            _count++;
                            seg = seg.NextSegment;
                        }
                    }
                }
                return _count;
            }
        }

        /**
         * <summary>
         * Base constructor simply initializes a river with the very creative
         * default name "River."
         * </summary>
         */
        public River()
        {
            Name = "River";
        }

        /**
         * <summary>
         * Constructor to give a river a proper name, other than "River."
         * </summary>
         * <param name="name">The name to give this river.</param>
         */
        public River(string name)
        {
            Name = name;
        }

        public void Extend(Hex.Side side, Coords coords)
        {
            RiverSegment newSeg = new RiverSegment(this, coords);
            LastSeg.ExitSide = side;
            newSeg.EntrySide = Hex.OppositeSide(side);
            newSeg.PrevSegment = LastSeg;
            LastSeg.NextSegment = newSeg;
            LastSeg = newSeg;
        }

        public void Terminate(Hex.Side side)
        {
            LastSeg.ExitSide = side;
        }

        public River(Coords start, string name = "River")
        {
            Name = name;
            RiverSegment seg = new RiverSegment(this, start);
            FirstSeg = LastSeg = seg;
        }
    }
}
