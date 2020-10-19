using System;

namespace Impostor.Hazel
{
    /// <summary>
    ///     Wrapper for exceptions thrown from Hazel.
    /// </summary>
    [Serializable]
    public class HazelException : Exception
    {
        internal HazelException(string msg) : base (msg)
        {

        }

        internal HazelException(string msg, Exception e) : base (msg, e)
        {

        }
    }
}
