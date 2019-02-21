using System;

namespace WorldGen
{
    public class InvalidAspectException : Exception
    {
        public InvalidAspectException()
        {

        }

        public InvalidAspectException(string message) : base(message)
        {
        }

        public InvalidAspectException(string message, Exception inner) : base(message, inner)
        {
        }

        public static InvalidAspectException FromAspect(string aspect)
        {
            string msg = "Invalid aspect: " + aspect;
            return new InvalidAspectException(msg);
        }

        public static InvalidAspectException FromPool(string pool)
        {
            string msg = "Invalid aspect pool: " + pool;
            return new InvalidAspectException(msg);
        }
    }
}
