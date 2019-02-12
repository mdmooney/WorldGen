using System;

namespace WorldGen
{
    class InvalidAspectException : Exception
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
    }
}
