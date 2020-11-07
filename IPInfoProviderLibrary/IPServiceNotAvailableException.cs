using System;

namespace IPInfoProviderLibrary
{
    class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException()
        {
        }

        public IPServiceNotAvailableException(string message) : base($"IPService responded with {message}")
        {

        }
    }
}
