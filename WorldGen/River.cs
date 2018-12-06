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
    }
}
