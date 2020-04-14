using System;

namespace TeamA.Exogredient.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException()
        { 
        }

        public NotAuthorizedException(string message) : base(message)
        {
        }

        public NotAuthorizedException(string message, Exception inner) :base(message, inner)
        {
        }
    }
}
