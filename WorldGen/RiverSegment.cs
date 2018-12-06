namespace WorldGen
{
    /**
     * <summary>
     * Class representing one hex-sized segment of a river.
     * 
     * These are basically doubly-linked lists (as it may be necessary to
     * traverse a river either in the direction that it is flowing, or the
     * reverse direction).
     * 
     * RiverSegments also have a parent River object, that indicates which
     * overall River they belong to.
     * </summary>
     */
    class RiverSegment
    {
        /// <summary>
        /// The side (of the containing Hex) that the RiverSegment enters from.
        /// May be null if this is the first segment of the river.
        /// </summary>
        public Hex.Side? EntrySide { get; set; }

        /// <summary>
        /// The side (of the containing Hex) that the RiverSegment exits from.
        /// May be null if this is the last segment of the river.
        /// </summary>
        public Hex.Side? ExitSide { get; set; }

        /// <summary>
        /// Previous RiverSegment of this river (upstream).
        /// May be null if this is the first segment of the river.
        /// </summary>
        public RiverSegment PrevSegment { get; set; }

        /// <summary>
        /// Next RiverSegment of this river (downstream).
        /// May be null if this is the last segment of the river.
        /// </summary>
        public RiverSegment NextSegment { get; set; }

        /// <summary>
        /// River that this RiverSegment belongs to.
        /// Should not be null.
        /// </summary>
        public River ParentRiver { get; private set; }

        /**
        * <summary>
        * Sole constructor for a RiverSegment just takes a River to make this
        * segment a part of.
        * </summary>
        * <param name="river">The River this RiverSegment is part of.</param>
        */
        public RiverSegment(River river)
        {
            ParentRiver = river;
        }
    }
}
