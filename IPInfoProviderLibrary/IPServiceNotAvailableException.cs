using System;

namespace IPInfoProviderLibrary
{
    class IPServiceNotAvailableException : Exception
    {
        public IPServiceNotAvailableException()
        {
        }

        public IPServiceNotAvailableException(string message) : base($"IPService responds with {message}")
        {

        }
    }
}
