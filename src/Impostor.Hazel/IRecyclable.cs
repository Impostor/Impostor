namespace Impostor.Hazel
{
    /// <summary>
    ///     Interface for all items that can be returned to an object pool.
    /// </summary>
    /// <threadsafety static="true" instance="true"/>
    public interface IRecyclable
    {
        /// <summary>
        ///     Returns this object back to the object pool.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Calling this when you are done with the object returns the object back to a pool in order to be reused.
        ///         This can reduce the amount of work the GC has to do dramatically but it is optional to call this.
        ///     </para>
        ///     <para>
        ///         Calling this indicates to Hazel that this can be reused and thus you should only call this when you are
        ///         completely finished with the object as the contents can be overwritten at any point after.
        ///     </para>
        /// </remarks>
        void Recycle();
    }
}
